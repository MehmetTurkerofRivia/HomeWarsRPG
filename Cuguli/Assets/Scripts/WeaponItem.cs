using UnityEngine;

public abstract class WeaponItem : ScriptableObject
{
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private Sprite icon;
    [SerializeField] private float cooldown = 0.25f;

    public string WeaponName => weaponName;
    public Sprite Icon => icon;
    public float Cooldown => cooldown;

    public abstract void UsePrimary(PlayerInventory owner, Vector2 aimDirection);
    public abstract void UseSecondary(PlayerInventory owner, Vector2 aimDirection);
}
