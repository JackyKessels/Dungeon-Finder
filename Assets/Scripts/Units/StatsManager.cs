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
    public static readonly int levelUpHealth = 5;
    public static readonly float levelScaling = 0.2f;

    public int currentHealth;
    public bool isDead = false;
    public bool isInvulnerable = false;

    [SerializeField] private List<Attribute> attributes;

    public UnitEvent OnReceiveUnitEvent;
    public UnitEvent OnCauseUnitEvent;
    public UnitEvent OnDeathEvent;
    public AttributesEvent OnAttributesChanged;

    private bool deathEventTriggered = false;

    private Unit unit;



    public StatsManager(Unit u, UnitObject data, int level = 1)
    {
        unit = u;
        Setup(data, level);

        OnReceiveUnitEvent += BreakIncapacitate;
        OnAttributesChanged += AttributesChanged;
    }

    public void Setup(UnitObject data, int level)
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
                    a.baseValue = GeneralUtilities.RoundFloat(data.baseHealth + (level - 1) * (data.baseHealth * levelScaling), 0);
                    currentHealth = a.baseValue;
                    break;
                case AttributeType.Power:
                    a.baseValue = GeneralUtilities.RoundFloat(data.basePower + (level - 1) * (data.basePower * levelScaling), 0);
                    break;
                case AttributeType.Wisdom:
                    a.baseValue = GeneralUtilities.RoundFloat(data.baseWisdom + (level - 1) * (data.baseWisdom * levelScaling), 0);
                    break;
                case AttributeType.Armor:
                    a.baseValue = data.armor;
                    break;
                case AttributeType.Resistance:
                    a.baseValue = data.resistance;
                    break;
                case AttributeType.Vitality:
                    a.baseValue = data.vitality;
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

        //Debug.Log(unit.name + "'s " + attributeType + " has been changed to " + GetAttributeValue(attributeType) + ".");
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

    public int GetAttributeValue(TargetAttribute targetAttribute)
    {
        switch (targetAttribute)
        {
            case TargetAttribute.CurrentHealth:
                return currentHealth;
            case TargetAttribute.MaximumHealth:
                return GetAttributeValue(AttributeType.Health);
            case TargetAttribute.Power:
                return GetAttributeValue(AttributeType.Power);
            case TargetAttribute.Wisdom:
                return GetAttributeValue(AttributeType.Wisdom);
            case TargetAttribute.Armor:
                return GetAttributeValue(AttributeType.Armor);
            case TargetAttribute.Resistance:
                return GetAttributeValue(AttributeType.Resistance);
            case TargetAttribute.Speed:
                return GetAttributeValue(AttributeType.Speed);
            default:
                return 0;
        }
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
        if (isInvulnerable)
        {
            //Color color = new Color(1f, 1f, 1f);

            //FCTData fctData = new FCTData(false, unit, "Invulnerable", color);
            //unit.fctHandler.AddToFCTQueue(fctData);
        }
        else if (unit.effectManager.HasDamageImmunity())
        {
            OnReceiveUnitEvent?.Invoke(abilityValue);
        }
        else
        {
            ApplyTransferDamage(abilityValue, allowDamageTransfer);

            currentHealth -= abilityValue.Rounded();

            // Show damage dealt
            FCTData fctData = new FCTData(true, unit, abilityValue.Rounded().ToString(), abilityValue.isGlancing, abilityValue.isCritical, abilityValue.color, abilityValue.color);
            unit.fctHandler.AddToFCTQueue(fctData);

            OnReceiveUnitEvent?.Invoke(abilityValue);

            CheckHealthStatus(abilityValue);
        }
    }

    private void ApplyTransferDamage(AbilityValue abilityValue, bool allowDamageTransfer)
    {
        if (!allowDamageTransfer)
        {
            return;
        }

        Effect damageTransferEffect = unit.effectManager.GetHighestDamageTransfer();

        if (damageTransferEffect != null)
        {
            EffectDamageTransfer damageTransfer = damageTransferEffect.effectObject as EffectDamageTransfer;

            AbilityValue transferValue = new AbilityValue(abilityValue.sourceAbility, abilityValue.sourceLevel, false, true, abilityValue.value * damageTransfer.percentage, abilityValue.school, abilityValue.abilityType, abilityValue.cannotCrit, abilityValue.cannotMiss, abilityValue.target, damageTransferEffect.caster, abilityValue.color, false, abilityValue.ignorePassives);

            transferValue.value = unit.statsManager.CalculateMitigatedDamage(transferValue.value, GeneralUtilities.GetReductionType(transferValue.school));

            damageTransferEffect.caster.statsManager.TakeDamage(transferValue, false);

            abilityValue.value *= (1 - damageTransfer.percentage);
        }
    }

    public void TakeDamage(AbilitySchool school, int amount)
    {
        AbilitySource abilitySource = new AbilitySource(school, amount);
        abilitySource.TriggerSource(null, 1, false, false, unit, unit, 1, true, 1, AbilityType.Assault);
    }

    public void TakeDamage(AbilitySource abilitySource, bool isUnitTrigger)
    {
        abilitySource.TriggerSource(null, 1, false, false, unit, unit, 1, isUnitTrigger, 1, AbilityType.Assault);
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

    public void LevelUp()
    {
        ModifyAttribute(AttributeType.Health, AttributeValue.bonusValue, levelUpHealth);
    }
}