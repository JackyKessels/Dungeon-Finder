using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivationMoment
{
    StartBattle,
    BelowHealthThreshold,
    AboveHealthThreshold,
    OnDeath,
    RoundStart,
    BelowAttributeValue,
    AboveAttributeValue,
    RoundCooldown
}

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Unit/Ability Object/Passive Add Effect")]
public class PassiveAddEffect : PassiveAbility
{
    [Header("[ Add Effect Functionality ]")]
    public List<EffectObject> effects;
    public ActivationMoment activationMoment;
    public AbilityTargets targets;

    [Header("[ Health Threshold ]")]
    public float healthPercentage;

    [Header("[ Attribute ]")]
    public AttributeType attributeType;
    public int attributeValue;

    [Header("[ Round Cooldown ]")]
    public int cooldown;

    public override void ActivatePassive(Unit unit)
    {
        if (activationMoment == ActivationMoment.StartBattle)
            unit.OnStartBattle += TriggerAddEffect;
        else if (activationMoment == ActivationMoment.BelowHealthThreshold || activationMoment == ActivationMoment.AboveHealthThreshold)
        {
            unit.OnStartBattle += CheckHealthThreshold;
            unit.statsManager.OnReceiveUnitEvent += TriggerHealthThreshold;
        }
        else if (activationMoment == ActivationMoment.OnDeath)
        {
            unit.statsManager.OnDeathEvent += TriggerOnDeath;
        }
        else if (activationMoment == ActivationMoment.RoundStart)
        {
            unit.OnRoundStart += TriggerAddEffect;
        }
        else if (activationMoment == ActivationMoment.BelowAttributeValue || activationMoment == ActivationMoment.AboveAttributeValue)
        {
            unit.OnStartBattle += CheckAttributeValue;
            unit.statsManager.OnReceiveUnitEvent += TriggerAttributeValue;
            unit.OnRoundStart += CheckAttributeValue;
        }
        else if (activationMoment == ActivationMoment.RoundCooldown)
        {
            unit.OnRoundStart += CheckCurrentRound;
        }
    }

    public override void DeactivatePassive(Unit unit)
    {
        if (activationMoment == ActivationMoment.StartBattle)
            unit.OnStartBattle -= TriggerAddEffect;
        else if (activationMoment == ActivationMoment.BelowHealthThreshold || activationMoment == ActivationMoment.AboveHealthThreshold)
        {
            unit.OnStartBattle -= CheckHealthThreshold;
            unit.statsManager.OnReceiveUnitEvent -= TriggerHealthThreshold;
        }
        else if (activationMoment == ActivationMoment.OnDeath)
        {
            unit.statsManager.OnDeathEvent -= TriggerOnDeath;
        }
        else if (activationMoment == ActivationMoment.RoundStart)
        {
            unit.OnRoundStart -= TriggerAddEffect;
        }
        else if (activationMoment == ActivationMoment.BelowAttributeValue || activationMoment == ActivationMoment.AboveAttributeValue)
        {
            unit.OnStartBattle -= CheckAttributeValue;
            unit.statsManager.OnReceiveUnitEvent -= TriggerAttributeValue;
            unit.OnRoundStart -= CheckAttributeValue;
        }
        else if (activationMoment == ActivationMoment.RoundCooldown)
        {
            unit.OnRoundStart -= CheckCurrentRound;
        }
    }

    private void AddEffect(Unit caster, Unit target)
    {
        foreach (EffectObject effectObject in effects)
        {
            EffectManager.ApplyEffect(effectObject, caster, target, 1, this);
        }
    }

    private void RemoveEffect(Unit target)
    {
        foreach (EffectObject effectObject in effects)
        {
            target.effectManager.RemoveEffect(effectObject);
        }
    }

    private void TriggerAddEffect(Unit caster)
    {
        foreach (Unit unit in AbilityUtilities.GetAbilityTargets(targets, caster))
        {
            AddEffect(caster, unit);
        }
    }

    private void TriggerHealthThreshold(AbilityValue abilityValue)
    {
        Unit attacked = abilityValue.target;

        CheckHealthThreshold(attacked);
    }

    private void TriggerAttributeValue(AbilityValue abilityValue)
    {
        Unit attacked = abilityValue.target;

        CheckAttributeValue(attacked);
    }

    private void TriggerOnDeath(AbilityValue abilityValue)
    {
        Unit killed = abilityValue.target;

        TriggerAddEffect(killed);
    }

    private void CheckHealthThreshold(Unit unit)
    {
        if (activationMoment == ActivationMoment.BelowHealthThreshold)
        {
            if (unit.statsManager.GetHealthPercentage() <= (healthPercentage / 100) && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerAddEffect(unit);
            }

            if (unit.statsManager.GetHealthPercentage() > (healthPercentage / 100) && unit.effectManager.HasEffect(effects[0]))
            {
                RemoveEffect(unit);
            }
        }
        else if (activationMoment == ActivationMoment.AboveHealthThreshold) 
        {
            if (unit.statsManager.GetHealthPercentage() >= (healthPercentage / 100) && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerAddEffect(unit);
            }

            if (unit.statsManager.GetHealthPercentage() < (healthPercentage / 100) && unit.effectManager.HasEffect(effects[0]))
            {
                RemoveEffect(unit);
            }
        }
    }

    private void CheckAttributeValue(Unit unit)
    {
        if (activationMoment == ActivationMoment.BelowAttributeValue)
        {
            if (unit.statsManager.GetAttributeValue(attributeType) <= attributeValue && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerAddEffect(unit);
            }
            else if (unit.statsManager.GetAttributeValue(attributeType) > attributeValue && unit.effectManager.HasEffect(effects[0]))
            {
                RemoveEffect(unit);
            }
        }
        else if (activationMoment == ActivationMoment.AboveAttributeValue)
        {
            if (unit.statsManager.GetAttributeValue(attributeType) >= attributeValue && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerAddEffect(unit);
            }
            else if (unit.statsManager.GetAttributeValue(attributeType) < attributeValue && unit.effectManager.HasEffect(effects[0]))
            {
                RemoveEffect(unit);
            }
        }
    }

    private void CheckCurrentRound(Unit unit)
    {
        if (activationMoment == ActivationMoment.RoundCooldown)
        {
            if (BattleManager.Instance.round % (cooldown + 1) == 0)
            {
                TriggerAddEffect(unit);
            }
        }
    }

    public override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = base.ParseDescription(s, tooltipInfo);

        temp = AbilityTooltipHandler.ParseAllEffectTooltips(temp, tooltipInfo, effects, new List<EffectObject>());

        temp = AbilityTooltipHandler.InsertRed(temp);

        temp = ThresholdParse(temp, "<threshold>");

        temp = RoundCooldown(temp, "<cooldown>");

        return temp;
    }

    private string ThresholdParse(string s, string check)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(AttributeType.Health) + ">{0}</color>");

            return string.Format(temp, healthPercentage);
        }

        return temp;
    }

    private string RoundCooldown(string s, string check)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

            return string.Format(temp, cooldown + 1);
        }

        return temp;
    }
}
