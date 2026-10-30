using UnityEngine;

/// <summary>
/// CameraFollow: Mengontrol Main Camera agar mengikuti posisi Player dengan mulus (smooth follow),
/// serta menyediakan interaksi Zoom In dan Zoom Out menggunakan mouse scroll wheel dalam batas tertentu.
/// 
/// Dibuat untuk proyek Raion Academy - Kelompok 4 (Daffa).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Target yang diikuti kamera. Jika kosong, script akan otomatis mencari objek ber-tag 'Player'.")]
    public Transform target;

    [Tooltip("Jarak offset kamera dari target. Nilai Z harus negatif (-10) untuk kamera 2D.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Tooltip("Tingkat kehalusan pergerakan kamera (makin kecil makin cepat mengikuti).")]
    public float smoothTime = 0.2f;

    [Header("Zoom Settings")]
    [Tooltip("Sensitivitas scroll wheel mouse saat melakukan zoom.")]
    public float zoomSensitivity = 3f;

    [Tooltip("Kecepatan transisi interpolasi zoom saat scroll diputar.")]
    public float zoomSpeed = 8f;

    [Tooltip("Batas ukuran kamera minimum (Zoom In terjauh / tampak paling dekat).")]
    public float minZoom = 3.5f;

    [Tooltip("Batas ukuran kamera maksimum (Zoom Out terjauh / tampak paling luas).")]
    public float maxZoom = 22f;

    [Tooltip("Ukuran default kamera ortografis saat permainan dimulai.")]
    public float defaultZoom = 8.5f;

    [Header("Map Bounds (Opsional)")]
    [Tooltip("Aktifkan bila pergerakan kamera ingin dibatasi pada koordinat tertentu.")]
    public bool useBounds = false;
    public Vector2 minBounds = new Vector2(-35f, -35f);
    public Vector2 maxBounds = new Vector2(35f, 35f);

    [Header("Auto-Attach Setting")]
    [Tooltip("Jika dicentang, CameraFollow akan otomatis aktif di Main Camera pada setiap scene tanpa perlu mengubah file scene.")]
    public static bool autoAttachOnSceneLoad = true;

    private Camera cam;
    private float targetZoom;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        defaultZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);

        if (cam != null && cam.orthographic)
        {
            // Mulai permainan dengan ukuran default yang lebih lapang
            cam.orthographicSize = defaultZoom;
            targetZoom = defaultZoom;
        }
        else
        {
            targetZoom = defaultZoom;
        }
    }

    private void Start()
    {
        FindPlayerTarget();
    }

    private void Update()
    {
        HandleZoom();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FindPlayerTarget()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + offset;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // Kunci koordinat Z kamera agar selalu berada di posisi Z offset (umumnya -10f)
        desiredPosition.z = offset.z;

        // Gerakkan kamera secara halus ke posisi target
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }

    private void HandleZoom()
    {
        if (cam == null || !cam.orthographic) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            // Scroll ke atas (positif) = memperkecil orthographicSize (Zoom In)
            // Scroll ke bawah (negatif) = memperbesar orthographicSize (Zoom Out)
            targetZoom -= scroll * zoomSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Interpolasi halus perubahan zoom
        if (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.001f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }

    /// <summary>
    /// Helper publik untuk mengubah target yang diikuti kamera secara dinamis jika diperlukan.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Otomatis memasangkan CameraFollow ke Main Camera saat scene berjalan.
    /// Hal ini memungkinkan fitur kamera aktif di scene manapun tanpa harus memodifikasi file scene di Git.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoAttachRuntime()
    {
        if (!autoAttachOnSceneLoad) return;

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            if (mainCam.GetComponent<CameraFollow>() == null)
            {
                mainCam.gameObject.AddComponent<CameraFollow>();
                Debug.Log("[CameraFollow] Berhasil dipasangkan otomatis ke Main Camera saat runtime.");
            }
        }
    }
}
