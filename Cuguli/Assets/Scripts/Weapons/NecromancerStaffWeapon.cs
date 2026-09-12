using UnityEngine;

public class NecromancerStaffWeapon : WeaponBehaviour
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private int summonCount = 4;
    [SerializeField] private float summonRadius = 1.5f;
    [SerializeField] private float summonLifetime = 8f;
    [SerializeField] private float damage = 15f;

    public override bool IsStaff => true;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnSummons(owner);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        SpawnSummons(owner);
    }

    private void SpawnSummons(PlayerInventory owner)
    {
        if (summonPrefab == null || summonCount <= 0)
            return;

        for (int i = 0; i < summonCount; i++)
        {
            float angle = (360f / summonCount) * i;
            Vector3 offset = Quaternion.Euler(0f, 0f, angle) * new Vector3(summonRadius, 0f, 0f);
            GameObject summon = Instantiate(summonPrefab, owner.transform.position + offset, Quaternion.identity);
            ContactDamage contactDamage = summon.GetComponent<ContactDamage>();
            if (contactDamage != null)
            {
                contactDamage.SetDamage(damage);
                contactDamage.SetDestroyOnHit(false);
            }

            Destroy(summon, summonLifetime);
        }
    }
}
