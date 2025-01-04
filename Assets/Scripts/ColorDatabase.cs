using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorDatabase
{
    public static string QualityColor(Quality quality)
    {
        switch (quality)
        {
            case Quality.Common:
                return "#FEF0AE";
            case Quality.Mystical:
                return "#00FF80";
            case Quality.Legendary:
                return "#FF9B00";
            default:
                return "#FFFFFF";
        }
    }

    public static string AbilityTypeColor(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Primary:
                return "#C6C6C6";
            case AbilityType.Protection:
                return "#97FF9A";
            case AbilityType.Assault:
                return "#C76868";
            case AbilityType.Flask:
                return "#FDFF8F";
            default:
                return "#FFFFFF";
        }
    }

    public static string SchoolColor(AbilitySchool school)
    {
        switch (school)
        {
            case AbilitySchool.Healing:
                return "#00FF80";
            case AbilitySchool.Physical:
                return "#808080";
            case AbilitySchool.Fire:
                return "#FF6600";
            case AbilitySchool.Ice:
                return "#9AFFFF";
            case AbilitySchool.Nature:
                return "#00B300";
            case AbilitySchool.Arcane:
                return "#3366FF";
            case AbilitySchool.Holy:
                return "#FFFF9B";
            case AbilitySchool.Shadow:
                return "#66339A";
            case AbilitySchool.Sacrificial:
                return "#FF0000";
            default:
                return "#FFFFFF";
        }
    }

    public static string MagicalColor()
    {
        return "#BF87FF";
    }

    public static string ScalingColor(AttributeType attributeType)
    {
        switch (attributeType)
        {
            // General Attributes
            case AttributeType.Health:
                return "#00FF80";
            case AttributeType.Power:
                return "#FF9300";
            case AttributeType.Wisdom:
                return "#007CFF";
            case AttributeType.Armor:
                return "#C4C4C4";
            case AttributeType.Resistance:
                return "#B064A8";
            case AttributeType.Vitality:
                return "#00FFB9";
            case AttributeType.Speed:
                return "#FFFF00";
            case AttributeType.Accuracy:
                return "#FFFFFF";
            case AttributeType.Crit:
                return "#FF4D00";

            // School Multipliers
            case AttributeType.HealingMultiplier:
                return SchoolColor(AbilitySchool.Healing);
            case AttributeType.PhysicalMultiplier:
                return SchoolColor(AbilitySchool.Physical);
            case AttributeType.FireMultiplier:
                return SchoolColor(AbilitySchool.Fire);
            case AttributeType.IceMultiplier:
                return SchoolColor(AbilitySchool.Ice);
            case AttributeType.NatureMultiplier:
                return SchoolColor(AbilitySchool.Nature);
            case AttributeType.ArcaneMultiplier:
                return SchoolColor(AbilitySchool.Arcane);
            case AttributeType.HolyMultiplier:
                return SchoolColor(AbilitySchool.Holy);
            case AttributeType.ShadowMultiplier:
                return SchoolColor(AbilitySchool.Shadow);
            case AttributeType.CritMultiplier:
                return ScalingColor(AttributeType.Crit);
            case AttributeType.SacrificialMultiplier:
                return SchoolColor(AbilitySchool.Sacrificial);
            default:
                return "#FFFFFF";
        }
    }

    public static string AttributeBaseColor(AttributeBase attributeBase)
    {
        switch (attributeBase)
        {
            case AttributeBase.CurrentHealthPercentage:
                return ScalingColor(AttributeType.Health);
            case AttributeBase.MissingHealthPercentage:
                return ScalingColor(AttributeType.Health);
            case AttributeBase.Armor:
                return ScalingColor(AttributeType.Armor);
            case AttributeBase.Resistance:
                return ScalingColor(AttributeType.Resistance);
            default:
                return "#FFFFFF";
        }
    }

    public static string ThresholdTypeColor(ThresholdType attributeBase)
    {
        switch (attributeBase)
        {
            case ThresholdType.CurrentHealthPercentage:
                return ScalingColor(AttributeType.Health);
            case ThresholdType.Armor:
                return ScalingColor(AttributeType.Armor);
            case ThresholdType.Resistance:
                return ScalingColor(AttributeType.Resistance);
            case ThresholdType.Speed:
                return ScalingColor(AttributeType.Speed);
            default:
                return "#FFFFFF";
        }
    }

    public static string AttributeIncreaseColor(AttributeIncrease attributeIncrease)
    {
        switch (attributeIncrease)
        {
            case AttributeIncrease.Accuracy:
                return ScalingColor(AttributeType.Accuracy);
            case AttributeIncrease.CritChance:
                return ScalingColor(AttributeType.Crit);
            case AttributeIncrease.CritDamage:
                return ScalingColor(AttributeType.Crit);
            case AttributeIncrease.TotalDamagePercentage:
                return ScalingColor(AttributeType.Health);
            default:
                return "#FFFFFF";
        }
    }

    public static string NonScalingColor()
    {
        return "#FEF0AE";
    }

    public static string EffectColor()
    {
        return "#70FF62";
    }

    public static string Gray()
    {
        return "#C0C0C0";
    }

    public static string CasterColor()
    {
        return "#66CCFF";
    }

    public static string GeneralInformation()
    {
        return "#DBFF73";
    }

    public static string Red()
    {
        return "#FF0000";
    }

    public static string Description()
    {
        return "#FFD78B";
    }

    public static string Positive => "#1FFF00";

    public static string Negative => "#FF0000";

    public static Color ConvertString2Color(string s)
    {
        ColorUtility.TryParseHtmlString(s, out Color c);

        return c;
    }
}
