using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private WeaponItem slot1Weapon;
    [SerializeField] private WeaponItem slot2Weapon;

    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth = 10;

    [Header("Shield")]
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private Vector2 shieldOffset = new Vector2(0.9f, 0f);

    private float nextPrimaryTime;
    private float nextSecondaryTime;
    private GameObject activeShield;

    public WeaponItem Slot1Weapon => slot1Weapon;
    public WeaponItem Slot2Weapon => slot2Weapon;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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

    public void ActivateShield(Vector2 direction)
    {
        if (shieldPrefab == null)
            return;

        if (activeShield != null)
            Destroy(activeShield);

        Vector3 offset = (Vector3)(direction.normalized * shieldOffset.magnitude);
        activeShield = Instantiate(shieldPrefab, transform.position + offset, Quaternion.identity);
        activeShield.transform.SetParent(transform);
        activeShield.transform.localScale = Vector3.one;
    }

    public bool TryHeal(int amount)
    {
        if (currentHealth >= maxHealth)
            return false;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        return true;
    }

    public bool TryDamage(int amount)
    {
        if (activeShield != null)
        {
            Destroy(activeShield);
            activeShield = null;
            return false;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        return true;
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
