using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

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

    [NonSerialized]
    public bool IsSwappingEquipment = false;

    // Equipment
    public bool dualWield = false;

    public bool IsFull()
    {
        return EmptySlotCount <= 0;
    }

    public InventoryAddResult AddItem(Item _item, int _amount)
    {
        var result = new InventoryAddResult
        {
            requested = Mathf.Max(0, _amount),
            added = 0
        };

        if (_item == null || _item.itemObject == null || _item.id == -1 || _amount <= 0) {
            return result;
        }

        var itemObject = _item.itemObject;
        var remaining = _amount;

        if (itemObject.stackable)
        {
            foreach (var slot in GetSlots)
            {
                if (slot.ItemObject != itemObject)
                {
                    continue;
                }

                var slotsAvailable = itemObject.maxStacks - slot.amount;
                if (slotsAvailable <= 0)
                {
                    continue;
                }

                var amountToAdd = Mathf.Min(remaining, slotsAvailable);

                slot.AddAmount(amountToAdd);
                remaining -= amountToAdd;

                if (remaining == 0)
                {
                    break;
                }
            }
        }
        
        if (remaining > 0)
        {
            foreach (var slot in GetSlots)
            {
                if (slot.ItemObject != null)
                {
                    continue;
                }

                var amountToAdd = itemObject.stackable ? Mathf.Min(remaining, itemObject.maxStacks) : 1;
               
                slot.UpdateSlot(_item, amountToAdd);
                remaining -= amountToAdd;

                if (remaining == 0)
                {
                    break;
                }
            }
        }

        result.added = _amount - remaining;
        return result;
    }

    public bool AddEquipment(int slot, Item item)
    {
        return AddEquipmentBase(slot, item, null);
    }

    public bool AddEquipment(int slot, InventorySlot sourceSlot)
    {
        if (sourceSlot == null || sourceSlot.item == null)
            return false;

        return AddEquipmentBase(slot, sourceSlot.item, sourceSlot);
    }

    private bool AddEquipmentBase(int slot, Item item, InventorySlot sourceSlot)
    {
        if (item == null || item.itemObject is not EquipmentObject equipment)
        {
            return false;
        }

        int mainHandSlot = GeneralUtilities.GetCorrectEquipmentslot(EquipmentSlot.TwoHand);
        int offHandSlot = GeneralUtilities.GetCorrectEquipmentslot(EquipmentSlot.Shield);

        int targetSlot = slot;
        var displacedSlots = new List<int>();

        // Resolve the target slot and equipment that must be displaced.
        if (GeneralUtilities.GetCorrectEquipmentslot(equipment.slot) == slot)
        {
            if (!GetSlots[slot].IsEmpty)
            {
                if (equipment.slot == EquipmentSlot.TwoHand)
                {
                    displacedSlots.Add(slot);

                    if (!GetSlots[offHandSlot].IsEmpty &&
                        !displacedSlots.Contains(offHandSlot))
                    {
                        displacedSlots.Add(offHandSlot);
                    }
                }
                else if (equipment.slot == EquipmentSlot.OneHand &&
                         dualWield &&
                         GetSlots[offHandSlot].IsEmpty &&
                         GetSlots[offHandSlot].SlotAllows(EquipmentSlot.OneHand))
                {
                    targetSlot = offHandSlot;
                }
                else
                {
                    displacedSlots.Add(slot);
                }
            }
            else
            {
                if (equipment.slot == EquipmentSlot.TwoHand &&
                    !GetSlots[offHandSlot].IsEmpty)
                {
                    displacedSlots.Add(offHandSlot);
                }
                else if ((equipment.slot == EquipmentSlot.Shield ||
                          equipment.slot == EquipmentSlot.Relic) &&
                         !GetSlots[slot].SlotAllows(EquipmentSlot.Shield))
                {
                    if (!GetSlots[mainHandSlot].IsEmpty)
                    {
                        displacedSlots.Add(mainHandSlot);
                    }
                }
            }
        }
        // Preserve the existing dual-wield fallback.
        else if (dualWield &&
                 GetSlots[offHandSlot].IsEmpty &&
                 GetSlots[offHandSlot].SlotAllows(EquipmentSlot.OneHand))
        {
            targetSlot = offHandSlot;
        }
        else
        {
            return false;
        }

        // Legacy callers: preserve the existing unequip/equip flow.
        if (sourceSlot == null)
        {
            foreach (int displacedSlot in displacedSlots)
            {
                if (!GetSlots[displacedSlot].UnequipSlot())
                {
                    return false;
                }
            }

            GetSlots[targetSlot].EquipSlot(item);
            return true;
        }

        // Check inventory capacity before making changes.
        var inventory = InventoryManager.Instance.inventoryObject;
        var freeSlots = new List<InventorySlot>();

        foreach (var inventorySlot in inventory.GetSlots)
        {
            if (inventorySlot != sourceSlot && inventorySlot.IsEmpty)
            {
                freeSlots.Add(inventorySlot);
            }
        }

        // The source slot receives one displaced item.
        int extraSlotsNeeded = Mathf.Max(0, displacedSlots.Count - 1);

        if (freeSlots.Count < extraSlotsNeeded)
        {
            return false;
        }

        // Snapshot both incoming and displaced items before updating slots.
        Item incomingItem = sourceSlot.item;
        var displacedItems = new List<Item>();

        foreach (int displacedSlot in displacedSlots)
        {
            displacedItems.Add(GetSlots[displacedSlot].item);
        }

        // Update all equipment slots before refreshing the spellbook.
        IsSwappingEquipment = true;

        try
        {
            // Remove displaced equipment and its effects.
            foreach (int displacedSlot in displacedSlots)
            {
                GetSlots[displacedSlot].UpdateSlot(new Item(), 0);
            }

            // Equip incoming item and apply its effects.
            GetSlots[targetSlot].UpdateSlot(incomingItem, 1);

            // Move displaced equipment into the inventory.
            if (displacedItems.Count > 0)
            {
                sourceSlot.UpdateSlot(displacedItems[0], 1);

                for (int i = 1; i < displacedItems.Count; i++)
                {
                    freeSlots[i - 1].UpdateSlot(displacedItems[i], 1);
                }
            }
            else
            {
                sourceSlot.UpdateSlot(new Item(), 0);
            }
        }
        finally
        {
            IsSwappingEquipment = false;
            SpellbookManager.Instance.Setup(HeroManager.Instance.CurrentHero());
        }

        return true;
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

    public void RemoveItem(ItemObject itemObject, int amount)
    {
        if (itemObject == null || amount <= 0)
        {
            return;
        }

        for (int i = 0; i < GetSlots.Length && amount > 0; i++)
        {
            var slot = GetSlots[i];

            if (slot.ItemObject != itemObject)
            {
                continue;
            }

            var amountToRemove = Mathf.Min(slot.amount, amount);
            slot.ReduceAmount(amountToRemove);
            amount -= amountToRemove;
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
    public InventorySlot[] slots = new InventorySlot[25];

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

    public bool IsEmpty => item.id == -1;

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

    public bool UnequipSlot()
    {
        InventoryAddResult result = InventoryManager.Instance.inventoryObject.AddItem(item, 1);

        if (!result.Complete)
        {
            return false;
        }

        UpdateSlot(new Item(), 0);
        return true;
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
