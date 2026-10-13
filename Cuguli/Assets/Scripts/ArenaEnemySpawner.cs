using System.Collections.Generic;
using UnityEngine;

public class ArenaEnemySpawner : MonoBehaviour
{
    [Header("Arena")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 7f;

    [Header("Ants")]
    [SerializeField] private GameObject antPrefab;
    [SerializeField] private float antSpawnInterval = 0.35f;
    [SerializeField] private int antMaxAlive = 8;
    [SerializeField] private int antTotalCount = 20;

    [Header("Spiders")]
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private int spiderCount = 2;

    [Header("Bullet Soldiers")]
    [SerializeField] private GameObject bulletSoldierPrefab;
    [SerializeField] private int bulletSoldierCount = 4;

    [Header("Cockroaches")]
    [SerializeField] private GameObject cockroachPrefab;
    [SerializeField] private int cockroachCount = 3;

    private float nextAntSpawnTime;
    private int antsSpawned;

    private void Awake()
    {
        SpawnPlayerAtCenter();

        if (player == null)
        {
            CharacterManager playerObject = FindFirstObjectByType<CharacterManager>();
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player != null)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.RebuildWeaponVisuals();
        }

        SpawnInitialEnemies();
    }

    private void SpawnPlayerAtCenter()
    {
        Vector2 center = playerSpawnPoint != null
            ? (Vector2)playerSpawnPoint.position
            : (Vector2)transform.position;

        if (player == null && playerPrefab != null)
        {
            GameObject spawnedPlayer = Instantiate(playerPrefab, center, Quaternion.identity);
            player = spawnedPlayer.transform;
            return;
        }

        if (player != null)
            player.position = center;
    }

    private void Update()
    {
        if (antPrefab == null || antsSpawned >= antTotalCount || Time.time < nextAntSpawnTime)
            return;

        if (CountAlive(antPrefab) >= antMaxAlive)
            return;

        Spawn(antPrefab, GetEdgePosition());
        antsSpawned++;
        nextAntSpawnTime = Time.time + antSpawnInterval;
    }

    private void SpawnInitialEnemies()
    {
        List<int> availableSpawnPoints = CreateAvailableSpawnPoints();
        SpawnMultiple(spiderPrefab, spiderCount, availableSpawnPoints);
        SpawnMultiple(bulletSoldierPrefab, bulletSoldierCount, availableSpawnPoints);
        SpawnMultiple(cockroachPrefab, cockroachCount, availableSpawnPoints);
    }

    private List<int> CreateAvailableSpawnPoints()
    {
        List<int> available = new List<int>();
        if (spawnPoints == null)
            return available;

        for (int i = 0; i < spawnPoints.Length; i++)
            available.Add(i);

        return available;
    }

    private void SpawnMultiple(GameObject prefab, int count, List<int> availableSpawnPoints)
    {
        if (prefab == null)
            return;

        int spawnCount = spawnPoints != null && spawnPoints.Length > 0
            ? Mathf.Min(count, availableSpawnPoints.Count)
            : count;

        for (int i = 0; i < spawnCount; i++)
        {
            int pointIndex = -1;
            if (availableSpawnPoints.Count > 0)
            {
                int availableIndex = Random.Range(0, availableSpawnPoints.Count);
                pointIndex = availableSpawnPoints[availableIndex];
                availableSpawnPoints.RemoveAt(availableIndex);
            }

            Spawn(prefab, GetSpawnPosition(pointIndex));
        }
    }

    private void Spawn(GameObject prefab, Vector2 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    private Vector2 GetSpawnPosition(int index)
    {
        if (index >= 0 && spawnPoints != null && index < spawnPoints.Length)
            return spawnPoints[index].position;

        Vector2 center = player != null ? (Vector2)player.position : Vector2.zero;
        return center + Random.insideUnitCircle * spawnRadius;
    }

    private Vector2 GetEdgePosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
            return spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        Vector2 direction = Random.insideUnitCircle.normalized;
        if (direction == Vector2.zero)
            direction = Vector2.right;
        Vector2 center = player != null ? (Vector2)player.position : Vector2.zero;
        return center + direction * spawnRadius;
    }

    private int CountAlive(GameObject prefab)
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        int count = 0;
        foreach (EnemyBase enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.name.StartsWith(prefab.name))
                count++;
        }

        return count;
    }

    private void OnValidate()
    {
        antSpawnInterval = Mathf.Max(0.05f, antSpawnInterval);
        antMaxAlive = Mathf.Max(0, antMaxAlive);
        antTotalCount = Mathf.Max(0, antTotalCount);
        spiderCount = Mathf.Max(0, spiderCount);
        bulletSoldierCount = Mathf.Max(0, bulletSoldierCount);
        cockroachCount = Mathf.Max(0, cockroachCount);
        spawnRadius = Mathf.Max(0f, spawnRadius);
    }
}
