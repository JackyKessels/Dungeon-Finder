using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    private readonly float healthWeight = 3f;
    private readonly float powerWeight = 1f;
    private readonly float wisdomWeight = 1f;

    public List<Attribute> attributes;
    public EquipmentObject equipmentObject;

    private string positiveAttributes;
    private string negativeAttributes;

    public Equipment(EquipmentObject equipmentObject, int level)
    {
        name = equipmentObject.name;
        id = equipmentObject.item.id;
        itemObject = equipmentObject;
        this.equipmentObject = equipmentObject;
        this.level = level;

        attributes = new List<Attribute>();

        float totalValue = TotalValue(equipmentObject.slot, equipmentObject.quality);

        foreach (AttributeType type in (AttributeType[])Enum.GetValues(typeof(AttributeType)))
        {
            attributes.Add(new Attribute(type));
        }

        foreach (Attribute a in attributes)
        {
            switch (a.attributeType)
            {
                // General Attributes
                case AttributeType.Health:
                    a.baseValue = GeneralUtilities.RoundFloat(equipmentObject.healthFactor * healthWeight * totalValue, 0);
                    break;
                case AttributeType.Power:
                    a.baseValue = GeneralUtilities.RoundFloat(equipmentObject.powerFactor * powerWeight * totalValue, 0);
                    break;
                case AttributeType.Wisdom:
                    a.baseValue = GeneralUtilities.RoundFloat(equipmentObject.wisdomFactor * wisdomWeight * totalValue, 0);
                    break;
                case AttributeType.Armor:
                    a.baseValue = equipmentObject.armor;
                    break;
                case AttributeType.Resistance:
                    a.baseValue = equipmentObject.resistance;
                    break;
                case AttributeType.Vitality:
                    a.baseValue = equipmentObject.vitality;
                    break;
                case AttributeType.Speed:
                    a.baseValue = equipmentObject.speed;
                    break;
                case AttributeType.Accuracy:
                    a.baseValue = equipmentObject.accuracy;
                    break;
                case AttributeType.Crit:
                    a.baseValue = equipmentObject.crit;
                    break;

                // School Multipliers
                case AttributeType.HealingMultiplier:
                    a.baseValue = equipmentObject.healingMultiplier;
                    break;
                case AttributeType.PhysicalMultiplier:
                    a.baseValue = equipmentObject.physicalMultiplier;
                    break;
                case AttributeType.FireMultiplier:
                    a.baseValue = equipmentObject.fireMultiplier;
                    break;
                case AttributeType.IceMultiplier:
                    a.baseValue = equipmentObject.iceMultiplier;
                    break;
                case AttributeType.NatureMultiplier:
                    a.baseValue = equipmentObject.natureMultiplier;
                    break;
                case AttributeType.ArcaneMultiplier:
                    a.baseValue = equipmentObject.arcaneMultiplier;
                    break;
                case AttributeType.HolyMultiplier:
                    a.baseValue = equipmentObject.holyMultiplier;
                    break;
                case AttributeType.ShadowMultiplier:
                    a.baseValue = equipmentObject.shadowMultiplier;
                    break;
                case AttributeType.CritMultiplier:
                    a.baseValue = equipmentObject.critMultiplier;
                    break;
                case AttributeType.SacrificialMultiplier:
                    a.baseValue = equipmentObject.sacrificialMultiplier;
                    break;
            }
        }
    }

    private float TotalValue(EquipmentSlot slot, Quality quality)
    {
        return (level + 1) * GetSlotValue(slot) * GetQualityValue(quality);
    }

    private float GetSlotValue(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Helmet:
                return 1.50f;
            case EquipmentSlot.Armor:
                return 2.00f;
            case EquipmentSlot.Necklace:
                return 1.00f;
            case EquipmentSlot.Ring:
                return 1.00f;
            case EquipmentSlot.Trinket:
                return 1.00f;
            case EquipmentSlot.Flask:
                return 1.25f;
            case EquipmentSlot.TwoHand:
                return 2.00f;
            case EquipmentSlot.OneHand:
                return 1.00f;
            case EquipmentSlot.Shield:
                return 1.00f;
            case EquipmentSlot.Relic:
                return 1.00f;
            default:
                return 1.00f;
        }
    }

    private float GetQualityValue(Quality quality)
    {
        switch (quality)
        {
            case Quality.Common:
                return 1.00f;
            case Quality.Mystical:
                return 1.25f;
            case Quality.Legendary:
                return 1.50f;
            default:
                return 1.00f;
        }
    }

    public override string GetDescription(TooltipObject tooltipInfo)
    {
        string tooltip = base.GetDescription(tooltipInfo) + 
                         string.Format("\nLevel: {0}", level) + 
                         ParseSlot() + 
                         ParseAttributes() + 
                         GetItemDescription();

        if (equipmentObject.slot == EquipmentSlot.Flask)
        {
            if (equipmentObject.useAbility != null)
                return tooltip += equipmentObject.useAbility.GetFlaskDescription(tooltipInfo);
            else
                return tooltip;
        }
        else
        {
            if (equipmentObject.passives.Count >= 1)
            {
                int count = equipmentObject.passives.Count;

                if (count == 1)
                {
                    tooltip += "\n\nYou learn the following passive ability: " + "\n\n" + equipmentObject.passives[0].GetEquipmentDescription(tooltipInfo);
                }
                else
                {
                    tooltip += "\n\nYou learn the following passive abilities: ";

                    foreach (PassiveAbility passiveAbility in equipmentObject.passives)
                    {
                        tooltip += "\n\n" + passiveAbility.GetEquipmentDescription(tooltipInfo);
                    }
                }
            }

            if (equipmentObject.useAbility != null)
            {
                tooltip += "\n\nYou learn the following active ability: " + "\n\n" + equipmentObject.useAbility.GetDescription(tooltipInfo);
            }


        }

        return tooltip;
    }

    private string ParseSlot()
    {
        if (equipmentObject.slot == EquipmentSlot.OneHand)
        {
            return string.Format("\nSlot: One-hand");
        }
        else if (equipmentObject.slot == EquipmentSlot.TwoHand)
        {
            return string.Format("\nSlot: Two-hand");
        }
        else
        {
            return string.Format("\nSlot: {0}", equipmentObject.slot);
        }
    }

    private string ParseAttributes()
    {
        positiveAttributes = "";
        negativeAttributes = "";

        // General stats
        ParseAttribute(AttributeType.Health, false);
        ParseAttribute(AttributeType.Power, false);
        ParseAttribute(AttributeType.Wisdom, false);
        ParseAttribute(AttributeType.Armor, false);
        ParseAttribute(AttributeType.Resistance, false);
        ParseAttribute(AttributeType.Vitality, false);
        ParseAttribute(AttributeType.Speed, false);
        ParseAttribute(AttributeType.Accuracy, true);
        ParseAttribute(AttributeType.Crit, true);

        // School Multipliers
        ParseAttribute(AttributeType.HealingMultiplier, true);
        ParseAttribute(AttributeType.PhysicalMultiplier, true);
        ParseAttribute(AttributeType.FireMultiplier, true);
        ParseAttribute(AttributeType.IceMultiplier, true);
        ParseAttribute(AttributeType.NatureMultiplier, true);
        ParseAttribute(AttributeType.ArcaneMultiplier, true);
        ParseAttribute(AttributeType.HolyMultiplier, true);
        ParseAttribute(AttributeType.ShadowMultiplier, true);
        ParseAttribute(AttributeType.CritMultiplier, true);
        ParseAttribute(AttributeType.SacrificialMultiplier, true);

        return positiveAttributes + AddLine() + negativeAttributes;
    }

    private string AddLine()
    {
        if (negativeAttributes.Length > 0)
            return "\n";

        return "";
    }

    private void ParseAttribute(AttributeType attributeType, bool percentage)
    {
        string positive;
        string negative;

        if (attributeType == AttributeType.SacrificialMultiplier)
        {
            positive = ColorDatabase.Negative;
            negative = ColorDatabase.Positive;
        }
        else
        {
            positive = ColorDatabase.Positive;
            negative = ColorDatabase.Negative;
        }

        string percentageSign = percentage ? "%" : "";

        int attributeValue = attributes[(int)attributeType].baseValue;

        if (attributeValue != 0)
        {
            if (attributeValue > 0)
                positiveAttributes += $"\n<color={positive}>+</color> {Math.Abs(attributeValue)}{percentageSign} {GeneralUtilities.GetCorrectAttributeName(attributeType)}";
            else
                negativeAttributes += $"\n<color={negative}>-</color> {Math.Abs(attributeValue)}{percentageSign} {GeneralUtilities.GetCorrectAttributeName(attributeType)}";
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
            int slotIndex = GeneralUtilities.GetCorrectEquipmentslot(equipmentObject.slot);

            HeroManager.Instance.CurrentHero().equipmentObject.AddEquipment(slotIndex, interactableItem.inventorySlot.item);

            interactableItem.inventorySlot.RemoveItem();

            Debug.Log("Equipping: " + name);

            TooltipHandler.Instance.HideTooltip();
        }
    }
}
