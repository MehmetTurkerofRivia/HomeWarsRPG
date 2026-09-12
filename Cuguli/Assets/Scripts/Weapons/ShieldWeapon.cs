using UnityEngine;

public class ShieldWeapon : WeaponBehaviour
{
    public override bool IsShield => true;

    public void Activate()
    {
        if (transform.parent != null)
        {
            transform.RotateAround(transform.parent.position, Vector3.forward, -90f);
            return;
        }

        transform.rotation *= Quaternion.Euler(0f, 0f, -90f);
    }

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("EnemyProjectile"))
            return;

        Destroy(other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("EnemyProjectile"))
            return;

        Destroy(collision.gameObject);
    }
}