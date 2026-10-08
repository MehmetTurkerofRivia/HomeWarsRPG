using UnityEngine;

public class BowWeapon : WeaponBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float damage = 25f;

    public override bool IsBow => true;
    public override bool IsSword => false;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        FireArrow(owner, aimDirection);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        FireArrow(owner, aimDirection);
    }

    public Vector2 GetAimDirection(PlayerInventory owner)
    {
        Transform visualSpawnPoint = GetVisualSpawnPoint(owner);
        if (visualSpawnPoint == null)
            return Vector2.zero;

        Vector2 direction = owner.GetMouseWorldPosition() - (Vector2)visualSpawnPoint.position;
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.zero;
    }

    private void FireArrow(PlayerInventory owner, Vector2 aimDirection)
    {
        if (arrowPrefab == null)
            return;

        Transform visualSpawnPoint = GetVisualSpawnPoint(owner);
        if (visualSpawnPoint == null)
            return;

        Vector3 spawnPosition = visualSpawnPoint.position;
        Vector2 arrowDirection = GetAimDirection(owner);
        if (arrowDirection == Vector2.zero)
            return;

        float velocityAngle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.Euler(0f, 0f, velocityAngle - 90f));
        Transform arrowVisual = arrow.GetComponentInChildren<SpriteRenderer>()?.transform;
        if (arrowVisual != null)
        {
            Vector3 visualOffset = arrowVisual.position - arrow.transform.position;
            arrow.transform.position -= visualOffset;
            arrowVisual.position = spawnPosition;
        }

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

    private Transform GetVisualSpawnPoint(PlayerInventory owner)
    {
        if (arrowSpawnPoint == null)
            return null;

        Transform visualTransform = owner.GetWeaponVisualTransform(this);
        if (visualTransform == null)
            return null;

        string spawnPointPath = GetRelativePath(transform, arrowSpawnPoint);
        return visualTransform.Find(spawnPointPath);
    }

    private static string GetRelativePath(Transform root, Transform target)
    {
        string path = target.name;
        Transform current = target.parent;
        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
