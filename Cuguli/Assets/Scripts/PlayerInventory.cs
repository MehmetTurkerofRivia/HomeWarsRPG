using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private WeaponItem slot1Weapon;
    [SerializeField] private WeaponItem slot2Weapon;

    [Header("Weapon Visual")]
    [SerializeField] private Transform weaponVisualParent;
    [SerializeField] private Vector3 weaponVisualOffset = new Vector3(1f, 0f, 0f);

    private float nextPrimaryTime;
    private float nextSecondaryTime;
    private GameObject slot1Visual;
    private GameObject slot2Visual;

    public WeaponItem Slot1Weapon => slot1Weapon;
    public WeaponItem Slot2Weapon => slot2Weapon;

    private void Awake()
    {
        if (weaponVisualParent == null)
            weaponVisualParent = transform;

        slot1Visual = CreateWeaponVisual(slot1Weapon);
        slot2Visual = CreateWeaponVisual(slot2Weapon);
    }

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
        ReplaceWeaponVisual(ref slot1Visual, weapon);
    }

    public void EquipSlot2(WeaponItem weapon)
    {
        slot2Weapon = weapon;
        ReplaceWeaponVisual(ref slot2Visual, weapon);
    }

    private void ReplaceWeaponVisual(ref GameObject currentVisual, WeaponItem weapon)
    {
        if (currentVisual != null)
            Destroy(currentVisual);

        currentVisual = CreateWeaponVisual(weapon);
    }

    private GameObject CreateWeaponVisual(WeaponItem weapon)
    {
        if (weapon == null || weapon.VisualPrefab == null)
            return null;

        GameObject visual = Instantiate(weapon.VisualPrefab, weaponVisualParent);
        visual.transform.localPosition = weaponVisualOffset;
        visual.transform.localRotation = Quaternion.identity;

        if (!visual.TryGetComponent<FloatingObject>(out FloatingObject floatingObject))
            floatingObject = visual.AddComponent<FloatingObject>();

        floatingObject.SetStartPosition(weaponVisualOffset);

        return visual;
    }

    public void UsePrimaryAttack()
    {
        if (slot1Weapon == null)
            return;

        if (Time.time < nextPrimaryTime)
            return;

        slot1Weapon.UsePrimary(this, GetAimDirection());

        if (slot1Weapon.RotatesOwnerOnPrimaryAttack)
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
