using UnityEngine;

public class CockroachEnemy : EnemyBase
{
    [Header("Cockroach Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runDuration = 2f;
    [SerializeField] private Transform target;

    private Rigidbody2D body;
    private Vector2 movementDirection;
    private float stateTime;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        if (body == null)
            body = gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        FindTarget();
        FaceTargetHorizontally(target);
        ChooseDirection();
    }

    private void Update()
    {
        stateTime -= Time.deltaTime;
        if (stateTime <= 0f)
            ChooseDirection();
    }

    private void FixedUpdate()
    {
        if (target == null)
            FindTarget();

        FaceTargetHorizontally(target);
        body.linearVelocity = movementDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision);
    }

    private void ChooseDirection()
    {
        movementDirection = Random.insideUnitCircle.normalized;
        if (movementDirection == Vector2.zero)
            movementDirection = Vector2.right;
        stateTime = runDuration;
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
        runDuration = Mathf.Max(0.01f, runDuration);
    }
}
