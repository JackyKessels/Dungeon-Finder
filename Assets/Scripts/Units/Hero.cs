using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HeroClass
{
    Templar,
    Invoker,
    Shaman,
    None
}

public class Hero : Unit
{
    [HideInInspector] public int heroObjectIndex;

    [HideInInspector] public HeroObject heroObject;
    [HideInInspector] public HeroClass heroClass;

    [HideInInspector] public InventoryObject equipmentObject;

    public HeroPathManager heroPathManager;

    //public List<Specialization> specializations = new List<Specialization>();
    //public Specialization currentSpecialization;

    public (int, int, int)[] itemIDs;

    public List<WeaponRequirement> equippedWeapons = new List<WeaponRequirement>();

    [HideInInspector] public int teamNumber;

    public bool newHero = true;

    public void Setup(int heroIndex, int index, (int, int, int)[] loadItemIDs = null)
    {
        this.heroObjectIndex = heroIndex;
        teamNumber = index;

        // Set correct data from the UnitObject
        heroObject = TeamManager.Instance.heroObjects[this.heroObjectIndex];
        name = heroObject.name;
        unitType = heroObject.unitType;
        icon = heroObject.icon;
        sprite = heroObject.sprite;
        unitRenderer.sprite = heroObject.sprite;
        heroClass = heroObject.heroClass;
        isEnemy = false;

        // Create the managers
        statsManager = new StatsManager(this, heroObject);
        heroPathManager = new HeroPathManager(this);

        // Spellbook
        spellbook = new Spellbook(this);
        foreach (AbilityObject abilityObject in heroObject.startingAbilities)
        {
            spellbook.LearnAbility(new Active(abilityObject as ActiveAbility, 1));
        }  
        spellbook.SetActiveSpellbook();

        // Equipment
        equippedWeapons.Add(WeaponRequirement.Nothing);

        equipmentObject = InventoryManager.Instance.equipmentObjects[index];
        if (equipmentObject != null)
        {
            for (int i = 0; i < equipmentObject.GetSlots.Length; i++)
            {
                equipmentObject.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
                equipmentObject.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
            }

            equipmentObject.SetDualWield(heroObject.dualWield);
        }

        if (loadItemIDs == null)
        {
            EquipStartingEquipment();
        }
        else
        {
            // Load items
            itemIDs = loadItemIDs;
        }

        InventoryManager.Instance.Setup(this);

        SetupEquipment(itemIDs, statsManager.currentHealth);

        if (newHero)
            RestoreHealth();
        else
            newHero = false;
    }

    private void EquipStartingEquipment()
    {
        itemIDs = new (int, int, int)[8] 
        {
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1), 
            (-1, 1, 1)
        };

