using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private WeaponBehaviour slot1Weapon;
    [SerializeField] private WeaponBehaviour slot2Weapon;

    [Header("Visual Offsets")]
    [SerializeField] private Vector3 slot1Offset = new Vector3(-0.75f, 0.1f, 0f);
    [SerializeField] private Vector3 slot2Offset = new Vector3(0.75f, 0.1f, 0f);

    private const float GlobalAttackCooldown = 0.5f;

    private float nextPrimaryTime;
    private float nextSecondaryTime;
    private float nextGlobalAttackTime;
    private GameObject slot1Visual;
    private GameObject slot2Visual;

    [Header("Staff Attack Animation")]
    [SerializeField] private float staffLiftAmount = 0.35f;
    [SerializeField] private float staffRotationAngle = 35f;
    [SerializeField] private float staffAnimationDuration = 0.18f;

    public WeaponBehaviour Slot1Weapon => slot1Weapon;
    public WeaponBehaviour Slot2Weapon => slot2Weapon;

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

    public void EquipSlot1(WeaponBehaviour weapon)
    {
        slot1Weapon = weapon;
        RefreshVisuals();
    }

    public void EquipSlot2(WeaponBehaviour weapon)
    {
        slot2Weapon = weapon;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (slot1Weapon != null)
        {
            if (slot1Visual != null)
                Destroy(slot1Visual);

            slot1Visual = CreateWeaponVisual(slot1Weapon, slot1Offset);
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

            slot2Visual = CreateWeaponVisual(slot2Weapon, slot2Offset);
        }
        else if (slot2Visual != null)
        {
            Destroy(slot2Visual);
            slot2Visual = null;
        }
    }

    private GameObject CreateWeaponVisual(WeaponBehaviour weapon, Vector3 offset)
    {
        if (weapon == null)
            return null;

        GameObject visual = Instantiate(weapon.gameObject);

        visual.transform.SetParent(transform, false);

        if (weapon.IsSword)
        {
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
        if (slot1Weapon.IsStaff)
            StartCoroutine(AnimateStaffAttack(slot1Visual, -staffRotationAngle));

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
        if (slot2Weapon.IsStaff)
            StartCoroutine(AnimateStaffAttack(slot2Visual, staffRotationAngle));
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

    private System.Collections.IEnumerator AnimateStaffAttack(GameObject staffVisual, float rotationAngle)
    {
        if (staffVisual == null)
            yield break;

        Vector3 startPosition = staffVisual.transform.localPosition;
        Quaternion startRotation = staffVisual.transform.localRotation;
        Vector3 liftedPosition = startPosition + Vector3.up * staffLiftAmount;
        Quaternion liftedRotation = Quaternion.Euler(0f, 0f, rotationAngle) * startRotation;
        float halfDuration = Mathf.Max(0.01f, staffAnimationDuration * 0.5f);

        for (float elapsed = 0f; elapsed < halfDuration; elapsed += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            staffVisual.transform.localPosition = Vector3.Lerp(startPosition, liftedPosition, progress);
            staffVisual.transform.localRotation = Quaternion.Slerp(startRotation, liftedRotation, progress);
            yield return null;
        }

        for (float elapsed = 0f; elapsed < halfDuration; elapsed += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            staffVisual.transform.localPosition = Vector3.Lerp(liftedPosition, startPosition, progress);
            staffVisual.transform.localRotation = Quaternion.Slerp(liftedRotation, startRotation, progress);
            yield return null;
        }

        staffVisual.transform.localPosition = startPosition;
        staffVisual.transform.localRotation = startRotation;
    }
}
