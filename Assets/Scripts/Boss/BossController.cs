using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class BossController : MonoBehaviour
{
    public enum BossState
    {
        Normal,
        Windup,
        Dashing,
        Recovery
    }

    [Header("Current State")]
    [SerializeField] private BossState currentState = BossState.Normal;

    [Header("Movement Settings")]
    public float moveSpeed = 1.2f;
    public float dashSpeed = 16f;
    public float dashCooldown = 9f;
    public float dashTriggerDistance = 6.5f;
    public float windupDuration = 2.0f;
    public float recoveryDuration = 0.4f;

    [Header("Combat & Damage")]
    public int contactDamage = 3;
    public float contactDamageCooldown = 1.0f;
    private float nextContactDamageTime = 0f;

    [Header("Ranged Attack (Spread 3 Peluru)")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 5.0f;
    public float spreadAngle = 15.0f;
    private float shootTimer = 0f;

    [Header("Dash Impact Zone")]
    public GameObject impactZonePrefab;
    public float impactRadius = 2.2f;

    [Header("Visual References")]
    public Transform visualContainer;
    public SpriteRenderer[] visualRenderers;

    private Color[] originalColors;
    private Transform player;
    private Rigidbody2D rb;
    private Health health;

    private float dashCooldownTimer = 0f;
    private float stateTimer = 0f;
    private Vector2 dashTargetPosition;
    private Vector3 initialVisualLocalPos;
    private Vector3 initialVisualScale = Vector3.one;
    private BossImpactZone currentImpactZone;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (visualContainer != null)
        {
            initialVisualLocalPos = visualContainer.localPosition;
            initialVisualScale = visualContainer.localScale;
            if (visualRenderers == null || visualRenderers.Length == 0)
            {
                visualRenderers = visualContainer.GetComponentsInChildren<SpriteRenderer>();
            }
        }
        else
        {
            initialVisualLocalPos = Vector3.zero;
            initialVisualScale = Vector3.one;
            visualRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        // Simpan warna asli tiap partikel blok agar tidak tertimpa permanen saat flash
        if (visualRenderers != null && visualRenderers.Length > 0)
        {
            originalColors = new Color[visualRenderers.Length];
            for (int i = 0; i < visualRenderers.Length; i++)
            {
                if (visualRenderers[i] != null)
                {
                    originalColors[i] = visualRenderers[i].color;
                }
            }
        }

        dashCooldownTimer = dashCooldown * 0.5f; // Dash pertama siap lebih cepat
        shootTimer = shootInterval * 0.4f;
    }

    private void Update()
    {
        // Cari player jika belum ditemukan atau respawn
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            return;
        }

        switch (currentState)
        {
            case BossState.Normal:
                HandleNormalUpdate();
                break;

            case BossState.Windup:
                HandleWindupUpdate();
                break;

            case BossState.Dashing:
                HandleDashingUpdate();
                break;

            case BossState.Recovery:
                HandleRecoveryUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (currentState == BossState.Normal)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // Menghadap ke arah pemain dengan mempertahankan skala visual
            if (visualContainer != null)
            {
                float sx = Mathf.Abs(initialVisualScale.x);
                float sy = initialVisualScale.y;
                if (direction.x > 0.05f)
                    visualContainer.localScale = new Vector3(-sx, sy, 1f);
                else if (direction.x < -0.05f)
                    visualContainer.localScale = new Vector3(sx, sy, 1f);
            }
        }
        else if (currentState == BossState.Dashing)
        {
            Vector2 dashDir = (dashTargetPosition - rb.position).normalized;
            rb.linearVelocity = dashDir * dashSpeed;

            // Cek jika sudah sangat dekat dengan target landasan
            if (Vector2.Distance(rb.position, dashTargetPosition) <= 0.35f)
            {
                LandAndSlam();
            }
        }
        else
        {
            // Saat Windup dan Recovery, boss diam di tempat
            rb.linearVelocity = Vector2.zero;
        }
    }

    #region State Handlers

    private void HandleNormalUpdate()
    {
        // 1. Logika Tembak 3 Peluru Berkala (Opsi A - Spread)
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootSpreadBullets();
            shootTimer = 0f;
        }

        // 2. Logika Pemicu Dash (Kombinasi Timer dan Jarak)
        dashCooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool readyToDash = dashCooldownTimer <= 0f;
        bool farAway = distanceToPlayer >= dashTriggerDistance;
        bool waitedTooLong = dashCooldownTimer <= -4f; // batas toleransi jika player terlalu dekat

        if (readyToDash && (farAway || waitedTooLong))
        {
            StartDashWindup();
        }
    }

    private void StartDashWindup()
    {
        currentState = BossState.Windup;
        stateTimer = 0f;
        rb.linearVelocity = Vector2.zero;

        // Kunci target posisi player saat charge DIMULAI (tidak bergeser)
        dashTargetPosition = player.position;

        // Munculkan tanda zona bahaya di tempat player saat charge dimulai
        if (impactZonePrefab != null)
        {
            GameObject zoneObj = Instantiate(impactZonePrefab, dashTargetPosition, Quaternion.identity);
            currentImpactZone = zoneObj.GetComponent<BossImpactZone>();
            if (currentImpactZone != null)
            {
                currentImpactZone.Setup(windupDuration, impactRadius);
            }
        }

        Debug.Log("Boss memulai ancang-ancang Dash! Menargetkan posisi: " + dashTargetPosition);
    }

    private void HandleWindupUpdate()
    {
        stateTimer += Time.deltaTime;

        // Efek Visual Ancang-ancang: getar (shake) dan kedip merah
        if (visualContainer != null)
        {
            float shakeAmount = 0.08f * (stateTimer / windupDuration);
            visualContainer.localPosition = initialVisualLocalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
        }

        float flashIntensity = Mathf.PingPong(stateTimer * 8f, 1f);
        ApplyFlashEffect(flashIntensity);

        if (stateTimer >= windupDuration)
        {
            // Selesai ancang-ancang 2 detik -> melesat cepat ke target
            ResetVisualState();
            currentState = BossState.Dashing;
            stateTimer = 0f;
        }
    }

    private void HandleDashingUpdate()
    {
        stateTimer += Time.deltaTime;

        // Timeout fallback jika terhalang rintangan
        if (stateTimer >= 0.65f)
        {
            LandAndSlam();
        }
    }

    private void LandAndSlam()
    {
        rb.position = dashTargetPosition;
        rb.linearVelocity = Vector2.zero;

        // Berikan damage ke zona bahaya jika player belum kabur
        if (currentImpactZone != null)
        {
            currentImpactZone.TriggerImpact(contactDamage);
            currentImpactZone = null;
        }

        currentState = BossState.Recovery;
        stateTimer = 0f;
    }

    private void HandleRecoveryUpdate()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= recoveryDuration)
        {
            currentState = BossState.Normal;
            dashCooldownTimer = dashCooldown;
            shootTimer = 0f; // Beri jeda tembak sedikit setelah dash
            ResetVisualState();
        }
    }

    #endregion

    #region Attacks & Combat

    private void ShootSpreadBullets()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector2 baseDirection = ((Vector2)player.position - (Vector2)spawnPos).normalized;
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        // Opsi A: 3 Peluru Kipas (Spread: Tengah, Kiri +15 deg, Kanan -15 deg)
        float[] spreadOffsets = { 0f, spreadAngle, -spreadAngle };

        foreach (float offset in spreadOffsets)
        {
            float currentAngle = baseAngle + offset;
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 bulletDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            BossProjectile projScript = bullet.GetComponent<BossProjectile>();
            if (projScript != null)
            {
                projScript.SetDirection(bulletDir);
            }
        }

        Debug.Log("Boss menembakkan 3 peluru spread ke arah player!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryApplyContactDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryApplyContactDamage(collision);
    }

    private void TryApplyContactDamage(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= nextContactDamageTime)
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                nextContactDamageTime = Time.time + contactDamageCooldown;
                Debug.Log("Player terkena Contact Damage Boss sebesar " + contactDamage);
            }
        }
    }

    #endregion

    #region Visual Helpers

    private void ApplyFlashEffect(float intensity)
    {
        if (visualRenderers == null || originalColors == null) return;
        for (int i = 0; i < visualRenderers.Length; i++)
        {
            if (visualRenderers[i] != null && i < originalColors.Length)
            {
                visualRenderers[i].color = Color.Lerp(originalColors[i], Color.red, intensity * 0.7f);
            }
        }
    }

    private void ResetVisualState()
    {
        if (visualContainer != null)
        {
            visualContainer.localPosition = initialVisualLocalPos;
        }
        if (visualRenderers != null && originalColors != null)
        {
            for (int i = 0; i < visualRenderers.Length; i++)
            {
                if (visualRenderers[i] != null && i < originalColors.Length)
                {
                    visualRenderers[i].color = originalColors[i];
                }
            }
        }
    }

    #endregion
}
