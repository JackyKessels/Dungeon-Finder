using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CharacterAttribute : MonoBehaviour, IDescribable
{
    public Attribute attribute;
    public int value = 0;

    private new string name;
    public Image image;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI valueText;

    public void Setup(Attribute a, Unit u)
    {
        attribute = a;
        name = GeneralUtilities.GetCorrectAttributeName(a.attributeType);
        image.sprite = GeneralUtilities.GetAttributeIcon(a.attributeType);
        typeText.text = name;
        value = a.GetTotalValue();

        if (a.attributeType == AttributeType.Health)
        {
            valueText.text = u.statsManager.currentHealth + " / " + value.ToString();
        }
        else if (a.attributeType == AttributeType.Power || a.attributeType == AttributeType.Wisdom)
        {
            if (value <= 0)
            {
                valueText.text = "0";
            }
            else
            {
                valueText.text = value.ToString();
            }
        }
        else if ((a.attributeType == AttributeType.Accuracy)            ||
                 (a.attributeType == AttributeType.Crit)                ||
                 (a.attributeType == AttributeType.HealingMultiplier)   ||
                 (a.attributeType == AttributeType.PhysicalMultiplier)  ||
                 (a.attributeType == AttributeType.FireMultiplier)      ||
                 (a.attributeType == AttributeType.IceMultiplier)       ||
                 (a.attributeType == AttributeType.NatureMultiplier)    ||
                 (a.attributeType == AttributeType.ArcaneMultiplier)    ||
                 (a.attributeType == AttributeType.HolyMultiplier)      ||
                 (a.attributeType == AttributeType.ShadowMultiplier)    ||
                 (a.attributeType == AttributeType.CritMultiplier)      ||
                 (a.attributeType == AttributeType.SacrificialMultiplier))
        {
            if (value <= 0)
            {
                valueText.text = "0%";
            }
            else
            {
                valueText.text = value.ToString() + "%";
            }

            if (a.attributeType == AttributeType.Accuracy || a.attributeType == AttributeType.Crit)
            {
                if (value > 100)
                {
                    string overCapColor = "#FF3C3C";

                    valueText.text = string.Format("<color={0}>100%</color>", overCapColor);
                }
            }
        }
        else if (a.attributeType == AttributeType.Armor || a.attributeType == AttributeType.Resistance)
        {
            valueText.text = value.ToString() + " / " + GeneralUtilities.RoundFloat((1 - GeneralUtilities.DefensiveReductionValue_League_Tweaked(value)) * 100, 0).ToString() + "%";
        }
        else
        {
            valueText.text = value.ToString();
        }
    }

    public string GetDescription(TooltipObject tooltipInfo)
    {
        switch (attribute.attributeType)
        {
            // General Attributes
            case AttributeType.Health:
                return "Health: Increases maximum Health Points.";
            case AttributeType.Power:
                return "Power: Increases the effectiveness of physical abilities.";
            case AttributeType.Wisdom:
                return "Wisdom: Increases the effectiveness of magical abilities.";
            case AttributeType.Armor:
                return "Armor: Decreases damage taken from Physical damage.";
            case AttributeType.Resistance:
                return "Resistance: Decreases damage taken from Magical damage.";
            case AttributeType.Vitality:
                return "Vitality: Increases healing taken by 1% per point.";
            case AttributeType.Speed:
                return "Speed: Determines order in queue.";
            case AttributeType.Accuracy:
                return "Accuracy: Increases the chance to hit your enemy with full force.\n\nCannot be higher than 100%.";
            case AttributeType.Crit:
                return "Crit: Increases chance to do additional damage or healing.\n\nCannot be higher than 100%.";

            // School Multipliers
            case AttributeType.HealingMultiplier:
                return "Healing: Determines healing done.";
            case AttributeType.PhysicalMultiplier:
                return "Physical Damage: Determines damage done by this school.";
            case AttributeType.FireMultiplier:
                return "Fire Damage: Determines damage done by this school.";
            case AttributeType.IceMultiplier:
                return "Ice Damage: Determines damage done by this school.";
            case AttributeType.NatureMultiplier:
                return "Nature Damage: Determines damage done by this school.";
            case AttributeType.ArcaneMultiplier:
                return "Arcane Damage: Determines damage done by this school.";
            case AttributeType.HolyMultiplier:
                return "Holy Damage: Determines damage done by this school.";
            case AttributeType.ShadowMultiplier:
                return "Shadow Damage: Determines damage done by this school.";
            case AttributeType.CritMultiplier:
                return "Determines the power of a critical hit.";
            case AttributeType.SacrificialMultiplier:
                return "Sacrificial Damage: Determines the damage done by this school.";
            default:
                return "No tooltip.";
        }       
    }
}
