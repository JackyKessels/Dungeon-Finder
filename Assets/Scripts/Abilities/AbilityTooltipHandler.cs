using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class AbilityTooltipHandler
{
    public static string ParseName(string temp, string check, AbilityObject abilityObject)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.GeneralInformation() + ">{0}</color>");

            return string.Format(temp, abilityObject.name);
        }

        return temp;
    }

    public static string ShowAbilityType(AbilityType abilityType)
    {
        if (abilityType != AbilityType.Passive)
            return string.Format("\n<color={0}>{1}</color>", ColorDatabase.AbilityTypeColor(abilityType), abilityType.ToString());
        else
            return "";
    }

    public static string ParseAllAbilitySourceTooltips(string s, TooltipObject tooltipInfo, List<AbilitySource> abilitySources, string prefix)
    {
        string temp = s;

        for (int i = 0; i < abilitySources.Count; i++)
        {
            temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}>", abilitySources[i], tooltipInfo);
            temp = DetermineAbilityModifierBaseline(temp, $"<{prefix}{i + 1}modV>", abilitySources[i].abilityModifier);
            temp = DetermineAbilityModifierAttribute(temp, $"<{prefix}{i + 1}modP>", abilitySources[i].abilityModifier);
            temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}modB>", abilitySources[i].abilityModifier.bonusAbilitySource.GetAbilitySource(), tooltipInfo);
            temp = DetermineAbilityConditionValue(temp, $"<{prefix}{i + 1}conV>", abilitySources[i].abilityModifier);
            temp = ParseEffectTooltips(temp, $"{prefix}{i + 1}modCE", new List<EffectObject>() { abilitySources[i].abilityModifier.conditionalEffect }, tooltipInfo);
            temp = DetermineFractionDrained(temp, $"<{prefix}{i + 1}drain>", abilitySources[i].drainModifier);
        }

        return temp;
    }

    public static string ParseAllEffectTooltips(string s, TooltipObject tooltipInfo, List<EffectObject> selfEffects, List<EffectObject> targetEffects)
    {
        string temp = s;

        temp = ParseEffectTooltips(temp, "s", selfEffects, tooltipInfo);

        temp = ParseEffectTooltips(temp, "t", targetEffects, tooltipInfo);

        return temp;
    }

    public static string ParseApplyEffectTooltips(string s, TooltipObject tooltipInfo, List<ApplyEffectObject> applyEffects)
    {
        string temp = s;

        List<EffectObject> applyEffectObjects = applyEffects.ConvertAll(a => a.effectObject);

        temp = ParseEffectTooltips(temp, "apply", applyEffectObjects, tooltipInfo);

        return temp;
    }

    public static string ParseCastAbility(string s, string checkInfo, string checkTooltip, TooltipObject tooltipObject, ActiveAbility activeAbility)
    {
        string temp = s;
        string color = ColorDatabase.GeneralInformation();

        Unit caster = GeneralUtilities.GetCorrectUnit(tooltipObject);

        int level = 1;

        if (caster != null)
        {
            (bool hasAbility, int abilityLevel) = caster.spellbook.GetAbility(activeAbility);

            if (hasAbility)
            {
                level = abilityLevel;
            }
        }

        if (temp.Contains(checkInfo))
        {
            string text = "Level " + level + " " + activeAbility.name;

            temp = temp.Replace(checkInfo, "<color={1}>{0}</color>");

            temp = string.Format(temp, text, color);
        }

        if (temp.Contains(checkTooltip))
        {
            temp = temp.Replace(checkTooltip, "{0}");

            tooltipObject.active.level = level;

            temp = string.Format(temp, activeAbility.GetNameAndDescription(tooltipObject));
        }

        return temp;
    }

    public static string ParseCastAbilityEffectiveness(string s, string check, CastActiveAbility castActiveAbility)
    {
        string temp = s;
        string color = ColorDatabase.GeneralInformation();

        if (temp.Contains(check))
        {
            int percentage = GeneralUtilities.RoundFloat(100 * castActiveAbility.effectiveness, 0);

            temp = temp.Replace(check, "<color={1}>{0}</color>% effectiveness");

            temp = string.Format(temp, percentage, color);
        }

        return temp;
    }

    public static string ParseDoubleCastEffectiveness(string s, string check, float effectiveness)
    {
        string temp = s;
        string color = ColorDatabase.GeneralInformation();

        if (temp.Contains(check))
        {
            int percentage = GeneralUtilities.RoundFloat(100 * effectiveness, 0);

            temp = temp.Replace(check, "<color={1}>{0}</color>% effectiveness");

            temp = string.Format(temp, percentage, color);
        }

        return temp;
    }

    public static string InsertRed(string s)
    {
        string pattern = @"\<r!([^>]*)\>";

        string temp = Regex.Replace(s, pattern, "<color=#FF0000>$1</color>");

        return temp;
    }

    public static string InsertGeneric(string s)
    {
        string pattern = @"\<g!([^>]*)\>";

        string color = ColorDatabase.GeneralInformation();

        string temp = Regex.Replace(s, pattern, "<color={0}>$1</color>");

        temp = string.Format(temp, color);

        return temp;
    }

    public static string InsertNonScaling(string s)
    {
        string pattern = @"\<ns!([^>]*)\>";

        string temp = Regex.Replace(s, pattern, "<color=" + ColorDatabase.NonScalingColor() + ">$1</color>");

        return temp;
    }

    public static string ParseEffectTooltips(string temp, string prefix, List<EffectObject> effects, TooltipObject tooltipInfo)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            temp = DetermineEffectName(temp, $"<{prefix}{i + 1}name>", effects[i]);
            temp = DetermineEffectDuration(temp, $"<{prefix}{i + 1}d>", effects[i]);
            temp = ParseStacking(temp, $"<{prefix}{i + 1}stacks>", effects[i]);
            temp = DoesNotRefresh(temp, $"<{prefix}{i + 1}refresh>", effects[i]);
            temp = UniqueEffect(temp, $"<{prefix}{i + 1}unique>", effects[i]);

            if (effects[i] is EffectAttributeModifier attributeModifier)
            {
                temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}modV>", attributeModifier.modifierSource, tooltipInfo);
                temp = DetermineModifierEffectPercentage(temp, $"<{prefix}{i + 1}modP>", attributeModifier, tooltipInfo);
            }

            if (effects[i] is EffectAbilityModifier abilityModifier)
            {
                temp = DetermineBonusMultipler(temp, $"<{prefix}{i + 1}bonus>", abilityModifier, tooltipInfo.GetAbilityLevel());
                temp = DetermineSpecificAbilities(temp, "<specific>", abilityModifier);
            }

            if (effects[i] is EffectCooldownReduction cdr)
            {
                temp = DetermineCooldownReduction(temp, $"<{prefix}{i + 1}cdr>", cdr);
                temp = DetermineCooldownReductionAbility(temp, $"<{prefix}{i + 1}cdrSpecific>", cdr);
                temp = DetermineCooldownReductionType(temp, $"<{prefix}{i + 1}type>", cdr);
            }

            if (effects[i] is EffectDamageTransfer dmg)
            {
                temp = DetermineTransferPercentage(temp, $"<{prefix}{i + 1}transfer>", dmg);
            }

            if (effects[i] is EffectOverTime dot)
            {
                for (int j = 0; j < 3; j++)
                {
                    temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}timAp{j + 1}>", dot.GetAbilitySource(TimedActionType.OnApplication, j), tooltipInfo);
                    temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}timAc{j + 1}>", dot.GetAbilitySource(TimedActionType.OnActive, j), tooltipInfo);
                    temp = DetermineAbilityValue(temp, $"<{prefix}{i + 1}timEx{j + 1}>", dot.GetAbilitySource(TimedActionType.OnExpiration, j), tooltipInfo);
                }
            }

            if (effects[i] is EffectConditionalTrigger trigger)
            {
                temp = GetConditionalTriggerDescription(temp, $"<{prefix}{i + 1}trigger>", trigger, tooltipInfo);
            }

            if (effects[i] is EffectActivatePassive passive)
            {
                temp = GetActivatedPassiveDescription(temp, $"<{prefix}{i + 1}passive>", passive, tooltipInfo);
            }
        }

        return temp;
    }

    private static string GetConditionalTriggerDescription(string s, string check, EffectConditionalTrigger trigger, TooltipObject tooltipInfo)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, trigger.GetConditionalDescription(tooltipInfo));
        }

        return temp;
    }

    private static string GetActivatedPassiveDescription(string s, string check, EffectActivatePassive passive, TooltipObject tooltipInfo)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, passive.passiveAbility.ParseDescription(passive.passiveAbility.description, tooltipInfo));
        }

        return temp;
    }

    public static string DoesNotEndTurn(string temp)
    {
        string color = ColorDatabase.GeneralInformation();

        temp += string.Format("<color={0}>\n\nThis ability does not end your Turn.</color>", color);

        return temp;
    }


    public static string ReplacesAbility(string temp, AbilityObject abilityObject)
    {
        string color = ColorDatabase.GeneralInformation();

        temp += string.Format("\n\nThis ability replaces <color={0}>{1}</color>.", color, abilityObject.name);

        return temp;
    }

    public static string ResetChance(string temp, int chance)
    {
        string color = ColorDatabase.GeneralInformation();

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
        temp = ColorSchool(temp, "<sac>", AbilitySchool.Sacrificial);

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
        string color = ColorDatabase.NonScalingColor();

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color={1}>{0}</color>%");

            return string.Format(temp, chance, color);
        }

        return temp;
    }

    public static string ParseAbilityType(string temp, string checkPrimary, string checkAssault, string checkProtection)
    {
        if (temp.Contains(checkPrimary))
        {
            string color = ColorDatabase.AbilityTypeColor(AbilityType.Primary);

            temp = temp.Replace(checkPrimary, "<color={1}>{0}</color>");

            temp = string.Format(temp, AbilityType.Primary.ToString(), color);
        }

        if (temp.Contains(checkAssault))
        {
            string color = ColorDatabase.AbilityTypeColor(AbilityType.Assault);

            temp = temp.Replace(checkAssault, "<color={1}>{0}</color>");

            temp = string.Format(temp, AbilityType.Assault.ToString(), color);
        }

        if (temp.Contains(checkProtection))
        {
            string color = ColorDatabase.AbilityTypeColor(AbilityType.Protection);

            temp = temp.Replace(checkProtection, "<color={1}>{0}</color>");

            temp = string.Format(temp, AbilityType.Protection.ToString(), color);
        }

        return temp;
    }

    public static string CriticalHit(string temp, string check)
    {
        string color = ColorDatabase.NonScalingColor();

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(AttributeType.Crit) + ">Critical</color>");

            return string.Format(temp, color);
        }

        return temp;
    }

    public static string GlancingHit(string temp, string check)
    {
        string color = ColorDatabase.NonScalingColor();

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + color + ">Glancing</color>");

            return string.Format(temp, color);
        }

        return temp;
    }

    public static string TriggerPassive(string temp, bool triggersPassives, bool triggeredByPassives, bool triggeredByEffects)
    {
        string color = ColorDatabase.Red();

        string triggers = "This ";

        if (!triggersPassives)
            triggers += "does not trigger passives";

        if (!triggersPassives && (!triggeredByPassives || !triggeredByEffects))
            triggers += " and ";

        if (!triggeredByPassives && triggeredByEffects)
            triggers += "is not triggered by passives";

        if (!triggeredByEffects && triggeredByPassives)
            triggers += "is not triggered by effects";

        if (!triggeredByPassives && !triggeredByEffects)
            triggers += "is not triggered by passives and effects";

        temp += string.Format("<color={0}>\n\n{1}.</color>", color, triggers);

        return temp;
    }

    public static string TriggeredByPassive(string temp)
    {
        string color = ColorDatabase.Red();

        temp += string.Format("<color={0}>\n\nThis is not triggered by passives.</color>", color);

        return temp;
    }

    public static string TriggeredByEffects(string temp)
    {
        string color = ColorDatabase.Red();

        temp += string.Format("<color={0}>\n\nThis is not triggered by effects.</color>", color);

        return temp;
    }

    public static string DetermineCooldownReduction(string temp, string check, EffectCooldownReduction cdr)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, cdr.roundsReduction);
        }

        return temp;
    }

    public static string DetermineCooldownReductionAbility(string temp, string check, EffectCooldownReduction cdr)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, cdr.specificAbility.name);
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
        if (source == null)
            return temp;

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
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, modifier.increaseValue);
        }

        return temp;
    }

    public static string DetermineAbilityModifierAttribute(string temp, string check, AbilityModifier modifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, modifier.percentageIncrease);
        }

        return temp;
    }

    public static string DetermineAbilityConditionValue(string temp, string check, AbilityModifier modifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, modifier.conditionThreshold);
        }

        return temp;
    }

    public static string DetermineFractionDrained(string temp, string check, DrainModifier drainModifier)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>%");

            float percentage = drainModifier.fractionDrained * 100f;

            double rounded = System.Math.Round((double)percentage, 1);

            return string.Format(temp, rounded);
        }

        return temp;
    }

    private static string DetermineModifierEffectPercentage(string temp, string check, EffectAttributeModifier effectAttributeModifier, TooltipObject tooltipInfo)
    {
        if (temp.Contains(check))
        {
            //temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(effectAttributeModifier.attributeModified) + ">{0}</color>%");
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>%");

            float percentage = (effectAttributeModifier.multiplier + effectAttributeModifier.multiplierPerLevel * (tooltipInfo.GetAbilityLevel() - 1)) * 100f;

            double rounded = System.Math.Round((double)percentage, 1);

            return string.Format(temp, rounded);
        }

        return temp;
    }

    private static string DetermineEffectDuration(string temp, string check, EffectObject effectObject)
    {
        string durationColor = ColorDatabase.Gray();

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
            string nameColor = ColorDatabase.GeneralInformation();

            temp = temp.Replace(check, "<color=" + nameColor + ">{0}</color>");

            return string.Format(temp, effectObject.name);
        }

        return temp;
    }

    private static string ParseStacking(string temp, string check, EffectObject effectObject)
    {
        if (temp.Contains(check) && effectObject.stackable)
        {
            string color = ColorDatabase.GeneralInformation();

            if (effectObject.maxStacks > 0)
            {
                temp = temp.Replace(check, "<color=" + color + ">{0}</color>" + " stacks up to <color=" + color + ">{1}</color> times.");

                return string.Format(temp, effectObject.name, effectObject.maxStacks);
            }
            else
            {
                temp = temp.Replace(check, "<color=" + color + ">{0}</color>" + " stacks and is not capped.");

                return string.Format(temp, effectObject.name);
            }
        }

        return temp;
    }

    private static string DoesNotRefresh(string temp, string check, EffectObject effectObject)
    {
        if (temp.Contains(check) && !effectObject.refreshes && effectObject.stackable)
        {
            string color = ColorDatabase.GeneralInformation();

            temp = temp.Replace(check, "The duration of " + "<color=" + color + ">{0}</color>" + " cannot be refreshed.");

            return string.Format(temp, effectObject.name, effectObject.maxStacks);
        }

        return temp;
    }

    private static string UniqueEffect(string temp, string check, EffectObject effectObject)
    {
        if (temp.Contains(check) && effectObject.unique)
        {
            string color = ColorDatabase.GeneralInformation();

            temp = temp.Replace(check, "<color=" + color + ">{0}</color>" + " can only be active on " + "<color=" + color + ">1</color>" + " target.");

            return string.Format(temp, effectObject.name);
        }

        return temp;
    }


    public static string DetermineBonusMultipler(string temp, string check, EffectAbilityModifier abilityModifier, int level)
    {
        if (temp.Contains(check))
        {
            string color = ColorDatabase.NonScalingColor();

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
            string color = ColorDatabase.GeneralInformation();

            List<string> strings = abilityModifier.specificAbilities.Select(x => x.name).ToList();

            return temp.Replace(check, JoinString(strings, ", ", " or ", color));
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

    public static string CurrentArmor(string temp, string check, TooltipObject tooltipObject)
    {
        Unit caster = GeneralUtilities.GetCorrectUnit(tooltipObject);

        if (temp.Contains(check))
        {
            if (caster == null)
            {
                temp = temp.Replace(check, "Current Armor: -");
            }
            else
            {
                string color = ColorDatabase.ScalingColor(AttributeType.Armor);

                temp = temp.Replace(check, "Current Armor: <color={0}>{1}</color>");

                return string.Format(temp, color, caster.statsManager.GetAttributeValue(AttributeType.Armor));
            }   
        }

        return temp;
    }

    // Turns a list of strings into a string where each element is seperated by a string
    // The last two elements are seperated with a different string
    // Each seperate element (not the commas or and) is also colored
    public static string JoinString(List<string> strings, string separator, string finalSeparator, string color)
    {
        if (strings == null || strings.Count == 0)
            return "";

        string fullString;

        if (strings.Count > 1)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < strings.Count - 1; i++)
            {
                stringList.Add(string.Format("<color={1}>{0}</color>", strings[i], color));
            }

            fullString = string.Join(separator, stringList);

            fullString += finalSeparator + string.Format("<color={1}>{0}</color>", strings[strings.Count - 1], color);
        }
        else
        {
            fullString = string.Format("<color={1}>{0}</color>", strings[0], color);
        }

        return fullString;
    }
}
