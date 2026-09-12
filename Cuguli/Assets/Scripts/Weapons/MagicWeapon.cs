using System.Collections;
using UnityEngine;

public class MagicWeapon : WeaponBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float delayBeforeStrike = 2f;
    [SerializeField] private float strikeRadius = 1.4f;
    [SerializeField] private float damage = 35f;
    [SerializeField] private float cameraShakeDuration = 0.18f;
    [SerializeField] private float cameraShakeStrength = 0.12f;

    public override bool IsStaff => true;

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
            GameObject marker = Instantiate(markerPrefab, targetPosition, Quaternion.identity);
            Destroy(marker, Mathf.Max(0f, delayBeforeStrike - 0.2f));
        }

        owner.StartCoroutine(StrikeAfterDelay(owner, targetPosition, strong));
    }

    private IEnumerator StrikeAfterDelay(PlayerInventory owner, Vector2 targetPosition, bool strong)
    {
        yield return new WaitForSeconds(delayBeforeStrike);

        if (lightningPrefab != null)
            Instantiate(lightningPrefab, targetPosition + Vector2.up * 1.5f, Quaternion.identity);

        CameraShake cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShake>() : null;
        if (cameraShake != null)
            cameraShake.Shake(cameraShakeDuration, strong ? cameraShakeStrength * 1.35f : cameraShakeStrength);

        float strikeDamage = strong ? damage * 1.5f : damage;
        float radius = strong ? strikeRadius * 1.2f : strikeRadius;
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, radius);
        foreach (Collider2D hit in hits)
        {
            Transform targetTransform = hit.transform;
            if (!targetTransform.CompareTag("Enemy"))
                targetTransform = targetTransform.root;

            if (targetTransform == owner.transform || !targetTransform.CompareTag("Enemy"))
                continue;

            if (targetTransform.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(strikeDamage);
        }
    }
}
