using System.Collections;
using UnityEngine;

public class FireStaffWeapon : WeaponBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float delayBeforeStrike = 0f;
    [SerializeField] private float meteorStartHeight = 11f;
    [SerializeField] private float meteorFallDuration = 2f;
    [SerializeField] private float meteorStartScaleMultiplier = 2.5f;
    [SerializeField] private float strikeRadius = 1.4f;
    [SerializeField] private float damage = 35f;
    [SerializeField] private float cameraShakeDuration = 0.18f;
    [SerializeField] private float cameraShakeStrength = 0.12f;

    public override bool IsStaff => true;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnMeteorStrike(owner, aimDirection, false);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnMeteorStrike(owner, aimDirection, true);
    }

    private void SpawnMeteorStrike(PlayerInventory owner, Vector2 aimDirection, bool strong)
    {
        if (aimDirection == Vector2.zero)
            return;

        Vector2 targetPosition = owner.GetMouseWorldPosition();
        GameObject marker = null;
        if (markerPrefab != null)
        {
            marker = Instantiate(markerPrefab, targetPosition, Quaternion.identity);
            owner.StartCoroutine(GrowMarker(marker));
        }

        owner.StartCoroutine(StrikeAfterDelay(owner, targetPosition, strong, marker));
    }

    private IEnumerator GrowMarker(GameObject marker)
    {
        if (marker == null)
            yield break;

        Transform markerTransform = marker.transform;
        Vector3 targetScale = markerTransform.localScale;
        markerTransform.localScale = Vector3.zero;

        const float duration = 1f;
        const float growthRate = 6f;
        float elapsed = 0f;
        float normalization = 1f - Mathf.Exp(-growthRate);

        while (elapsed < duration)
        {
            if (marker == null)
                yield break;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float logarithmicProgress = (1f - Mathf.Exp(-growthRate * progress)) / normalization;
            markerTransform.localScale = targetScale * logarithmicProgress;
            yield return null;
        }

        if (marker != null)
            markerTransform.localScale = targetScale;
    }

    private IEnumerator StrikeAfterDelay(PlayerInventory owner, Vector2 targetPosition, bool strong, GameObject marker)
    {
        yield return new WaitForSeconds(delayBeforeStrike);

        if (meteorPrefab != null)
            yield return DropMeteor(targetPosition, marker, strong);
        else if (marker != null)
            Destroy(marker);

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

    private IEnumerator DropMeteor(Vector2 targetPosition, GameObject marker, bool strong)
    {
        Vector3 startPosition = targetPosition + Vector2.up * meteorStartHeight;
        GameObject meteor = Instantiate(meteorPrefab, startPosition, Quaternion.identity);
        Vector3 targetScale = meteor.transform.localScale;
        meteor.transform.localScale = targetScale * meteorStartScaleMultiplier;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, meteorFallDuration);
        const float fallRate = 2f;
        float normalization = Mathf.Exp(fallRate) - 1f;
        bool markerDestroyed = false;

        while (elapsed < duration)
        {
            if (meteor == null)
                yield break;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float logarithmicProgress = (Mathf.Exp(fallRate * progress) - 1f) / normalization;
            meteor.transform.position = Vector3.Lerp(startPosition, targetPosition, logarithmicProgress);
            meteor.transform.localScale = Vector3.Lerp(
                targetScale * meteorStartScaleMultiplier,
                targetScale,
                logarithmicProgress);

            if (!markerDestroyed && marker != null && duration - elapsed <= 0.1f)
            {
                Destroy(marker);
                markerDestroyed = true;
            }

            yield return null;
        }

        if (meteor != null)
        {
            meteor.transform.position = targetPosition;
            if (marker != null)
                Destroy(marker);

            CameraShake cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShake>() : null;
            if (cameraShake != null)
                cameraShake.Shake(cameraShakeDuration, strong ? cameraShakeStrength * 1.35f : cameraShakeStrength);

            yield return new WaitForSeconds(0.3f);
            Destroy(meteor);
        }
    }
}
