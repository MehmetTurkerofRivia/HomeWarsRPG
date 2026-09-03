using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private bool damageOnlyOncePerTarget = true;

    private bool hasHit;

    public void SetDamage(float value)
    {
        damage = value;
    }

    public void SetDestroyOnHit(bool value)
    {
        destroyOnHit = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    private void TryDamage(Collider2D other)
    {
        if (hasHit && damageOnlyOncePerTarget)
            return;

        Transform targetTransform = other.transform;
        if (!targetTransform.CompareTag("Enemy"))
            targetTransform = targetTransform.root;

        if (!targetTransform.CompareTag("Enemy"))
            return;

        IDamageable target = targetTransform.GetComponentInParent<IDamageable>();
        if (target == null)
            return;

        target.TakeDamage(damage);
        hasHit = true;

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
