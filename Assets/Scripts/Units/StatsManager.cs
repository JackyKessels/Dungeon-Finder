using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType
{
    // General Stats
    Health,
    Power,
    Wisdom,
    Armor,
    Resistance,
    Vitality,
    Speed,
    Accuracy,
    Crit,

    // School multipliers
    HealingMultiplier,
    PhysicalMultiplier,
    FireMultiplier,
    IceMultiplier,
    NatureMultiplier,
    ArcaneMultiplier,
    HolyMultiplier,
    ShadowMultiplier,
    CritMultiplier
}

public enum AttributeValue
{
    baseValue,
    bonusValue,
    multiplier
}

public delegate void UnitEvent(AbilityValue abilityValue);

public delegate void AttributesEvent(Unit unit, AttributeType attributeType);

[System.Serializable]
public class StatsManager
{
    public int currentHealth;
    public bool isDead = false;
    [SerializeField] private List<Attribute> attributes;

    public UnitEvent OnReceiveUnitEvent;
    public UnitEvent OnCauseUnitEvent;
    public UnitEvent OnDeathEvent;
    public AttributesEvent OnAttributesChanged;

    private bool deathEventTriggered = false;

    private Unit unit;

    public StatsManager(Unit u, UnitObject data)
    {
        unit = u;
        Setup(data);

        OnReceiveUnitEvent += BreakIncapacitate;
        OnAttributesChanged += AttributesChanged;
    }

    public void Setup(UnitObject data)
    {
        attributes = new List<Attribute>();

        foreach(AttributeType type in (AttributeType[])System.Enum.GetValues(typeof(AttributeType)))
        {
            attributes.Add(new Attribute(type, 3));   
        }

        foreach(Attribute a in attributes)
        {
            switch (a.attributeType)
            {
                // General Stats
                case AttributeType.Health:
                    a.baseValue = data.baseHealth;
                    currentHealth = a.baseValue;
                    break;
                case AttributeType.Power:
                    a.baseValue = data.basePower;
                    break;
                case AttributeType.Wisdom:
                    a.baseValue = data.baseWisdom;
                    break;
                case AttributeType.Armor:
                    a.baseValue = data.baseArmor;
                    break;
                case AttributeType.Resistance:
                    a.baseValue = data.baseResistance;
                    break;
                case AttributeType.Vitality:
                    a.baseValue = data.baseVitality;
                    break;
                case AttributeType.Speed:
                    a.baseValue = data.speed;
                    break;
                case AttributeType.Accuracy:
                    a.baseValue = data.accuracy;
                    break;
                case AttributeType.Crit:
                    a.baseValue = data.criticalHit;
                    break;

                // Damage school multipliers
                case AttributeType.HealingMultiplier:
                    a.baseValue = data.healingMultiplier;
                    break;
                case AttributeType.PhysicalMultiplier:
                    a.baseValue = data.crushingMultiplier;
                    break;
                case AttributeType.FireMultiplier:
                    a.baseValue = data.fireMultiplier;
                    break;
                case AttributeType.IceMultiplier:
                    a.baseValue = data.iceMultiplier;
                    break;
                case AttributeType.NatureMultiplier:
                    a.baseValue = data.natureMultiplier;
                    break;
                case AttributeType.ArcaneMultiplier:
                    a.baseValue = data.arcaneMultiplier;
                    break;
                case AttributeType.HolyMultiplier:
                    a.baseValue = data.holyMultiplier;
                    break;
                case AttributeType.ShadowMultiplier:
                    a.baseValue = data.shadowMultiplier;
                    break;
                case AttributeType.CritMultiplier:
                    a.baseValue = data.critMultiplier;
                    break;
            }
        }
    }

    public void ModifyAttribute(AttributeType attributeType, AttributeValue attributeValue, float value)
    {

        float percentage = GetHealthPercentage();      

        switch (attributeValue)
        {
            case AttributeValue.baseValue:
                {        
                    GetAttribute(attributeType).baseValue += (int)value;
                }
                break;
            case AttributeValue.bonusValue:
                {
                    GetAttribute(attributeType).bonusValue += (int)value;
                }
                break;
            case AttributeValue.multiplier:
                {
                    GetAttribute(attributeType).multiplier += value;
                }
                break;
            default:
                return;
        }

        if (attributeType == AttributeType.Health)
        {
            SetHealthPercentage(percentage);
        }

        OnAttributesChanged?.Invoke(unit, attributeType);
    }

    public void AttributesChanged(Unit unit, AttributeType attributeType)
    {
        if (attributeType == AttributeType.Speed)
        {
            QueueManager.Instance.Refresh(false);
        }

        CapCurrentHealth();

        Debug.Log(unit.name + "'s " + attributeType + " has been changed to " + GetAttributeValue(attributeType) + ".");
    }

    public Attribute GetAttribute(AttributeType type)
    {
        return attributes[(int)type];
    }

    public List<Attribute> GetAttributes()
    {
        return attributes;
    }

