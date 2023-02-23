using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilitySchool
{
    Healing,
    Physical,
    Fire,
    Ice,
    Nature,
    Arcane,
    Holy,
    Shadow,
    Sacrificial
}

public enum ReductionType
{
    None,
    Armor,
    Resistance,
}

[System.Serializable]
public class AbilitySource
{
    [HideInInspector] public static readonly float valueRangeVariance = 0.2f;

    [Header("[ General Values ]")]
    public bool ignoreMultipliers;
    public bool cannotCrit;
    public bool cannotMiss;
    public PassiveAbility ignorePassive; // This AbilitySource does not trigger this Passive
    public AbilitySchool school;
    public AttributeType attributeType;
    public int baseValue;
    public int levelBase;
    public float scaling;
    public float levelScaling;
    public ReductionType reductionType;

    [Header("[ Extra ]")]
    public AbilityModifier modifier;

    public AbilitySource (BonusAbilitySource bonusAbilitySource)
    {
        ignoreMultipliers = bonusAbilitySource.ignoreMultipliers;
        cannotCrit = bonusAbilitySource.cannotCrit;
        cannotMiss = bonusAbilitySource.cannotMiss;
        ignorePassive = bonusAbilitySource.ignorePassive;
        school = bonusAbilitySource.school;
        attributeType = bonusAbilitySource.attributeType;
        baseValue = bonusAbilitySource.baseValue;
        levelBase = bonusAbilitySource.levelBase;
        scaling = bonusAbilitySource.scaling;
        levelScaling = bonusAbilitySource.levelScaling;
        reductionType = bonusAbilitySource.reductionType;

        modifier = null;
    }

    // Gets the right value based on the current interface
    public int GetCorrectAttribute(TooltipObject tooltipInfo)
    {
        Unit unit;

        if (tooltipInfo.effect != null && tooltipInfo.effect.effectObject != null && tooltipInfo.effect.effectObject.description == "<passive>")
            unit = tooltipInfo.effect.caster;
        else
            unit = GeneralUtilities.GetCorrectUnit(tooltipInfo);

        return unit.statsManager.GetAttributeValue(attributeType);
    }

    public string GetTooltipValue(TooltipObject tooltipInfo)
    {
        int totalBase = baseValue + levelBase * (tooltipInfo.active.level - 1);

        float totalScaling = scaling + levelScaling * (tooltipInfo.active.level - 1);

        if (totalBase > 0 && totalScaling > 0)
        {
            return totalBase.ToString() + " + " + Mathf.RoundToInt(totalScaling * 100).ToString() + "%";
        }
        else if (totalBase > 0)
        {
            return totalBase.ToString();
        }
        else if (totalScaling > 0)
        {
            return (totalScaling * 100).ToString() + "%";
        }
        else
            return "";
    }

    // Calculates the value of the source using ability info from the TooltipIcon
    // Used for ability tooltips
    public int CalculateValue(TooltipObject tooltipInfo)
    {
        int totalBase = baseValue + levelBase * (tooltipInfo.active.level - 1);

        float totalScaling = scaling + levelScaling * (tooltipInfo.active.level - 1);

        int totalAttribute = GetCorrectAttribute(tooltipInfo);

        float totalValue = totalBase + totalScaling * totalAttribute;

        if (!ignoreMultipliers)
            totalValue = AddSchoolMultiplier(totalValue, school, GeneralUtilities.GetCorrectUnit(tooltipInfo));

        if (totalValue <= 0)
            totalValue = 0;

        return (int)System.Math.Round(totalValue, System.MidpointRounding.AwayFromZero);
    }

    // Calculates the value of the source using the caster and level
    // Used for damage calculations
    public float CalculateValue(Unit caster, int level, float modifier, float abilityMultiplier)
    {
        int totalBase = baseValue + levelBase * (level - 1);

        float totalScaling = scaling + levelScaling * (level - 1);

        int totalAttribute = caster.statsManager.GetAttributeValue(attributeType);

        float totalValue = totalBase + totalScaling * totalAttribute * modifier;

        if (!ignoreMultipliers)
            totalValue = AddSchoolMultiplier(totalValue, school, caster);

        totalValue *= abilityMultiplier;

        if (totalValue <= 0)
            totalValue = 0;

        return totalValue;
    }

    // Does all the functionality
    public void TriggerSource(AbilityObject sourceAbility, bool isPassive, bool isEffect, Unit caster, Unit target, int level, float adjacentModifier, bool isUnitTrigger, float abilityMultiplier, AbilityType abilityType)
    {
        Color color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(school));

        float value = CalculateValue(caster, level, adjacentModifier, abilityMultiplier);

        AbilityValue abilityValue = new AbilityValue(sourceAbility, isPassive, isEffect, value, school, abilityType, cannotCrit, cannotMiss, caster, target, color, isUnitTrigger, ignorePassive);

        if (school == AbilitySchool.Healing)
        {
            abilityValue.value = target.statsManager.CalculateVitalityHealing(value);
        }
        else
        {
            abilityValue.value = target.statsManager.CalculateMitigatedDamage(value, reductionType);
        }

        if (modifier != null)
            modifier.TriggerModifier(abilityValue, level);

