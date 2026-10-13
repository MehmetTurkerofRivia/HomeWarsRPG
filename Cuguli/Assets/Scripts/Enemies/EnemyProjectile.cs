using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 6f;

    public void SetDamage(float value)
    {
        damage = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        gameObject.tag = "EnemyProjectile";
        transform.rotation = Quaternion.identity;

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
            body.constraints |= RigidbodyConstraints2D.FreezeRotation;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HitPlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitPlayer(collision.collider);
    }

    private void HitPlayer(Collider2D other)
    {
        CharacterManager player = other.GetComponentInParent<CharacterManager>();
        if (player == null)
            return;

        player.TakeDamage(damage);
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        damage = Mathf.Max(0f, damage);
        lifetime = Mathf.Max(0.1f, lifetime);
    }
}