        for (int i = 0; i < heroObject.startingEquipment.Count; i++)
        {
            int slot = GeneralUtilities.GetCorrectEquipmentslot(heroObject.startingEquipment[i].slot);
            itemIDs[slot] = (heroObject.startingEquipment[i].item.id, 1, 1);
        }
    }

    public override UnitObject GetUnitObject()
    {
        return heroObject;
    }

    public Item GetFlask()
    {
        if (equipmentObject.container.slots[7].item.id < 0)
        {
            return null;
        }
        else
        {
            return equipmentObject.container.slots[7].item;
        }
    }

    public void SetupEquipment((int id, int level, int amount)[] itemIDs, int currentHealth = -1)
    {
        for (int i = 0; i < itemIDs.Length; i++)
        {
            if (itemIDs[i].id != -1)
            {
                Item item = Item.CreateItem(DatabaseHandler.Instance.itemDatabase.itemObjects[itemIDs[i].id], itemIDs[i].level);
                equipmentObject.AddEquipment(i, item);
            }
        }

        // No specified current Health resets it to full
        if (currentHealth == -1)
            RestoreHealth();
        else
            statsManager.currentHealth = currentHealth;
    }

    public void ForceEquipItem(Equipment equipment)
    {
        int slot = GeneralUtilities.GetCorrectEquipmentslot(equipment.equipmentObject.slot);

        equipmentObject.AddEquipment(slot, equipment);
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        UnequipItem(_slot);
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        EquipItem(_slot);
    }

    public void EquipItem(InventorySlot slot)
    {
        if (slot.item is not Equipment)
        {
            return;
        }

        Equipment equipment = slot.item as Equipment;

        // Equip one-hand
        if (equipment.equipmentObject.slot == EquipmentSlot.OneHand)
        {
            equippedWeapons.Add(WeaponRequirement.OneHand);

            if (heroObject.dualWield && slot.SlotAllows(EquipmentSlot.Shield))
                equipmentObject.RestrictMainHand(true);
        }

        // Equip a two-hand
        if (equipment.equipmentObject.slot == EquipmentSlot.TwoHand)
        {
            equippedWeapons.Add(WeaponRequirement.TwoHand);

            equipmentObject.RestrictOffHand(true);
        }

        // Equip a shield
        if (equipment.equipmentObject.slot == EquipmentSlot.Shield)
        {
            equippedWeapons.Add(WeaponRequirement.Shield);

            equipmentObject.RestrictMainHand(true);
        }

        // Equip a relic
        if (equipment.equipmentObject.slot == EquipmentSlot.Relic)
        {
            equippedWeapons.Add(WeaponRequirement.Relic);

            equipmentObject.RestrictMainHand(true);
        }

        // Add passive bonuses
        if (equipment.equipmentObject.passives.Count > 0)
        {
            for (int i = 0; i < equipment.equipmentObject.passives.Count; i++)
            {
                Passive equipmentPassive = new Passive(equipment.equipmentObject.passives[i], 1);

                equipmentPassive.ActivatePassive(this);
            }
        }

        // Items with abilities
        if (equipment.equipmentObject.useAbility != null)
        {
            if (equipment.equipmentObject.slot == EquipmentSlot.Flask)
            {
                spellbook.flaskAbility = new Active(equipment.equipmentObject.useAbility, 1);
                spellbook.flaskAbility.Initialize();
            }
            else
            {
                Active useAbility = new Active(equipment.equipmentObject.useAbility, 1);
                spellbook.LearnAbility(useAbility, true);
            }
        }

        // Add value to Attribute
        for (int i = 0; i < equipment.attributes.Count; i++)
        {
            if (equipment.attributes[i].baseValue != 0)
            {
                statsManager.ModifyAttribute(equipment.attributes[i].attributeType, AttributeValue.bonusValue, equipment.attributes[i].baseValue);
            }
        }

        SpellbookManager.Instance.Setup(this);
        InventoryManager.Instance.UpdateCharacterAttributes(this, -1);
    }

    public void UnequipItem(InventorySlot slot)
    {
        if (slot.item is not Equipment)
        {
            return;
        }

        Equipment equipment = slot.item as Equipment;

        // Unequip one-hand
        if (equipment.equipmentObject.slot == EquipmentSlot.OneHand)
        {
            equippedWeapons.Remove(WeaponRequirement.OneHand);

            if (heroObject.dualWield && slot.SlotAllows(EquipmentSlot.Shield))
                equipmentObject.RestrictMainHand(false);
        }

        // Unequip a two-hand
        if (equipment.equipmentObject.slot == EquipmentSlot.TwoHand)
        {
            equippedWeapons.Remove(WeaponRequirement.TwoHand);

            equipmentObject.RestrictOffHand(false);
        }

        // Unequip a shield
        if (equipment.equipmentObject.slot == EquipmentSlot.Shield)
        {
            equippedWeapons.Remove(WeaponRequirement.Shield);

            equipmentObject.RestrictMainHand(false);
        }

        // Unequip a relic
        if (equipment.equipmentObject.slot == EquipmentSlot.Relic)
        {
            equippedWeapons.Remove(WeaponRequirement.Relic);

            equipmentObject.RestrictMainHand(false);
        }

        // Remove passive bonuses
        if (equipment.equipmentObject.passives.Count > 0)
        {
            for (int i = 0; i < equipment.equipmentObject.passives.Count; i++)
            {
                Passive equipmentPassive = new Passive(equipment.equipmentObject.passives[i], 1);

                equipmentPassive.DeactivatePassive(this);
            }
        }

        // Items with abilities
        if (equipment.equipmentObject.useAbility != null)
        {
            if (equipment.equipmentObject.slot == EquipmentSlot.Flask)
            {
                spellbook.flaskAbility = new Active();
            }
            else
            {
                spellbook.UnlearnAbility(spellbook.FindItemAbility(equipment.equipmentObject.useAbility), true);
            }
        }

        // Remove value from Attribute
        for (int i = 0; i < equipment.attributes.Count; i++)
        {
            if (equipment.attributes[i].baseValue != 0)
            {
                statsManager.ModifyAttribute(equipment.attributes[i].attributeType, AttributeValue.bonusValue, equipment.attributes[i].baseValue * -1);
            }
        }

        SpellbookManager.Instance.Setup(this);
        InventoryManager.Instance.UpdateCharacterAttributes(this, -1);
    }
}
