using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int StorageSlotCount = 6;

    [Header("Shared Storage")]
    [SerializeField] private WeaponItem[] storage = new WeaponItem[StorageSlotCount];

    public WeaponItem GetStorageItem(int index)
    {
        return IsValidStorageIndex(index) ? storage[index] : null;
    }

    public bool TryAddToStorage(WeaponItem item)
    {
        if (item == null)
            return false;

        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i] == null)
            {
                storage[i] = item;
                return true;
            }
        }

        return false;
    }

    public WeaponItem RemoveFromStorage(int index)
    {
        if (!IsValidStorageIndex(index))
            return null;

        WeaponItem item = storage[index];
        storage[index] = null;
        return item;
    }

    public bool TryMoveStorageItemToPlayer(GameObject player, int storageIndex, int equippedSlot)
    {
        if (player == null || !IsValidStorageIndex(storageIndex) || (equippedSlot != 1 && equippedSlot != 2))
            return false;

        PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory == null)
            return false;

        WeaponItem item = storage[storageIndex];
        if (item == null)
            return false;

        WeaponItem equippedItem = equippedSlot == 1 ? playerInventory.Slot1Weapon : playerInventory.Slot2Weapon;
        if (equippedItem != null && !TryAddToStorage(equippedItem))
            return false;

        storage[storageIndex] = null;
        if (equippedSlot == 1)
            playerInventory.EquipSlot1(item);
        else
            playerInventory.EquipSlot2(item);

        return true;
    }

    private void OnValidate()
    {
        if (storage == null || storage.Length != StorageSlotCount)
            System.Array.Resize(ref storage, StorageSlotCount);
    }

    private bool IsValidStorageIndex(int index)
    {
        return index >= 0 && index < storage.Length;
    }
}