        abilityValue.Trigger();
    }

    // Uses stored value for effects
    public void TriggerSource(AbilityObject sourceAbility, bool isPassive, bool isEffect, Unit caster, Unit target, float value, bool isUnitTrigger)
    {
        Color color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(school));

        AbilityValue abilityValue = new AbilityValue(sourceAbility, isPassive, isEffect, value, school, AbilityType.Assault, cannotCrit, cannotMiss, caster, target, color, isUnitTrigger, ignorePassive);

        if (school == AbilitySchool.Healing)
        {
            abilityValue.value = target.statsManager.CalculateVitalityHealing(value);
        }
        else
        {
            abilityValue.value = target.statsManager.CalculateMitigatedDamage(value, reductionType);
        }

        if (modifier != null)
            modifier.TriggerModifier(abilityValue);

        abilityValue.Trigger();
    }

    public static float AddSchoolMultiplier(float value, AbilitySchool school, Unit caster)
    {
        float multiplier;

        switch (school)
        {
            case AbilitySchool.Healing:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.HealingMultiplier);
                break;
            case AbilitySchool.Physical:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.PhysicalMultiplier);
                break;
            case AbilitySchool.Fire:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.FireMultiplier);
                break;
            case AbilitySchool.Ice:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.IceMultiplier);
                break;
            case AbilitySchool.Nature:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.NatureMultiplier);
                break;
            case AbilitySchool.Arcane:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.ArcaneMultiplier);
                break;
            case AbilitySchool.Holy:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.HolyMultiplier);
                break;
            case AbilitySchool.Shadow:
                multiplier = caster.statsManager.GetAttributeValue(AttributeType.ShadowMultiplier);
                break;
            default:
                multiplier = 100;
                break;
        }

        return value * (Mathf.Max(0, multiplier) / 100);
    }

    public bool HasAttributeScaling()
    {
        if (scaling == 0 && levelScaling == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public class AbilityValue
{
    public AbilityObject sourceAbility;
    public bool isPassive = false;
    public bool isEffect = false;

    public float value;
    public AbilitySchool school;
    public AbilityType abilityType;
    public bool cannotCrit;
    public bool cannotMiss;

    public Unit caster;
    public Unit target;

    public int accuracy;
    public int critChance;
    public int critDamage;

    public bool isGlancing;
    public bool isCritical;

    public bool isUnitTrigger;
    public PassiveAbility ignorePassive = null;


    public Color color;

    public AbilityValue(AbilityObject _sourceAbility, bool _isPassive, bool _isEffect, float _value, AbilitySchool _school, AbilityType _abilityType, bool _cannotCrit, bool _cannotMiss, Unit _caster, Unit _target, Color _color, bool _isUnitTrigger, PassiveAbility _ignorePassive)
    {
        sourceAbility = _sourceAbility;
        isPassive = _isPassive;
        isEffect = _isEffect;

        value = _value;// GetVarianceValue(_value);
        school = _school;
        abilityType = _abilityType;
        cannotCrit = _cannotCrit;
        cannotMiss = _cannotMiss;   

        caster = _caster;
        target = _target;

        accuracy = caster.statsManager.GetAttributeValue(AttributeType.Accuracy);
        critChance = caster.statsManager.GetAttributeValue(AttributeType.Crit);
        critDamage = caster.statsManager.GetAttributeValue(AttributeType.CritMultiplier);

        SetGlancingChance();
        SetCriticalChance();

        isUnitTrigger = _isUnitTrigger;
        ignorePassive = _ignorePassive;

        color = _color;
    }

    public void SetGlancingChance()
    {
        if (cannotMiss)
            isGlancing = false;
        else
            isGlancing = school != AbilitySchool.Healing && school != AbilitySchool.Sacrificial ? Random.Range(0, 100) > accuracy : false;
    }

    public void SetCriticalChance()
    {
        if (cannotCrit || school == AbilitySchool.Sacrificial)
            isCritical = false;
        else
            isCritical = Random.Range(0, 100) < critChance;
    }

    private float GetVarianceValue(float value)
    {
        float upperRange = value * (1 + AbilitySource.valueRangeVariance);
        float lowerRange = value * (1 - AbilitySource.valueRangeVariance);

        return Random.Range(lowerRange, upperRange);
    }

    public void Trigger(bool flatHeal = false)
    {
        if (!flatHeal)
        {
            if (isGlancing)
                value *= 0.5f;
            else
                if (isCritical)
                    value = CriticalValue(value, critDamage);

            if (value < 0)
                value = 0;
        }
        else
        {
            isGlancing = false;
            isCritical = false;
        }

        if (school != AbilitySchool.Healing)
        {
            caster.statsManager.OnCauseUnitEvent?.Invoke(this);
            target.statsManager.TakeDamage(this, true);
        }
        else
        {
            caster.statsManager.OnCauseUnitEvent?.Invoke(this);
            target.statsManager.ReceiveHealing(this);
        }
    }

    private float CriticalValue(float value, int critMultiplier)
    {
        return value * (Mathf.Max(0, (float)critMultiplier) / 100);
    }

    public int Rounded()
    {
        return (int)System.Math.Round(value, System.MidpointRounding.AwayFromZero);
    }
}