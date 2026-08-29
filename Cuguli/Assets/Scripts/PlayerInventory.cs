using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private WeaponItem slot1Weapon;
    [SerializeField] private WeaponItem slot2Weapon;

    [Header("Visual Holders")]
    [SerializeField] private Transform slot1Holder;
    [SerializeField] private Transform slot2Holder;

    [Header("Visual Offsets")]
    [SerializeField] private Vector3 slot1Offset = new Vector3(-0.75f, 0.1f, 0f);
    [SerializeField] private Vector3 slot2Offset = new Vector3(0.75f, 0.1f, 0f);

    private const float GlobalAttackCooldown = 0.5f;

    private float nextPrimaryTime;
    private float nextSecondaryTime;
    private float nextGlobalAttackTime;
    private GameObject slot1Visual;
    private GameObject slot2Visual;

    public WeaponItem Slot1Weapon => slot1Weapon;
    public WeaponItem Slot2Weapon => slot2Weapon;

    private void Awake()
    {
        RefreshVisuals();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            UsePrimaryAttack();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
            UseSecondaryAttack();
    }

    public void EquipSlot1(WeaponItem weapon)
    {
        slot1Weapon = weapon;
        RefreshVisuals();
    }

    public void EquipSlot2(WeaponItem weapon)
    {
        slot2Weapon = weapon;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (slot1Holder == null)
            return;

        if (slot2Holder == null)
            return;

        if (slot1Weapon != null)
        {
            if (slot1Visual != null)
                Destroy(slot1Visual);

            slot1Visual = CreateWeaponVisual(slot1Weapon, slot1Holder, slot1Offset);
        }
        else if (slot1Visual != null)
        {
            Destroy(slot1Visual);
            slot1Visual = null;
        }

        if (slot2Weapon != null)
        {
            if (slot2Visual != null)
                Destroy(slot2Visual);

            slot2Visual = CreateWeaponVisual(slot2Weapon, slot2Holder, slot2Offset);
        }
        else if (slot2Visual != null)
        {
            Destroy(slot2Visual);
            slot2Visual = null;
        }
    }

    private GameObject CreateWeaponVisual(WeaponItem weapon, Transform holder, Vector3 offset)
    {
        if (weapon == null || weapon.VisualPrefab == null)
            return null;

        GameObject visual = Instantiate(weapon.VisualPrefab, holder);

        if (weapon.IsSword)
        {
            visual.transform.SetParent(transform, false);
            visual.transform.localPosition = Vector3.zero;
            var orbit = visual.GetComponent<WeaponOrbitVisual>();
            if (orbit == null)
                orbit = visual.AddComponent<WeaponOrbitVisual>();

            orbit.Initialize(transform, 0.9f, -260f, -90f, new Vector3(0f, 0.35f, 0f));
            return visual;
        }

        visual.transform.localPosition = offset;
        visual.transform.localRotation = Quaternion.identity;

        if (!visual.TryGetComponent<FloatingObject>(out FloatingObject floatingObject))
            floatingObject = visual.AddComponent<FloatingObject>();

        floatingObject.SetStartPosition(offset);

        return visual;
    }

    public void UsePrimaryAttack()
    {
        if (slot1Weapon == null)
            return;

        float cooldown = Mathf.Max(GlobalAttackCooldown, slot1Weapon.Cooldown);
        if (Time.time < Mathf.Max(nextPrimaryTime, nextGlobalAttackTime))
            return;

        slot1Weapon.UsePrimary(this, GetAimDirection());

        if (slot1Weapon.RotatesOwnerOnPrimaryAttack)
            transform.Rotate(0f, 0f, 90f);

        nextPrimaryTime = Time.time + slot1Weapon.Cooldown;
        nextGlobalAttackTime = Time.time + cooldown;
    }

    public void UseSecondaryAttack()
    {
        if (slot2Weapon == null)
            return;

        float cooldown = Mathf.Max(GlobalAttackCooldown, slot2Weapon.Cooldown);
        if (Time.time < Mathf.Max(nextSecondaryTime, nextGlobalAttackTime))
            return;

        slot2Weapon.UseSecondary(this, GetAimDirection());
        nextSecondaryTime = Time.time + slot2Weapon.Cooldown;
        nextGlobalAttackTime = Time.time + cooldown;
    }

    public Vector2 GetMouseWorldPosition()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return transform.position;

        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }

    private Vector2 GetAimDirection()
    {
        Vector2 mouseWorldPosition = GetMouseWorldPosition();
        Vector2 direction = mouseWorldPosition - (Vector2)transform.position;
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
    }
}
