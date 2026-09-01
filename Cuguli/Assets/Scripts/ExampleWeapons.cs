using System.Collections;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

[CreateAssetMenu(menuName = "Weapons/Sword Weapon")]
public class SwordWeaponItem : WeaponItem
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float radius = 1.3f;
    [SerializeField] private GameObject swingEffect;

    public override bool IsSword => true;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        owner.StartCoroutine(DoSwordSwing(owner, aimDirection));
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        owner.StartCoroutine(DoSwordSwing(owner, aimDirection, true));
    }

    private IEnumerator DoSwordSwing(PlayerInventory owner, Vector2 aimDirection, bool heavy = false)
    {
        if (swingEffect != null)
        {
            var effect = Object.Instantiate(swingEffect, owner.transform.position, Quaternion.identity);
            Object.Destroy(effect, 0.2f);
        }

        float attackRadius = heavy ? radius * 1.35f : radius;
        float attackDamage = heavy ? damage * 1.5f : damage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(owner.transform.position, attackRadius);

        foreach (var hit in hits)
        {
            if (hit.transform == owner.transform)
                continue;

            if (hit.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(attackDamage);
            }
        }

        yield return null;
    }
}

[CreateAssetMenu(menuName = "Weapons/Bow Weapon")]
public class BowWeaponItem : WeaponItem
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float projectileSpeed = 10f;

    public override bool IsBow => true;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        Debug.Log($"UsePrimary called - aimDirection: {aimDirection}, arrowPrefab: {arrowPrefab}");
        
        // Rastgele -15 ile +15 derece arasında
        float randomOffset = Random.Range(-15f, 15f);
        
        if (aimDirection == Vector2.zero)
        {
            Debug.Log("aimDirection is zero, returning");
            return;
        }
        
        if (arrowPrefab == null)
        {
            Debug.Log("arrowPrefab is null, returning");
            return;
        }

        // Mouse yönünün açısı
        float baseAngleDeg = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float finalAngleDeg = baseAngleDeg + randomOffset;
        
        // OK yönü
        float angleRad = finalAngleDeg * Mathf.Deg2Rad;
        Vector2 arrowDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        
        // Spawn pozisyonu
        Vector3 spawnPos = owner.transform.position + (Vector3)aimDirection.normalized * 0.8f;
        
        // Okun rotation'ını velocity yönüne göre hesapla
        float velocityAngle = Mathf.Atan2(arrowDir.y, arrowDir.x) * Mathf.Rad2Deg;
        
        // OK oluştur ve ata - direkt doğru açıda
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.Euler(0, 0, velocityAngle - 90f));
        Debug.Log($"Arrow created at {spawnPos}");
        
        // Fizik ayarla
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = arrow.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = arrowDir * projectileSpeed;
        
        Debug.Log($"Arrow spawned successfully with velocity: {arrowDir * projectileSpeed}");
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        // Secondary için hiçbir şey yapma (ya da double arrow istersen burada yaz)
    }
}

[CreateAssetMenu(menuName = "Weapons/Magic Weapon")]
public class MagicWeaponItem : WeaponItem
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float delayBeforeStrike = 2f;
    [SerializeField] private float strikeRadius = 1.4f;
    [SerializeField] private float damage = 35f;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnLightningStrike(owner, aimDirection, false);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnLightningStrike(owner, aimDirection, true);
    }

    private void SpawnLightningStrike(PlayerInventory owner, Vector2 aimDirection, bool strong)
    {
        if (aimDirection == Vector2.zero)
            return;

        Vector2 targetPosition = owner.GetMouseWorldPosition();

        if (markerPrefab != null)
        {
            var marker = Object.Instantiate(markerPrefab, targetPosition, Quaternion.identity);
            Object.Destroy(marker, delayBeforeStrike - 0.2f);
        }

        owner.StartCoroutine(StrikeAfterDelay(owner, targetPosition, strong));
    }

    private IEnumerator StrikeAfterDelay(PlayerInventory owner, Vector2 targetPosition, bool strong)
    {
        yield return new WaitForSeconds(delayBeforeStrike);

        if (lightningPrefab != null)
        {
            Vector2 lightningSpawnPos = targetPosition + Vector2.up * 1.5f;
            Object.Instantiate(lightningPrefab, lightningSpawnPos, Quaternion.identity);
        }

        float strikeDamage = strong ? damage * 1.5f : damage;
        float radius = strong ? strikeRadius * 1.2f : strikeRadius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, radius);
        foreach (var hit in hits)
        {
            if (hit.transform == owner.transform)
                continue;

            if (hit.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(strikeDamage);
        }
    }
}

[CreateAssetMenu(menuName = "Items/Health Potion")]
public class HealthPotionItem : WeaponItem
{
    [SerializeField] private int healAmount = 1;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        CharacterManager character = owner.GetComponent<CharacterManager>();
        if (character != null)
            character.TryHeal(healAmount);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        CharacterManager character = owner.GetComponent<CharacterManager>();
        if (character != null)
            character.TryHeal(healAmount + 1);
    }
}

[CreateAssetMenu(menuName = "Weapons/Necromancer Staff")]
public class NecromancerStaffItem : WeaponItem
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private int summonCount = 4;
    [SerializeField] private float summonRadius = 1.5f;
    [SerializeField] private float summonLifetime = 8f;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnSummons(owner);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnSummons(owner, true);
    }

    private void SpawnSummons(PlayerInventory owner, bool stronger = false)
    {
        if (summonPrefab == null)
        {
            Debug.Log("Necromancer staff has no summon prefab.");
            return;
        }

        for (int i = 0; i < summonCount; i++)
        {
            float angle = (360f / summonCount) * i;
            Vector3 offset = Quaternion.Euler(0f, 0f, angle) * new Vector3(summonRadius, 0f, 0f);
            var summon = Object.Instantiate(summonPrefab, owner.transform.position + offset, Quaternion.identity);
            Object.Destroy(summon, summonLifetime);
        }
    }
}
