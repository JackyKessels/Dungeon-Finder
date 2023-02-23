using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EquipmentSlot
{
    Helmet,
    Armor,
    TwoHand,
    OneHand,
    Shield,
    Relic,
    Necklace,
    Ring,
    Trinket,
    Flask,
    Nothing
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/Equipment")]
[System.Serializable]
public class Equipment : ItemObject
{
    public EquipmentSlot slot;
    public int level = 1;

    [Header("[ General Attributes ]")]
    public int health;
    public int power;
    public int wisdom;
    public int armor;
    public int resistance;
    public int vitality;
    public int speed;
    public int accuracy;
    public int crit;

    [Header("[ School Modifiers ]")]
    public int healingMultiplier;
    public int physicalMultiplier;
    public int fireMultiplier;
    public int iceMultiplier;
    public int natureMultiplier;
    public int arcaneMultiplier;
    public int holyMultiplier;
    public int shadowMultiplier;
    public int critMultiplier;

    [Header("[ Additional ]")]
    public List<PassiveAbility> passives;
    public ActiveAbility useAbility;

    private string positiveAttributes;
    private string negativeAttributes;

    public override string GetDescription(TooltipObject tooltipInfo)
    {
        string itemDescription = base.GetDescription(tooltipInfo) + string.Format("\nLevel: {0}", level) + ParseSlot() + ParseAttributes(tooltipInfo) + GetItemDescription();

        if (slot == EquipmentSlot.Flask)
        {
            if (useAbility != null)
                return itemDescription += useAbility.GetFlaskDescription(tooltipInfo);
            else
                return itemDescription;
        }
        else
        {

            if (passives.Count == 1)
            {
                itemDescription += "\n\nYou learn the following passive ability: " + "\n\n" + passives[0].GetDescription(tooltipInfo);
            }

            if (useAbility != null)
            {
                itemDescription += "\n\nYou learn the following active ability: " + "\n\n" + useAbility.GetDescription(tooltipInfo);
            }

            
        }

        return itemDescription;
    }

    private string ParseSlot()
    {
        if (slot == EquipmentSlot.OneHand)
        {
            return string.Format("\nSlot: One-hand");
        }
        else if (slot == EquipmentSlot.TwoHand)
        {
            return string.Format("\nSlot: Two-hand");
        }
        else
        {
            return string.Format("\nSlot: {0}", slot);
        }
    }

    private string ParseAttributes(TooltipObject tooltipInfo)
    {
        positiveAttributes = "";
        negativeAttributes = "";

        // General stats
        ParseAttribute(health, AttributeType.Health, false);
        ParseAttribute(power, AttributeType.Power, false);
        ParseAttribute(wisdom, AttributeType.Wisdom, false);
        ParseAttribute(armor, AttributeType.Armor, false);
        ParseAttribute(resistance, AttributeType.Resistance, false);
        ParseAttribute(vitality, AttributeType.Vitality, false);
        ParseAttribute(speed, AttributeType.Speed, false);
        ParseAttribute(accuracy, AttributeType.Accuracy, true);
        ParseAttribute(crit, AttributeType.Crit, true);

        // School Multipliers
        ParseAttribute(healingMultiplier, AttributeType.HealingMultiplier, true);
        ParseAttribute(physicalMultiplier, AttributeType.PhysicalMultiplier, true);
        ParseAttribute(fireMultiplier, AttributeType.FireMultiplier, true);
        ParseAttribute(iceMultiplier, AttributeType.IceMultiplier, true);
        ParseAttribute(natureMultiplier, AttributeType.NatureMultiplier, true);
        ParseAttribute(arcaneMultiplier, AttributeType.ArcaneMultiplier, true);
        ParseAttribute(holyMultiplier, AttributeType.HolyMultiplier, true);
        ParseAttribute(shadowMultiplier, AttributeType.ShadowMultiplier, true);
        ParseAttribute(critMultiplier, AttributeType.CritMultiplier, true);

        return positiveAttributes + AddLine() + negativeAttributes;
    }

    private string AddLine()
    {
        if (negativeAttributes.Length > 0)
            return "\n";

        return "";
    }

    private void ParseAttribute(int attributeValue, AttributeType attributeType, bool percentage)
    {
        string plus = "#1FFF00";
        string minus = "#FF0000";

        string percentageSign = percentage ? "%" : "";


        if (attributeValue != 0)
        {
            if (attributeValue > 0)
                positiveAttributes += string.Format("\n<color={1}>+</color> {0}{2} {3}", Math.Abs(attributeValue), plus, percentageSign, GeneralUtilities.GetCorrectAttributeName(attributeType));
            else
                negativeAttributes += string.Format("\n<color={1}>-</color> {0}{2} {3}", Math.Abs(attributeValue), minus, percentageSign, GeneralUtilities.GetCorrectAttributeName(attributeType));
        }
    }

    public override void LeftClick(InteractableItem interactableItem)
    {
        // Do nothing
    }

    public override void RightClick(InteractableItem interactableItem)
    {
        if (interactableItem.equipped)
        {
            if (InventoryManager.Instance.inventoryObject.IsFull())
            {
                Debug.Log("Inventory is full.");
            }
            else
            {
                interactableItem.inventorySlot.UnequipSlot();

                Debug.Log("Unequipping: " + name);

                TooltipHandler.Instance.HideTooltip();
            }
        }
        else
        {
            int slotIndex = GeneralUtilities.GetCorrectEquipmentslot(slot);

            HeroManager.Instance.CurrentHero().equipmentObject.AddEquipment(slotIndex, interactableItem.inventorySlot.item);

            interactableItem.inventorySlot.RemoveItem();

            Debug.Log("Equipping: " + name);

            TooltipHandler.Instance.HideTooltip();
        }
    }
}
