using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine.UI;
using System;

public enum InterfaceType
{
    Inventory,
    Food,
    Equipment,
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Systems/Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public InterfaceType type;
    public Inventory container;
    public InventorySlot[] GetSlots { get { return container.slots; } }

    // Equipment
    public bool dualWield = false;

    public bool IsFull()
    {
        return EmptySlotCount <= 0;
    }

    public bool AddItem(Item _item, int _amount)
    {
        if (_item.id == -1)
        {
            Debug.Log("The item you are trying to add has not yet been added to the item database.");
            return false;
        }

        if (_item == null)
        {
            return false;
        }

        // Find all items that are the same in the inventory
        List<InventorySlot> sameItems = FindAllItems(_item);

        // Full
        if (IsFull())
        {
            // Not stackable
            if (!_item.itemObject.stackable)
            {
                Debug.Log("Inventory is full");
                return false;
            }
            // Stackable
            else
            {
                // For every item that is the same
                foreach (InventorySlot slot in sameItems)
                {
                    // Slot contains less items than the maximum allowed stacks then add 1
                    if (slot.amount < _item.itemObject.maxStacks)
                    {
                        slot.AddAmount(_amount);
                        return true;
                    }
                }

                // Every stackable slot was full and there is no slot left
                Debug.Log("Inventory is full");
                return false;
            }
        }
        // Not full
        else
        {
            // Not stackable
            if (!_item.itemObject.stackable)
            {
                // Get first empty slot and put it in there
                SetFirstEmpty(_item, _amount);
                return true;
            }
            // Stackable
            else
            {
                // For every item that is the same
                foreach (InventorySlot slot in sameItems)
                {
                    // Slot contains less items than the maximum allowed stacks then add 1
                    if (slot.amount < _item.itemObject.maxStacks)
                    {
                        slot.AddAmount(_amount);
                        return true;
                    }
                }

                // If every stackable item is full, then add it to next empty slot
                SetFirstEmpty(_item, _amount);
                return true;
            }
        }
    }

