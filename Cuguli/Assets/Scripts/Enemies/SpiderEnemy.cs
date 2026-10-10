using UnityEngine;

public class SpiderEnemy : EnemyBase
{
    [Header("Spider Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private float webInterval = 3f;
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private Transform target;

    private Rigidbody2D body;
    private Vector2 movementDirection;
    private float stateTime;
    private float nextWebTime;
    private bool isMoving = true;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        if (body == null)
            body = gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        ChooseDirection();
        nextWebTime = Time.time + webInterval;
        FindTarget();
    }

    private void Update()
    {
        if (Time.time >= nextWebTime)
        {
            SpawnWeb();
            nextWebTime = Time.time + webInterval;
        }

        stateTime -= Time.deltaTime;
        if (stateTime > 0f)
            return;

        isMoving = !isMoving;
        if (isMoving)
            ChooseDirection();
        else
        {
            stateTime = pauseDuration;
            body.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            FindTarget();

        body.linearVelocity = isMoving ? movementDirection * moveSpeed : Vector2.zero;
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
        stateTime = moveDuration;
    }

    private void SpawnWeb()
    {
        if (webPrefab != null)
            Instantiate(webPrefab, transform.position, Quaternion.identity);
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
        moveDuration = Mathf.Max(0.01f, moveDuration);
        pauseDuration = Mathf.Max(0f, pauseDuration);
        webInterval = Mathf.Max(0.01f, webInterval);
    }
}
