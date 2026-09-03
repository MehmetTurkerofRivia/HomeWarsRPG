using UnityEngine;

public abstract class WeaponBehaviour : MonoBehaviour
{
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private Sprite icon;
    [SerializeField] private float cooldown = 0.25f;
    [SerializeField] private bool rotatesOwnerOnPrimaryAttack;

    public string WeaponName => weaponName;
    public Sprite Icon => icon;
    public float Cooldown => cooldown;
    public bool RotatesOwnerOnPrimaryAttack => rotatesOwnerOnPrimaryAttack;
    public virtual bool IsSword => false;
    public virtual bool IsStaff => false;

    public abstract void UsePrimary(PlayerInventory owner, Vector2 aimDirection);
    public abstract void UseSecondary(PlayerInventory owner, Vector2 aimDirection);
}
