using System.Collections;
using UnityEngine;

public class SwordWeapon : WeaponBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float radius = 1.3f;
    [SerializeField] private GameObject swingEffect;

    public override bool IsSword => true;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        owner.StartCoroutine(DoSwordSwing(owner, false));
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        owner.StartCoroutine(DoSwordSwing(owner, true));
    }

    private IEnumerator DoSwordSwing(PlayerInventory owner, bool heavy)
    {
        if (swingEffect != null)
        {
            GameObject effect = Instantiate(swingEffect, owner.transform.position, Quaternion.identity);
            Destroy(effect, 0.2f);
        }

        float attackRadius = heavy ? radius * 1.35f : radius;
        float attackDamage = heavy ? damage * 1.5f : damage;
        Collider2D[] hits = Physics2D.OverlapCircleAll(owner.transform.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null || target is CharacterManager)
                continue;

            if (target is MonoBehaviour targetBehaviour && targetBehaviour.transform.root == owner.transform.root)
                continue;

            if (target != null)
                target.TakeDamage(attackDamage);
        }

        yield return null;
    }
}
