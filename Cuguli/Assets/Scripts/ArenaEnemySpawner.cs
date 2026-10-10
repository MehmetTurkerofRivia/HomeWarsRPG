using UnityEngine;

public class ArenaEnemySpawner : MonoBehaviour
{
    [Header("Arena")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 7f;

    [Header("Ants")]
    [SerializeField] private GameObject antPrefab;
    [SerializeField] private float antSpawnInterval = 1f;
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
        if (player == null)
        {
            CharacterManager playerObject = FindFirstObjectByType<CharacterManager>();
            if (playerObject != null)
                player = playerObject.transform;
        }

        SpawnInitialEnemies();
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
        SpawnMultiple(spiderPrefab, spiderCount);
        SpawnMultiple(bulletSoldierPrefab, bulletSoldierCount);
        SpawnMultiple(cockroachPrefab, cockroachCount);
    }

    private void SpawnMultiple(GameObject prefab, int count)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < count; i++)
            Spawn(prefab, GetSpawnPosition(i));
    }

    private void Spawn(GameObject prefab, Vector2 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    private Vector2 GetSpawnPosition(int index)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
            return spawnPoints[index % spawnPoints.Length].position;

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
