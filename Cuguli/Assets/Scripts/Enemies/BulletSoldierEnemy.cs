using UnityEngine;

public class BulletSoldierEnemy : EnemyBase
{
    [Header("Bullet Soldier")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float fireInterval = 1.5f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileDamage = 12f;
    [SerializeField] private Transform target;

    private float nextFireTime;

    protected override void Awake()
    {
        base.Awake();
        FindTarget();
        nextFireTime = Time.time + fireInterval;
    }

    private void Update()
    {
        if (target == null)
            FindTarget();

        if (target != null && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || target == null)
            return;

        Transform spawnPoint = muzzle != null ? muzzle : transform;
        Vector2 direction = ((Vector2)target.position - (Vector2)spawnPoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        EnemyProjectile projectileBehaviour = projectile.GetComponent<EnemyProjectile>();
        if (projectileBehaviour != null)
            projectileBehaviour.SetDamage(projectileDamage);

        Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
        if (body == null)
            body = projectile.AddComponent<Rigidbody2D>();

        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.linearVelocity = direction * projectileSpeed;
    }

    private void FindTarget()
    {
        CharacterManager player = FindFirstObjectByType<CharacterManager>();
        if (player != null)
            target = player.transform;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        fireInterval = Mathf.Max(0.05f, fireInterval);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileDamage = Mathf.Max(0f, projectileDamage);
    }
}
