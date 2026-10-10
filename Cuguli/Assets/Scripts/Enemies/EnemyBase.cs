using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float currentHealth = 30f;

    [Header("Contact Damage")]
    [SerializeField] protected float contactDamage = 10f;
    [SerializeField] protected float contactCooldown = 0.75f;

    private float nextContactDamageTime;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || currentHealth <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected bool TryDamagePlayer(Collider2D other)
    {
        if (Time.time < nextContactDamageTime)
            return false;

        CharacterManager player = other.GetComponentInParent<CharacterManager>();
        if (player == null)
            return false;

        player.TakeDamage(contactDamage);
        nextContactDamageTime = Time.time + contactCooldown;
        return true;
    }

    protected bool TryDamagePlayer(Collision2D collision)
    {
        return TryDamagePlayer(collision.collider);
    }

    protected virtual void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        contactDamage = Mathf.Max(0f, contactDamage);
        contactCooldown = Mathf.Max(0f, contactCooldown);
    }
}