    public int GetAttributeValue(int type)
    {
        return attributes[type].GetTotalValue();
    }

    public int GetAttributeValue(AttributeType type)
    {
        return attributes[(int)type].GetTotalValue();
    }

    public int GetReductionValue(ReductionType type)
    {
        switch (type)
        {
            case ReductionType.None:
                return 0;
            case ReductionType.Armor:
                return attributes[(int)AttributeType.Armor].GetTotalValue();
            case ReductionType.Resistance:
                return attributes[(int)AttributeType.Resistance].GetTotalValue();
            default:
                return 0;
        }
    }

    public void TakeDamage(AbilityValue abilityValue, bool allowDamageTransfer)
    {
        Effect damageTransferEffect = unit.effectManager.GetHighestDamageTransfer();

        if (damageTransferEffect != null && allowDamageTransfer)
        {
            EffectDamageTransfer damageTransfer = damageTransferEffect.effectObject as EffectDamageTransfer;

            AbilityValue transferValue = new AbilityValue(abilityValue.sourceAbility, false, true, abilityValue.value * damageTransfer.percentage, abilityValue.school, abilityValue.abilityType, abilityValue.cannotCrit, abilityValue.cannotMiss, abilityValue.target, damageTransferEffect.caster, abilityValue.color, abilityValue.isUnitTrigger, abilityValue.ignorePassive);

            transferValue.value = unit.statsManager.CalculateMitigatedDamage(transferValue.value, GeneralUtilities.GetReductionType(transferValue.school));

            damageTransferEffect.caster.statsManager.TakeDamage(transferValue, false);

            abilityValue.value *= (1 - damageTransfer.percentage);
        }

        currentHealth -= abilityValue.Rounded();

        // Show damage dealt
        FCTData fctData = new FCTData(true, unit, abilityValue.Rounded().ToString(), abilityValue.isGlancing, abilityValue.isCritical, abilityValue.color, abilityValue.color);
        unit.fctHandler.AddToFCTQueue(fctData);

        OnReceiveUnitEvent?.Invoke(abilityValue);

        CheckHealthStatus(abilityValue);
    }

    private void BreakIncapacitate(AbilityValue abilityValue)
    {
        unit.effectManager.BreakCrowdControl(CrowdControlType.Incapacitate);
    }

    public void ReceiveHealing(AbilityValue abilityValue)
    {
        FCTData fctData = new FCTData(true, unit, abilityValue.Rounded().ToString(), abilityValue.isGlancing, abilityValue.isCritical, abilityValue.color, abilityValue.color);
        unit.fctHandler.AddToFCTQueue(fctData);

        currentHealth += abilityValue.Rounded();

        OnReceiveUnitEvent?.Invoke(abilityValue);

        CheckHealthStatus(abilityValue);
    }



    // Pretty much check if dead
    public void CheckHealthStatus(AbilityValue abilityValue) 
    {
        if (isDead)
            return;

        if (currentHealth <= 0)
        {
            isDead = true;
            currentHealth = 0;

            if (BattleManager.Instance.activeBattle)
            {
                if (!deathEventTriggered)
                {
                    OnDeathEvent?.Invoke(abilityValue);
                    deathEventTriggered = true;
                }

                BattleManager.Instance.RemoveUnitFromBattle(unit);
                QueueManager.Instance.SetIcon(unit);
                EffectManager.RemoveAuras();
            }
        }
        else
        {
            CapCurrentHealth();

            isDead = false;
        }
    }

    public void CapCurrentHealth()
    {
        if (currentHealth > GetAttributeValue(AttributeType.Health))
        {
            currentHealth = GetAttributeValue(AttributeType.Health);
        }
    }

    public void IncreaseAttribute(Attribute attributeIncrease)
    {
        foreach (Attribute a in attributes)
        {
            if (a.attributeType == attributeIncrease.attributeType)
            {
                a.baseValue += attributeIncrease.baseValue;

                if (a.attributeType == AttributeType.Health && !isDead)
                    currentHealth += attributeIncrease.baseValue;
            }
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / (float)GetAttributeValue(AttributeType.Health);
    }

    public void SetHealthPercentage(float percentage)
    {
        currentHealth = GeneralUtilities.RoundFloat(percentage * (float)GetAttributeValue(AttributeType.Health), 0);

        CapCurrentHealth();
    }

    public float CalculateMitigatedDamage(float value, ReductionType reductionType)
    {
        float mitigationValue = GetReductionValue(reductionType);

        float damageMultiplier = GeneralUtilities.DefensiveReductionValue_League_Tweaked(mitigationValue);

        float reducedDamage = value * damageMultiplier;

        if (reducedDamage < 0)
            reducedDamage = 0;

        return reducedDamage;
    }

    public float CalculateVitalityHealing(float value)
    {
        float targetVitality = GetAttribute(AttributeType.Vitality).GetTotalValue();

        return value * (1 + targetVitality / 100);
    }
}