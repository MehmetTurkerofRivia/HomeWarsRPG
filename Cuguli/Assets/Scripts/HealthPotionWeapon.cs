using UnityEngine;

public class HealthPotionWeapon : WeaponBehaviour
{
    [SerializeField] private int healAmount = 1;

    public override void UsePrimary(PlayerInventory owner, Vector2 aimDirection)
    {
        Heal(owner, healAmount);
    }

    public override void UseSecondary(PlayerInventory owner, Vector2 aimDirection)
    {
        Heal(owner, healAmount + 1);
    }

    private void Heal(PlayerInventory owner, int amount)
    {
        CharacterManager character = owner.GetComponent<CharacterManager>();
        if (character != null)
            character.TryHeal(amount);
    }
}