    public void AddEquipment(int _slot, Item _item)
    {
        EquipmentObject equipment = _item.itemObject as EquipmentObject;

        int mainHandSlot = GeneralUtilities.GetCorrectEquipmentslot(EquipmentSlot.TwoHand);
        int offHandSlot = GeneralUtilities.GetCorrectEquipmentslot(EquipmentSlot.Shield);

        if (GeneralUtilities.GetCorrectEquipmentslot(equipment.slot) == _slot)
        {
            // If target slot is empty
            if (GetSlots[_slot].item.id == -1)
            {
                if (equipment.slot == EquipmentSlot.Shield || equipment.slot == EquipmentSlot.Relic)
                {
                    // If main-hand slot blocks the shield, then unequip main-hand
                    if (!GetSlots[_slot].SlotAllows(EquipmentSlot.Shield))
                    {
                        GetSlots[mainHandSlot].UnequipSlot();
                    }

                    GetSlots[_slot].EquipSlot(_item);
                }
                // If equipping a two-hand, then unequip off-hand if there is one
                else if (equipment.slot == EquipmentSlot.TwoHand)
                {
                    // If off-hand is filled
                    if (GetSlots[offHandSlot].item.id != -1)
                        GetSlots[offHandSlot].UnequipSlot();

                    GetSlots[_slot].EquipSlot(_item);
                }
                else
                {
                    GetSlots[_slot].EquipSlot(_item);
                }
            }
            // If target slot is filled
            else
            {
                // If equipping a two-hand, then unequip off-hand if there is one
                if (equipment.slot == EquipmentSlot.TwoHand)
                {
                    GetSlots[_slot].UnequipSlot();

                    // If off-hand is filled
                    if (GetSlots[offHandSlot].item.id != -1)
                        GetSlots[offHandSlot].UnequipSlot();

                    GetSlots[_slot].EquipSlot(_item);
                }
                else if (equipment.slot == EquipmentSlot.OneHand)
                {
                    // If we can dual wield and the off-hand is empty
                    if (dualWield && GetSlots[offHandSlot].item.id == -1 && GetSlots[offHandSlot].SlotAllows(EquipmentSlot.OneHand))
                    {
                        GetSlots[offHandSlot].EquipSlot(_item);
                    }
                    else
                    {
                        GetSlots[_slot].UnequipSlot();
                        GetSlots[_slot].EquipSlot(_item);
                    }
                }
                else
                {
                    GetSlots[_slot].UnequipSlot();
                    GetSlots[_slot].EquipSlot(_item);
                }
            }
        }
        // Load dual wielded weapons
        else if (dualWield && GetSlots[offHandSlot].item.id == -1 && GetSlots[offHandSlot].SlotAllows(EquipmentSlot.OneHand))
        {
            GetSlots[offHandSlot].EquipSlot(_item);
        }
        else
        {
            Debug.Log("BAD SLOT");
            InventoryManager.Instance.inventoryObject.AddItem(_item, 1);
        }
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.id <= -1)
                    counter++;
            }
            return counter;
        }
    }

    public bool ContainsItemObject(ItemObject itemObject)
    {
        bool contains = false;

        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.itemObject == itemObject)
            {
                contains = true;
                break;
            }
        }

        return contains;
    }

    private List<InventorySlot> FindAllItems(Item _item)
    {
        List<InventorySlot> slots = new List<InventorySlot>();

        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.id == _item.id)
            {
                slots.Add(GetSlots[i]);
            }
        }

        return slots;
    }

    private InventorySlot SetFirstEmpty(Item _item, int _amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            // Get first empty slot and add item to it
            if (GetSlots[i].item.id <= -1)
            {
                GetSlots[i].UpdateSlot(_item, _amount);
                return GetSlots[i];
            }
        }

        return null;
    }

    public void RemoveItem(ItemObject itemObject, int amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.itemObject == itemObject)
            {
                int resultingAmount = GetSlots[i].amount - amount;
                if (resultingAmount <= 0)
                {
                    GetSlots[i].RemoveItem();
                }
                else
                {
                    GetSlots[i].UpdateSlot(null, resultingAmount);
                }
            }
        }
    }

    public void SwapItems(InventorySlot itemSlot1, InventorySlot itemSlot2)
    {
        // Not appropriate item type for inventory
        if (itemSlot1.parent.inventoryObject.type != itemSlot2.parent.inventoryObject.type)
            return;

        // Stack items
        InventorySlot.StackSlots(itemSlot2, itemSlot1);

        // Swap items
        if (itemSlot1.CanPlaceInSlot(itemSlot2.ItemObject) && itemSlot2.CanPlaceInSlot(itemSlot1.ItemObject))
        {
            InventorySlot temp = new InventorySlot(itemSlot2.item, itemSlot2.amount);
            itemSlot2.UpdateSlot(itemSlot1.item, itemSlot1.amount);
            itemSlot1.UpdateSlot(temp.item, temp.amount);
        }
    }

    public (int, int, int)[] ConvertInventoryToIDs()
    {
        (int id, int level, int stacks)[] inventoryList = new (int, int, int)[container.slots.Length];

        for (int i = 0; i < container.slots.Length; i++)
        {
            inventoryList[i].id = container.slots[i].item.id;
            inventoryList[i].level = container.slots[i].item.level;
            inventoryList[i].stacks = container.slots[i].amount;
        }

        return inventoryList;
    }

    public void AddItemsToInventory((int id, int level, int amount)[] itemIDs)
    {
        for (int i = 0; i < container.slots.Length; i++)
        {
            if (itemIDs[i].id != -1)
            {
                Item item = Item.CreateItem(database.itemObjects[itemIDs[i].id], itemIDs[i].level);
                container.slots[i].UpdateSlot(item, itemIDs[i].amount);
            }           
        }
    }

    public Item FindFirstMatchingItem(ItemObject itemObject)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].ItemObject == itemObject)
            {
                return GetSlots[i].item;
            }
        }

        return null;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        container.Clear();
    }

    // Equipment Only

    public void ResetSlotRestrictions()
    {
        GetSlots[0].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Helmet };
        GetSlots[1].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Armor };
        GetSlots[2].allowedItems = new List<EquipmentSlot> { EquipmentSlot.TwoHand, EquipmentSlot.OneHand };
        GetSlots[2].restricted = false;
        GetSlots[3].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Shield, EquipmentSlot.Relic };
        GetSlots[3].restricted = false;
        GetSlots[4].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Necklace };
        GetSlots[5].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Ring };
        GetSlots[6].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Trinket };
        GetSlots[7].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Flask };
    }

    public void SetDualWield(bool active)
    {
        dualWield = active;

        if (active)
            GetSlots[3].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Shield, EquipmentSlot.Relic, EquipmentSlot.OneHand };
        else
            GetSlots[3].allowedItems = new List<EquipmentSlot> { EquipmentSlot.Shield, EquipmentSlot.Relic };
    }

    public void RestrictMainHand(bool restrict)
    {
        InventorySlot mainHand = GetSlots[2];

        if (restrict)
        {
            mainHand.allowedItems = new List<EquipmentSlot> { EquipmentSlot.OneHand };
        }
        else
        {
            mainHand.allowedItems = new List<EquipmentSlot> { EquipmentSlot.TwoHand, EquipmentSlot.OneHand };
        }
    }

    public void RestrictOffHand(bool restrict)
    {
        InventorySlot offHand = GetSlots[3];

        if (restrict) // Equip
        {
            offHand.slotDisplay.GetComponent<Image>().sprite = GameAssets.i.restrictedSlot;
            offHand.restricted = true;
            offHand.allowedItems = new List<EquipmentSlot> { EquipmentSlot.Nothing };
        }
        else // Unequip
        {
            offHand.slotDisplay.GetComponent<Image>().sprite = offHand.slotDisplay.GetComponent<EquipmentSlotIcon>().icon;
            offHand.restricted = false;

            if (dualWield)
                offHand.allowedItems = new List<EquipmentSlot> { EquipmentSlot.Shield, EquipmentSlot.Relic, EquipmentSlot.OneHand };
            else
                offHand.allowedItems = new List<EquipmentSlot> { EquipmentSlot.Shield, EquipmentSlot.Relic };
        }
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] slots = new InventorySlot[24];

    public void Clear()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot _slot);

