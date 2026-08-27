using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Attack Power")]
    [SerializeField] private int physicalAttackPower = 10;
    [SerializeField] private int magicalAttackPower = 10;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float MovementSpeed => movementSpeed;
    public int PhysicalAttackPower => physicalAttackPower;
    public int MagicalAttackPower => magicalAttackPower;

    public bool TryHeal(int amount)
    {
        if (currentHealth >= maxHealth)
            return false;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        return true;
    }

    public bool TryDamage(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        return currentHealth < previousHealth;
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        movementSpeed = Mathf.Max(0f, movementSpeed);
        physicalAttackPower = Mathf.Max(0, physicalAttackPower);
        magicalAttackPower = Mathf.Max(0, magicalAttackPower);
    }
}