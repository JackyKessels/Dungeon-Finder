using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class AbilityTooltipHandler
{
    public static string ParseAbilityType(AbilityType abilityType)
    {
        if (abilityType != AbilityType.Passive)
            return string.Format("\n<color={0}>{1}</color>", ColorDatabase.AbilityTypeColor(abilityType), abilityType.ToString());
        else
            return "";
    }

    public static string ParseAllEffectTooltips(string s, TooltipObject tooltipInfo, List<EffectObject> selfEffects, List<EffectObject> targetEffects)
    {
        string temp = s;

        temp = ParseEffectTooltips(tooltipInfo, selfEffects, temp, "s");

        temp = ParseEffectTooltips(tooltipInfo, targetEffects, temp, "t");

        return temp;
    }

    public static string InsertRed(string s)
    {
        string pattern = @"\<r([^>]*)\>";

        string temp = Regex.Replace(s, pattern, "<color=#FF0000>$1</color>");

        return temp;
    }

    public static string ParseEffectTooltips(TooltipObject tooltipInfo, List<EffectObject> effects, string temp, string check)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            temp = DetermineEffectName(temp, string.Format("<{1}{0}name>", i + 1, check), effects[i]);
            temp = DetermineEffectDuration(temp, string.Format("<{1}{0}d>", i + 1, check), effects[i]);

            if (effects[i] is EffectAttributeModifier attributeModifier)
            {
                temp = DetermineAbilityValue(temp, string.Format("<{1}{0}modV>", i + 1, check), attributeModifier.modifierSource, tooltipInfo);
                temp = DetermineModifierEffectPercentage(temp, string.Format("<{1}{0}modP>", i + 1, check), attributeModifier, tooltipInfo);
            }

            if (effects[i] is EffectAbilityModifier abilityModifier)
            {
                temp = DetermineBonusMultipler(temp, string.Format("<{1}{0}bonus>", i + 1, check), abilityModifier, tooltipInfo.active.level);
                temp = DetermineSpecificAbilities(temp, "<specific>", abilityModifier);
            }

            if (effects[i] is EffectCooldownReduction cdr)
            {
                temp = DetermineCooldownReduction(temp, string.Format("<{1}{0}cdr>", i + 1, check), cdr);
                temp = DetermineCooldownReductionType(temp, string.Format("<{1}{0}type>", i + 1, check), cdr);
            }

            if (effects[i] is EffectDamageTransfer dmg)
            {
                temp = DetermineTransferPercentage(temp, string.Format("<{1}{0}transfer>", i + 1, check), dmg);
            }

            if (effects[i] is EffectOverTime dot)
            {
                for (int j = 0; j < 3; j++)
                {
                    temp = DetermineAbilityValue(temp, string.Format("<{2}{0}timAp{1}>", i + 1, j + 1, check), dot.GetAbilitySource(TimedActionType.OnApplication, j), tooltipInfo);
                    temp = DetermineAbilityValue(temp, string.Format("<{2}{0}timAc{1}>", i + 1, j + 1, check), dot.GetAbilitySource(TimedActionType.OnActive, j), tooltipInfo);
                    temp = DetermineAbilityValue(temp, string.Format("<{2}{0}timEx{1}>", i + 1, j + 1, check), dot.GetAbilitySource(TimedActionType.OnExpiration, j), tooltipInfo);
                }
            }
        }

        return temp;
    }

    public static string DoesNotEndTurn(string temp)
    {
        string color = "#DBFF73";

        temp += string.Format("<color={0}>\n\nThis ability does not end your Turn.</color>", color);

        return temp;
    }

    public static string ReplacesAbility(string temp, AbilityObject abilityObject)
    {
        string color = "#DBFF73";

        temp += string.Format("\n\nThis ability replaces <color={0}>{1}</color>.", color, abilityObject.name);

        return temp;
    }

    public static string ResetChance(string temp, int chance)
    {
        string color = "#DBFF73";

        temp += string.Format("\n\nHas a <color={0}>{1}</color>% chance to reset its cooldown.", color, chance);

        return temp;
    }

    public static string ColorAllSchools(string temp)
    {
        temp = ColorSchool(temp, "<Heal>", AbilitySchool.Healing);
        temp = ColorSchool(temp, "<P>", AbilitySchool.Physical);
        temp = ColorSchool(temp, "<F>", AbilitySchool.Fire);
        temp = ColorSchool(temp, "<I>", AbilitySchool.Ice);
        temp = ColorSchool(temp, "<N>", AbilitySchool.Nature);
        temp = ColorSchool(temp, "<A>", AbilitySchool.Arcane);
        temp = ColorSchool(temp, "<H>", AbilitySchool.Holy);
        temp = ColorSchool(temp, "<S>", AbilitySchool.Shadow);

        temp = ColorMagicalSchool(temp, "<M>");

        return temp;
    }

    public static string ColorAllAttributes(string temp)
    {
        foreach (AttributeType attributeType in System.Enum.GetValues(typeof(AttributeType)))
        {
            string lowerCase = attributeType.ToString().ToLower();

            temp = ColorAttribute(temp, "<" + lowerCase + ">", attributeType);
        }

        return temp;
    }

    private static string ColorSchool(string temp, string check, AbilitySchool school)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.SchoolColor(school) + ">{0}</color>");

            return string.Format(temp, school);
        }

        return temp;
    }

    private static string ColorAttribute(string temp, string check, AttributeType attributeType)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(attributeType) + ">{0}</color>");

            return string.Format(temp, GeneralUtilities.GetCorrectAttributeName(attributeType));
        }

        return temp;
    }

    private static string ColorMagicalSchool(string temp, string check)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.MagicalColor() + ">Magical</color>");
        }

        return temp;
    }

    public static string ParseProcChance(string temp, string check, int chance)
    {
        string color = "#FEF0AE";

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color={1}>{0}</color>%");

            return string.Format(temp, chance, color);
        }

        return temp;
    }

    public static string CriticalHit(string temp, string check)
    {
        string color = "#FFE29B";

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(AttributeType.Crit) + ">Critical</color>");

            return string.Format(temp, color);
        }

        return temp;
    }

    public static string TriggerPassive(string temp)
    {
        string color = "#FF0000";

        temp += string.Format("<color={0}>\n\nThis does not trigger passive effects.</color>", color);

        return temp;
    }

    public static string DetermineCooldownReduction(string temp, string check, EffectCooldownReduction cdr)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=#FEF0AE>{0}</color>");

            return string.Format(temp, cdr.roundsReduction);
        }

        return temp;
    }

    public static string DetermineCooldownReductionType(string temp, string check, EffectCooldownReduction cdr)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.AbilityTypeColor(cdr.abilityType) + ">{0}</color>");

            return string.Format(temp, cdr.abilityType);
        }

        return temp;
    }

    public static string DetermineAbilityValue(string temp, string check, AbilitySource source, TooltipObject tooltipInfo)
    {
        if (temp.Contains(check))
        {
            if (tooltipInfo.state == CurrentState.Values)
            {
                if (!source.HasAttributeScaling())
                {
                    temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

                    return string.Format(temp, source.GetTooltipValue(tooltipInfo));
                }
                else
                {
                    temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(source.attributeType) + ">({0} {1})</color>");

                    return string.Format(temp, source.GetTooltipValue(tooltipInfo), GeneralUtilities.GetCorrectAttributeName(source.attributeType));
                }
            }
            else
            {
                if (!source.HasAttributeScaling())
                {
                    temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

                    return string.Format(temp, source.CalculateValue(tooltipInfo));
                }
                else
                {
                    temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(source.attributeType) + ">{0}</color>");

                    return string.Format(temp, source.CalculateValue(tooltipInfo));
                }
            }
        }

        return temp;
    }

    public static string DetermineAbilityModifierBaseline(string temp, string check, AbilityModifier modifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.AttributeIncreaseColor(modifier.increasedAttribute) + ">{0}</color>");

            return string.Format(temp, modifier.increaseValue);
        }

        return temp;
    }

    public static string DetermineAbilityModifierAttribute(string temp, string check, AbilityModifier modifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.AttributeBaseColor(modifier.attributeBase) + ">{0}</color>");

            return string.Format(temp, modifier.percentageIncrease);
        }

        return temp;
    }

    public static string DetermineAbilityConditionValue(string temp, string check, AbilityModifier modifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ThresholdTypeColor(modifier.thresholdType) + ">{0}</color>");

            return string.Format(temp, modifier.conditionThreshold);
        }

        return temp;
    }

    //private static string DetermineModifierEffectValue(string temp, string check, EffectAttributeModifier effectAttributeModifier, TooltipObject tooltipInfo)
    //{
    //    if (temp.Contains(check))
    //    {
    //        AbilitySource currentSource = effectAttributeModifier.modifierSource;

    //        if (tooltipInfo.state == CurrentState.Values)
    //        {
    //            if (!currentSource.HasAttributeScaling())
    //            {
    //                string color = "#FEF0AE";

    //                temp = temp.Replace(check, "<color=" + color + ">{0}</color>");

    //                return string.Format(temp, currentSource.GetTooltipValue(tooltipInfo));
    //            }
    //            else
    //            {
    //                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(currentSource.attributeType) + ">({0} {1})</color>");

    //                return string.Format(temp, currentSource.GetTooltipValue(tooltipInfo), GeneralUtilities.GetCorrectAttributeName(currentSource.attributeType));
    //            }
    //        }
    //        else
    //        {
    //            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(currentSource.attributeType) + ">{0}</color>");

    //            return string.Format(temp, currentSource.CalculateValue(tooltipInfo));
    //        }
    //    }

    //    return temp;
    //}

    private static string DetermineModifierEffectPercentage(string temp, string check, EffectAttributeModifier effectAttributeModifier, TooltipObject tooltipInfo)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(effectAttributeModifier.attributeModified) + ">{0}</color>%");

            return string.Format(temp, (effectAttributeModifier.multiplier + effectAttributeModifier.multiplierPerLevel * (tooltipInfo.active.level - 1)) * 100);
        }

        return temp;
    }

    //private static string DetermineEffectValue(string temp, string check, EffectOverTime effectOverTime, TimedActionType actionType, int actionIndex, TooltipObject tooltipInfo)
    //{
    //    if (temp.Contains(check))
    //    {
    //        AbilitySource currentSource = effectOverTime.GetAbilitySource(actionType, actionIndex);

    //        if (tooltipInfo.state == CurrentState.Values)
    //        {
    //            if (!currentSource.HasAttributeScaling())
    //            {
    //                temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

    //                return string.Format(temp, currentSource.GetTooltipValue(tooltipInfo));
    //            }
    //            else
    //            {
    //                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(currentSource.attributeType) + ">({0} {1})</color>");

    //                return string.Format(temp, currentSource.GetTooltipValue(tooltipInfo), GeneralUtilities.GetCorrectAttributeName(currentSource.attributeType));
    //            }
    //        }
    //        else
    //        {
    //            if (!currentSource.HasAttributeScaling())
    //            {
    //                temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

    //                return string.Format(temp, currentSource.CalculateValue(tooltipInfo));
    //            }
    //            else
    //            {
    //                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(currentSource.attributeType) + ">{0}</color>");

    //                return string.Format(temp, currentSource.CalculateValue(tooltipInfo));
    //            }
    //        }
    //    }

    //    return temp;
    //}

    private static string DetermineEffectDuration(string temp, string check, EffectObject effectObject)
    {
        string durationColor = "#C4C4C4";

        if (temp.Contains(check))
        {
            int duration = effectObject.duration;

            temp = temp.Replace(check, "<color=" + durationColor + ">{0}</color>");

            return string.Format(temp, duration);
        }

        return temp;
    }

    private static string DetermineEffectName(string temp, string check, EffectObject effectObject)
    {
        if (temp.Contains(check))
        {
            string nameColor = "#FEF0AE";

            temp = temp.Replace(check, "<color=" + nameColor + ">{0}</color>");

            return string.Format(temp, effectObject.name);
        }

        return temp;
    }

    public static string DetermineBonusMultipler(string temp, string check, EffectAbilityModifier abilityModifier, int level)
    {
        if (temp.Contains(check))
        {
            string color = "#FF0000";

            float fullValue = abilityModifier.GetBonusMultiplier(level) * 100;

            temp = temp.Replace(check, "<color={1}>{0}</color>%");

            return string.Format(temp, fullValue, color);
        }

        return temp;
    }

    public static string DetermineSpecificAbilities(string temp, string check, EffectAbilityModifier abilityModifier)
    {
        if (abilityModifier.specificAbilities.Count == 0)
            return temp;

        if (temp.Contains(check))
        {
            string color = "#DBFF73";

            string fullString = "";

            // One, Two or Three
            if (abilityModifier.specificAbilities.Count > 1)
            {
                List<string> stringList = new List<string>();

                for (int i = 0; i < abilityModifier.specificAbilities.Count - 1; i++)
                {
                    stringList.Add(string.Format("<color={1}>{0}</color>", abilityModifier.specificAbilities[i].name, color));
                }

                fullString = string.Join(", ", stringList);

                fullString += " or " + string.Format("<color={1}>{0}</color>", abilityModifier.specificAbilities[abilityModifier.specificAbilities.Count - 1].name, color);
            }
            // One
            else
            {
                fullString = string.Format("<color={1}>{0}</color>", abilityModifier.specificAbilities[0].name, color);
            }

            return temp.Replace(check, fullString);
        }

        return temp;
    }

    private static string DetermineTransferPercentage(string temp, string check, EffectDamageTransfer dmg)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.CasterColor() + ">" + dmg.percentage * 100 + "</color>%");
        }

        return temp;
    }
}