[System.Serializable]
public class InventorySlot
{
    public List<EquipmentSlot> allowedItems = new List<EquipmentSlot>();
    public bool restricted = false;
    [System.NonSerialized] public InventoryInterface parent;
    [System.NonSerialized] public GameObject slotDisplay;
    [System.NonSerialized] public SlotUpdated OnAfterUpdate;
    [System.NonSerialized] public SlotUpdated OnBeforeUpdate;
    public Item item;
    public int amount;

    public bool foodOnly = false;

    public ItemObject ItemObject
    {
        get
        {
            if (item.id >= 0)
            {
                return parent.inventoryObject.database.itemObjects[item.id];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }

    public InventorySlot(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }

    public void UpdateSlot(Item _item, int _amount)
    {
        OnBeforeUpdate?.Invoke(this);

        item = _item;
        amount = _amount;

        OnAfterUpdate?.Invoke(this);
    }

    public void EquipSlot(Item _item)
    {
        item = _item;
        amount = 1;

        OnAfterUpdate?.Invoke(this);
    }

    public void UnequipSlot()
    {
        InventoryManager.Instance.inventoryObject.AddItem(item, 1);

        UpdateSlot(new Item(), 0);
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }

    public void ReduceAmount(int value)
    {
        if (amount - value <= 0)
        {
            RemoveItem();
        }
        else
        {
            UpdateSlot(item, amount -= value);
        }
    }

    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }

    public bool CanPlaceInSlot(ItemObject itemObject)
    {
        if (allowedItems.Count <= 0 || itemObject == null || itemObject.item.id < 0)
            return true;

        if (itemObject is EquipmentObject equip)
        {
            return CorrectSlot(equip);
        }

        return false;
    }

    private bool CorrectSlot(EquipmentObject equip)
    {
        return allowedItems.Contains(equip.slot);
    }

    public bool SlotAllows(EquipmentSlot equipmentSlot)
    {
        return allowedItems.Contains(equipmentSlot);
    }

    public static void StackSlots(InventorySlot startSlot, InventorySlot targetSlot)
    {
        // Dragging empty slot
        if (startSlot.ItemObject == null)
            return;

        // Return if items aren't the same
        if (startSlot.ItemObject != targetSlot.ItemObject)
            return;

        // Return if items can't stack
        if (!startSlot.ItemObject.stackable)
            return;

        // Start values of the slots
        int startAmount = startSlot.amount;
        int targetAmount = targetSlot.amount;

        // How much can maximally be moved from 1 stack to the other
        int transferMax = targetSlot.ItemObject.maxStacks - targetAmount;

        // Return if nothing to stack
        if (transferMax == 0)
            return;

        // New amount of the starting stack
        int startResult = Mathf.Max(startAmount - transferMax, 0);

        // Actual transfered amount
        int transferAmount = startAmount - startResult;

        // Set new values
        startAmount = startResult;
        targetAmount += transferAmount;
        startSlot.amount = startResult;
        targetSlot.amount = targetAmount;

        // Remove the item if it completely transfers over
        if (startAmount == 0)
            startSlot.RemoveItem();
    }
}
