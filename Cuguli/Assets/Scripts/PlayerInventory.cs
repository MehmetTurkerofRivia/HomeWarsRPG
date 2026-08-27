using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private WeaponItem slot1Weapon;
    [SerializeField] private WeaponItem slot2Weapon;

    private float nextPrimaryTime;
    private float nextSecondaryTime;

    public WeaponItem Slot1Weapon => slot1Weapon;
    public WeaponItem Slot2Weapon => slot2Weapon;

    private void Update()
    {
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                UsePrimaryAttack();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                UseSecondaryAttack();
        }
    }

    public void EquipSlot1(WeaponItem weapon)
    {
        slot1Weapon = weapon;
    }

    public void EquipSlot2(WeaponItem weapon)
    {
        slot2Weapon = weapon;
    }

    public void UsePrimaryAttack()
    {
        if (slot1Weapon == null)
            return;

        if (Time.time < nextPrimaryTime)
            return;

        slot1Weapon.UsePrimary(this, GetAimDirection());
        transform.Rotate(0f, 0f, 90f);
        nextPrimaryTime = Time.time + slot1Weapon.Cooldown;
    }

    public void UseSecondaryAttack()
    {
        if (slot2Weapon == null)
            return;

        if (Time.time < nextSecondaryTime)
            return;

        slot2Weapon.UseSecondary(this, GetAimDirection());
        nextSecondaryTime = Time.time + slot2Weapon.Cooldown;
    }

    private Vector2 GetAimDirection()
    {
        if (Mouse.current == null)
            return Vector2.right;

        Camera camera = Camera.main;
        if (camera == null)
            return Vector2.right;

        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f;

        Vector2 direction = (Vector2)(mouseWorldPosition - transform.position);
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
    }
}
