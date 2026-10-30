using UnityEngine;

// Spawner milik sendiri: memunculkan beberapa jenis musuh secara acak.
// Tidak mengubah script tim (Spawner, Health, EnemyMovement).
public class VariantSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyOption
    {
        public GameObject prefab;
        public int weight = 1; // makin besar, makin sering muncul
    }

    [Header("Daftar Musuh")]
    public EnemyOption[] enemies;

    [Header("Pengaturan Spawn")]
    public Transform target;          // isi dengan Player
    public float minDistance = 4f;    // jarak minimal dari pemain
    public float maxDistance = 7f;    // jarak maksimal dari pemain
    public float spawnInterval = 2f;  // jeda antar spawn (detik)

    private float timer;

    private void Update()
    {
        if (target == null || enemies == null || enemies.Length == 0)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefab = PickRandomPrefab();

        if (prefab == null)
        {
            return;
        }

        // Arah acak, lalu jarak acak antara min dan max
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 spawnPosition = target.position + (Vector3)(direction * distance);
        spawnPosition.z = target.position.z;

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    // Pilih prefab berdasarkan bobot (weight)
    private GameObject PickRandomPrefab()
    {
        int totalWeight = 0;

        foreach (EnemyOption option in enemies)
        {
            if (option.prefab != null)
            {
                totalWeight += option.weight;
            }
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        int roll = Random.Range(0, totalWeight);

        foreach (EnemyOption option in enemies)
        {
            if (option.prefab == null)
            {
                continue;
            }

            if (roll < option.weight)
            {
                return option.prefab;
            }

            roll -= option.weight;
        }

        return null;
    }
}