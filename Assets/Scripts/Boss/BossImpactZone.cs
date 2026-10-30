using UnityEngine;

public class BossImpactZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    public float impactRadius = 1.6f;
    public float windupDuration = 2.0f;
    public Color normalWarningColor = new Color(1f, 0.2f, 0.2f, 0.25f);
    public Color flashColor = new Color(1f, 0.1f, 0.1f, 0.65f);

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool hasTriggered = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public void Setup(float duration, float radius)
    {
        windupDuration = duration;
        impactRadius = radius;
        transform.localScale = Vector3.one * (impactRadius * 2f);
    }

    private void Start()
    {
        transform.localScale = Vector3.one * (impactRadius * 2f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalWarningColor;
        }
        // Auto cleanup fallback
        Destroy(gameObject, windupDuration + 1.5f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / windupDuration);

        // Semakin mendekati 2 detik, kedipannya semakin cepat
        if (spriteRenderer != null)
        {
            float pulse = Mathf.PingPong(timer * (3f + progress * 8f), 1f);
            spriteRenderer.color = Color.Lerp(normalWarningColor, flashColor, pulse);
        }
    }

    public void TriggerImpact(int damage)
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player terkena Slam Area Boss! Damage: " + damage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
