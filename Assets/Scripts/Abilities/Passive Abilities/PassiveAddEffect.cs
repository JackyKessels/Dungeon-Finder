using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public ActivationMoment activationMoment;

    [Header("[ On Activate Events ]")]
    public List<CastActiveAbility> castActiveAbilities;
    [Space(10)]
    public List<EffectObject> effects;
    [Space(10)]
    public List<PassiveAbility> passives;
    [Space(10)]

    public AbilityTargets targets;

    [Header("[ Health Threshold ]")]
    public float healthPercentage;

    [Header("[ Attribute ]")]
    public AttributeType attributeType;
    public int attributeValue;

    [Header("[ Round Cooldown ]")]
    public int cooldown;
    public bool castFirstRound = false;

    public override void ActivatePassive(Unit unit)
    {
        if (activationMoment == ActivationMoment.StartBattle)
        {
            unit.OnStartBattle += TriggerPassiveEvent;
        }
        else if (activationMoment == ActivationMoment.BelowHealthThreshold || activationMoment == ActivationMoment.AboveHealthThreshold)
        {
            // Only remove passives and effects on Self
            unit.OnStartBattle += CheckHealthThreshold;
            unit.statsManager.OnReceiveUnitEvent += TriggerHealthThreshold;
        }
        else if (activationMoment == ActivationMoment.OnDeath)
        {
            unit.statsManager.OnDeathEvent += TriggerOnDeath;
        }
        else if (activationMoment == ActivationMoment.RoundStart)
        {
            unit.OnRoundStart += TriggerPassiveEvent;
        }
        else if (activationMoment == ActivationMoment.BelowAttributeValue || activationMoment == ActivationMoment.AboveAttributeValue)
        {
            // Only remove passives and effects on Self
            unit.OnStartBattle += TriggerAttributeValue;
            unit.statsManager.OnReceiveUnitEvent += TriggerAttributeValue;
            unit.OnRoundStart += TriggerAttributeValue;
            // TODO: Fix the infinite loop
            //unit.statsManager.OnAttributesChanged += TriggerAttributeValue;
        }
        else if (activationMoment == ActivationMoment.RoundCooldown)
        {
            unit.OnRoundStart += CheckCurrentRound;
        }
    }

    public override void DeactivatePassive(Unit unit)
    {
        if (activationMoment == ActivationMoment.StartBattle)
            unit.OnStartBattle -= TriggerPassiveEvent;
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
            unit.OnRoundStart -= TriggerPassiveEvent;
        }
        else if (activationMoment == ActivationMoment.BelowAttributeValue || activationMoment == ActivationMoment.AboveAttributeValue)
        {
            unit.OnStartBattle -= TriggerAttributeValue;
            unit.statsManager.OnReceiveUnitEvent -= TriggerAttributeValue;
            unit.OnRoundStart -= TriggerAttributeValue;
            // TODO: Fix the infinite loop
            //unit.statsManager.OnAttributesChanged -= TriggerAttributeValue;
        }
        else if (activationMoment == ActivationMoment.RoundCooldown)
        {
            unit.OnRoundStart -= CheckCurrentRound;
        }
    }

    private void CastActiveAbility(Unit caster, Unit target)
    {
        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                castActiveAbilities[i].CastAbility(caster, target);
            }
        }
    }

    private void AddEffects(Unit caster, Unit target, bool checkHasEffect = false)
    {
        if (effects.Count == 0)
        {
            return;
        }

        int level = caster.spellbook.GetAbilityLevel(this);

        foreach (EffectObject effectObject in effects)
        {
            if (!checkHasEffect || !target.effectManager.HasEffect(effectObject))
            {
                EffectManager.ApplyEffect(effectObject, caster, target, level, this);
            }
        }
    }

    private void RemoveEffects(Unit caster, bool checkHasEffect = false)
    {
        if (effects.Count == 0)
        {
            return;
        }

        foreach (EffectObject effectObject in effects)
        {
            if (!checkHasEffect || caster.effectManager.HasEffect(effectObject))
            {
                caster.effectManager.RemoveEffect(effectObject);
            }
        }
    }

    private void AddPassives(Unit caster, Unit target, bool checkHasPassive = false)
    {
        if (passives.Count == 0)
        {
            return;
        }

        int level = caster.spellbook.GetAbilityLevel(this);

        var passivesList = checkHasPassive ? passives.Where(p => !caster.spellbook.HasPassiveAbility(p, level)).ToList() : passives;

        target.spellbook.LearnPassives(passivesList.ConvertAll(p => (p, level)));
    }

    private void RemovePassives(Unit caster, bool checkHasPassive = false)
    {
        if (passives.Count == 0)
        {
            return;
        }

        int level = caster.spellbook.GetAbilityLevel(this);

        var passivesList = checkHasPassive ? passives.Where(p => !caster.spellbook.HasPassiveAbility(p, level)).ToList() : passives;

        caster.spellbook.UnlearnPassives(passivesList.ConvertAll(p => (p, level)));
    }

    private void TriggerPassiveEvent(Unit caster)
    {
        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);

        foreach (Unit target in AbilityUtilities.GetAbilityTargets(targets, caster))
        {
            CastActiveAbility(caster, target);
            AddEffects(caster, target);
            AddPassives(caster, target);
            ObjectUtilities.CreateSpecialEffects(targetSpecialEffects, target);
        }
    }

    private void ApplySelfEvent(Unit caster)
    {
        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);
        AddEffects(caster, caster, true);
        AddPassives(caster, caster, true);
    }

    private void ReverseSelfEvent(Unit caster)
    {
        RemoveEffects(caster, true);
        RemovePassives(caster, true);
    }

    private void TriggerHealthThreshold(AbilityValue abilityValue)
    {
        Unit attacked = abilityValue.target;

        CheckHealthThreshold(attacked);
    }

    private void TriggerAttributeValue(AbilityValue abilityValue)
    {
        Unit attacked = abilityValue.target;

        CheckAttributeValue(attacked, attributeType);
    }

    private void TriggerAttributeValue(Unit unit)
    {
        CheckAttributeValue(unit, attributeType);
    }

    private void TriggerAttributeValue(Unit unit, AttributeType attributeType)
    {
        CheckAttributeValue(unit, attributeType);
    }

    private void TriggerOnDeath(AbilityValue abilityValue)
    {
        Unit killed = abilityValue.target;

        TriggerPassiveEvent(killed);
    }

    private void CheckHealthThreshold(Unit unit)
    {
        if (activationMoment == ActivationMoment.BelowHealthThreshold)
        {
            if (unit.statsManager.GetHealthPercentage() <= (healthPercentage / 100) && !unit.effectManager.HasEffect(effects[0]))
            {
                ApplySelfEvent(unit);
            }

            if (unit.statsManager.GetHealthPercentage() > (healthPercentage / 100) && unit.effectManager.HasEffect(effects[0]))
            {
                ReverseSelfEvent(unit);
            }
        }
        else if (activationMoment == ActivationMoment.AboveHealthThreshold) 
        {
            if (unit.statsManager.GetHealthPercentage() >= (healthPercentage / 100) && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerPassiveEvent(unit);
            }

            if (unit.statsManager.GetHealthPercentage() < (healthPercentage / 100) && unit.effectManager.HasEffect(effects[0]))
            {
                ReverseSelfEvent(unit);
            }
        }
    }

    private void CheckAttributeValue(Unit unit, AttributeType attributeType)
    {
        if (activationMoment == ActivationMoment.BelowAttributeValue)
        {
            if (unit.statsManager.GetAttributeValue(attributeType) <= attributeValue && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerPassiveEvent(unit);
            }
            else if (unit.statsManager.GetAttributeValue(attributeType) > attributeValue && unit.effectManager.HasEffect(effects[0]))
            {
                ReverseSelfEvent(unit);
            }
        }
        else if (activationMoment == ActivationMoment.AboveAttributeValue)
        {
            if (unit.statsManager.GetAttributeValue(attributeType) >= attributeValue && !unit.effectManager.HasEffect(effects[0]))
            {
                TriggerPassiveEvent(unit);
            }
            else if (unit.statsManager.GetAttributeValue(attributeType) < attributeValue && unit.effectManager.HasEffect(effects[0]))
            {
                ReverseSelfEvent(unit);
            }
        }
    }

    private void CheckCurrentRound(Unit unit)
    {
        if (activationMoment == ActivationMoment.RoundCooldown)
        {
            if (castFirstRound)
            {
                // If CD = 3
                // 1, 4, 7
                if ((BattleManager.Instance.round == 1) ||
                    (BattleManager.Instance.round % (cooldown + 1) == 1))
                {
                    TriggerPassiveEvent(unit);
                }
            }
            else
            {
                // If CD = 3
                // 3, 6, 9
                if (BattleManager.Instance.round % (cooldown + 1) == 0)
                {
                    TriggerPassiveEvent(unit);
                }
            }
        }
    }

    public override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = base.ParseDescription(s, tooltipInfo);

        temp = AbilityTooltipHandler.ParseEffectTooltips(temp, "effect", effects, tooltipInfo);

        for (int i = 0; i < castActiveAbilities.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseCastAbility(temp, string.Format("<castInfo{0}>", i + 1), string.Format("<castTooltip{0}>", i + 1), tooltipInfo, castActiveAbilities[i].activeAbility);
            temp = AbilityTooltipHandler.ParseCastAbilityEffectiveness(temp, string.Format("<castEffect{0}>", i + 1), castActiveAbilities[i]);
        }

        temp = ThresholdParse(temp, "<threshold>");

        temp = RoundCooldown(temp, "<cooldown>");

        return temp;
    }

    private string ThresholdParse(string s, string check)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.NonScalingColor() + ">{0}</color>");

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
