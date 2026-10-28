
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform target;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float spawnInterval = 2f;

    private float spawnTimer;

    private void Update()
    {
        if (prefab == null || target == null)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnPrefab();
            spawnTimer = 0f;
        }
    }

    private void SpawnPrefab()
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPosition = new Vector3(
            target.position.x + randomOffset.x,
            target.position.y + randomOffset.y,
            target.position.z
        );

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
