using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityCondition
{
    None,
    CasterAboveThreshold,
    CasterBelowThreshold,
    TargetAboveThreshold,
    TargetBelowThreshold,
    CriticalHit
}

public enum ThresholdType
{
    None,
    CurrentHealthPercentage,
    Armor,
    Resistance,
    Speed
}

public enum AbilityModification
{
    None,
    IncreasedBaseline,
    AttributeBasedIncrease,
    BonusAbilitySource,
    ConditionalEffect
}

public enum AttributeIncrease
{
    None,
    Accuracy,
    CritChance,
    CritDamage,
    TotalDamagePercentage
}

public enum AttributeBase
{
    None,
    CurrentHealthPercentage,
    MissingHealthPercentage,
    Armor,
    Resistance
}

[System.Serializable]
public class AbilityModifier
{
    [Header("[ Modifier Check ]")]
    public bool hasModifier = false;
    public AbilityModification modification;

    [Header("[ Conditions ]")]
    public AbilityCondition condition = AbilityCondition.None;
    public ThresholdType thresholdType;
    public int conditionThreshold;

    [Header("[ Increased Baseline ]")]
    public AttributeIncrease increasedAttribute;
    public int increaseValue;

    [Header("[ Attribute Based Increase ]")]
    [Tooltip("true = caster, false = target")]
    public bool casterBased = true;
    public AttributeBase attributeBase;
    public float percentageIncrease;

    [Header("[ Bonus Ability Source ]")]
    public BonusAbilitySource bonusAbilitySource;

    [Header("[ Conditional Effect ]")]
    public bool effectOnTarget = true;
    public EffectObject conditionalEffect;

    public void TriggerModifier(AbilityValue _value, int level = 1)
    {
        if (!hasModifier)
            return;

        if (PassCondition(_value))
        {
            if (modification == AbilityModification.IncreasedBaseline)
            {
                if (increasedAttribute == AttributeIncrease.CritChance)
                {
                    _value.critChance += increaseValue;
                    _value.SetCriticalChance();
                }
                else if (increasedAttribute == AttributeIncrease.CritDamage)
                    _value.critDamage += increaseValue;
                else if (increasedAttribute == AttributeIncrease.Accuracy)
                {
                    _value.accuracy += increaseValue;
                    _value.SetGlancingChance();
                }
                else if (increasedAttribute == AttributeIncrease.TotalDamagePercentage)
                {
                    _value.value *= (1 + ((float)increaseValue / 100));
                }
            }
            else if (modification == AbilityModification.AttributeBasedIncrease)
            {
                Unit attributeBasedUnit = casterBased ? _value.caster : _value.target;

                float attributeValue = 0;

                if (attributeBase == AttributeBase.CurrentHealthPercentage)
                {
                    float currentHealth = attributeBasedUnit.statsManager.currentHealth;
                    float maxHealth = attributeBasedUnit.statsManager.GetAttributeValue(AttributeType.Health);

                    float healthPercentage = (currentHealth / maxHealth) * 100;

                    attributeValue = healthPercentage;
                }
                else if (attributeBase == AttributeBase.MissingHealthPercentage)
                {
                    float currentHealth = attributeBasedUnit.statsManager.currentHealth;
                    float maxHealth = attributeBasedUnit.statsManager.GetAttributeValue(AttributeType.Health);
                    float missingHealth = maxHealth - currentHealth;

                    float healthPercentage = (missingHealth / maxHealth) * 100;

                    attributeValue = healthPercentage;
                }
                else if (attributeBase == AttributeBase.Armor)
                {
                    attributeValue = attributeBasedUnit.statsManager.GetAttributeValue(AttributeType.Armor);
                }
                else if (attributeBase == AttributeBase.Resistance)
                {
                    attributeValue = attributeBasedUnit.statsManager.GetAttributeValue(AttributeType.Resistance);
                }

                _value.value *= (1 + (attributeValue * percentageIncrease / 100));
            }
            else if (modification == AbilityModification.BonusAbilitySource)
            {
                bonusAbilitySource.GetAbilitySource().TriggerSource(_value.sourceAbility, _value.isPassive, _value.isEffect, _value.caster, _value.target, level, 1, true, 1, AbilityType.Passive); 
            }
            else if (modification == AbilityModification.ConditionalEffect)
            {
                if (conditionalEffect == null)
                    return;

                Unit target = effectOnTarget ? _value.target : _value.caster;

                EffectManager.ApplyEffect(conditionalEffect, _value.caster, target, level, _value.sourceAbility);
            }
        }
        else
        {
            // Condition not met so do nothing
            return;
        }


    }

    private bool PassCondition(AbilityValue value)
    {
        if (condition == AbilityCondition.None)
            return true;
        else
        {
            int casterValue = GetCurrentThreshold(value.caster, thresholdType);
            int targetValue = GetCurrentThreshold(value.target, thresholdType);

            if (condition == AbilityCondition.CasterAboveThreshold)
            {
                if (casterValue >= conditionThreshold)
                    return true;
            }
            else if (condition == AbilityCondition.CasterBelowThreshold)
            {
                if (casterValue <= conditionThreshold)
                    return true;
            }
            else if (condition == AbilityCondition.TargetAboveThreshold)
            {
                if (targetValue >= conditionThreshold)
                    return true;
            }
            else if (condition == AbilityCondition.TargetBelowThreshold)
            {
                if (targetValue <= conditionThreshold)
                    return true;
            }
            else if (condition == AbilityCondition.CriticalHit)
            {
                if (value.isCritical && !value.isGlancing)
                    return true;
            }

            // No condition met
            return false;
        }
    }

    private int GetCurrentThreshold(Unit unit, ThresholdType type)
    {
        if (type == ThresholdType.CurrentHealthPercentage)
        {
            float currentHealth = unit.statsManager.currentHealth;
            float maxHealth = unit.statsManager.GetAttributeValue(AttributeType.Health);

            float healthPercentage = (currentHealth / maxHealth) * 100;

            return (int)healthPercentage;
        }
        else if (type == ThresholdType.Armor)
            return unit.statsManager.GetAttributeValue(AttributeType.Armor);
        else if (type == ThresholdType.Resistance)
            return unit.statsManager.GetAttributeValue(AttributeType.Resistance);
        else if (type == ThresholdType.Speed)
            return unit.statsManager.GetAttributeValue(AttributeType.Speed);
        else
            return 0;
    }
}

[System.Serializable]
public class BonusAbilitySource
{
    [Header("[ General Values ]")]
    public bool ignoreMultipliers;
    public bool cannotCrit;
    public bool cannotMiss;
    public List<PassiveAbility> ignorePassive;
    public AbilitySchool school;
    public AttributeType attributeType;
    public int baseValue;
    public int levelBase;
    public float scaling;
    public float levelScaling;
    public ReductionType reductionType;

    public AbilitySource GetAbilitySource()
    {
        return new AbilitySource(this);
    }
}
