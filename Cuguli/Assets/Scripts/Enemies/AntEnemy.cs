using UnityEngine;

public class AntEnemy : EnemyBase
{
    [Header("Ant Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Transform target;

    private Rigidbody2D body;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        if (body == null)
            body = gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        FindTarget();
    }

    private void FixedUpdate()
    {
        if (target == null)
            FindTarget();

        if (target == null)
            return;

        Vector2 direction = ((Vector2)target.position - body.position).normalized;
        body.linearVelocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision);
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
        moveSpeed = Mathf.Max(0f, moveSpeed);
    }
}
