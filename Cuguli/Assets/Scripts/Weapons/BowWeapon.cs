using UnityEngine;

public class BowWeapon : WeaponBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float damage = 25f;

    public override bool IsSword => false;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        if (aimDirection == Vector2.zero || arrowPrefab == null)
            return;

        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float finalAngle = baseAngle + Random.Range(-spreadAngle, spreadAngle);
        float angleRadians = finalAngle * Mathf.Deg2Rad;
        Vector2 arrowDirection = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        Vector3 spawnPosition = owner.transform.position + (Vector3)aimDirection.normalized * 0.8f;
        float velocityAngle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.Euler(0f, 0f, velocityAngle - 90f));
        ContactDamage contactDamage = arrow.GetComponent<ContactDamage>();
        if (contactDamage != null)
            contactDamage.SetDamage(damage);

        Rigidbody2D body = arrow.GetComponent<Rigidbody2D>();
        if (body == null)
            body = arrow.AddComponent<Rigidbody2D>();

        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.linearVelocity = arrowDirection * projectileSpeed;
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
    }
}
