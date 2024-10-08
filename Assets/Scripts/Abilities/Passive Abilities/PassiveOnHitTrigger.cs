﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OnHitTriggerType
{
    Thorns,
    Enhancement
}

public enum HitType
{
    Any,
    Critical,
    Glancing
}

public enum TargetTrigger
{
    Attacker,
    Attacked
}

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Unit/Ability Object/Passive On Hit Trigger")]
public class PassiveOnHitTrigger : PassiveAbility
{
    [Header("[ On Unit Trigger Functionality ]")]
    public OnHitTriggerType triggerType;
    public HitType hitType;
    public TargetTrigger targetTrigger;
    public AffectedAbility abilityTypeTrigger;
    public AbilityType typeRequirement;
    public EffectObject specificEffect;

    public List<AbilitySchool> schoolTriggers;
    public bool triggersPassives = true;
    public bool triggeredByPassives = true;
    public bool triggeredByEffects = true;

    [Space(10)]
    [Range(0, 100)] public int procChance;

    [Header("[ On Hit Events ]")]
    public List<AbilitySource> abilitySources;
    [Space(10)]
    public List<CastActiveAbility> castActiveAbilities;
    [Space(10)]
    [Tooltip("True = Triggers Self Effect for each target hit\nFalse = Just once")]
    public bool selfEffectsPerTarget = false;
    public List<EffectObject> selfEffects;
    public List<EffectObject> targetEffects;

    public override void ActivatePassive(Unit unit)
    {
        if (triggerType == OnHitTriggerType.Thorns)
            unit.statsManager.OnReceiveUnitEvent += TriggerReceiveEvent;
        else if (triggerType == OnHitTriggerType.Enhancement)
            unit.statsManager.OnCauseUnitEvent += TriggerCauseEvent;
    }

    public override void DeactivatePassive(Unit unit)
    {
        if (triggerType == OnHitTriggerType.Thorns)
            unit.statsManager.OnReceiveUnitEvent -= TriggerReceiveEvent;
        else if (triggerType == OnHitTriggerType.Enhancement)
            unit.statsManager.OnCauseUnitEvent -= TriggerCauseEvent;
    }

    // Thorns Effect
    private void TriggerReceiveEvent(AbilityValue abilityValue)
    {
        if (!SuccessfulTrigger(abilityValue))
            return;

        Unit attacked = abilityValue.target;
        Unit attacker = abilityValue.caster;

        if (abilitySources.Count > 0)
        {
            for (int i = 0; i < abilitySources.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    abilitySources[i].TriggerSource(this, this, attacked.spellbook.GetAbilityLevel(this), true, false, attacked, attacked, 1, triggersPassives, 1, abilityType);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(this, this, attacked.spellbook.GetAbilityLevel(this), true, false, attacked, attacker, 1, triggersPassives, 1, abilityType);
                }
            }
        }

        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    castActiveAbilities[i].CastAbility(attacked, attacked);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    castActiveAbilities[i].CastAbility(attacked, attacker);
                }
            }
        }

        EffectManager.ApplyEffects(selfEffects, attacked, attacked, 1, this);
        EffectManager.ApplyEffects(targetEffects, attacked, attacker, 1, this);

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, attacked);
        ObjectUtilities.CreateSpecialEffects(targetSpecialEffects, attacker);
    }

    // Enhancement Effect
    private void TriggerCauseEvent(AbilityValue abilityValue)
    {
        if (!SuccessfulTrigger(abilityValue))
            return;

        Unit attacker = abilityValue.caster;
        Unit attacked = abilityValue.target;

        if (abilitySources.Count > 0)
        {
            for (int i = 0; i < abilitySources.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    abilitySources[i].TriggerSource(this, this, attacker.spellbook.GetAbilityLevel(this), true, false, attacker, attacked, 1, triggersPassives, 1, abilityType);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(this, this, attacker.spellbook.GetAbilityLevel(this), true, false, attacker, attacker, 1, triggersPassives, 1, abilityType);
                }
            }
        }

        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    castActiveAbilities[i].CastAbility(attacker, attacked);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    castActiveAbilities[i].CastAbility(attacker, attacker);
                }
            }
        }

        EffectManager.ApplyEffects(selfEffects, attacker, attacker, 1, this);
        EffectManager.ApplyEffects(targetEffects, attacker, attacked, 1, this);

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, attacker);
        ObjectUtilities.CreateSpecialEffects(targetSpecialEffects, attacked);
    }

    private bool SuccessfulTrigger(AbilityValue abilityValue)
    {
        if (abilityValue.ignorePassives == null || abilityValue.ignorePassives.Contains(this))
            return false;

        if (!abilityValue.isUnitTrigger)
            return false;

        if (!triggeredByPassives && abilityValue.isPassive)
            return false;

        if (!triggeredByEffects && abilityValue.isEffect)
            return false;

        if (!IsTriggeredByHitType(abilityValue))
            return false;

        if (!IsTriggeredByAbilityType(abilityValue))
            return false;

        if (!IsTriggeredBySchool(abilityValue.school))
            return false;

        if (!IsProcced())
            return false;

        return true;
    }

    private bool IsTriggeredBySchool(AbilitySchool abilitySchool)
    {
        if (schoolTriggers.Contains(abilitySchool))
            return true;

        return false; 
    }

    private bool IsTriggeredByHitType(AbilityValue abilityValue)
    {
        if (hitType == HitType.Any)
            return true;

        if (hitType == HitType.Critical && !abilityValue.isGlancing && abilityValue.isCritical)
            return true;

        if (hitType == HitType.Glancing && abilityValue.isGlancing)
            return true;

        return false;
    }

    private bool IsTriggeredByAbilityType(AbilityValue abilityValue)
    {
        if (abilityTypeTrigger == AffectedAbility.AnyAbility)
        {
            return true;
        }

        if (abilityTypeTrigger == AffectedAbility.TypedAbility && 
            typeRequirement == abilityValue.abilityType)
        {
            return true;
        }

        if (abilityTypeTrigger == AffectedAbility.SpecificEffect &&
            abilityValue.triggerSource is EffectObject triggerEffect &&
            specificEffect == triggerEffect)
        {
            return true;
        }

        return false;
    }

    private bool IsProcced()
    {
        return Random.Range(0, 100) <= procChance;
    }

    public override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = base.ParseDescription(s, tooltipInfo);

        temp = AbilityTooltipHandler.ParseAllAbilitySourceTooltips(temp, tooltipInfo, abilitySources, "");

        for (int i = 0; i < castActiveAbilities.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseCastAbility(temp, string.Format("<castInfo{0}>", i + 1), string.Format("<castTooltip{0}>", i + 1), tooltipInfo, castActiveAbilities[i].activeAbility);
        }

        temp = AbilityTooltipHandler.ParseAllEffectTooltips(temp, tooltipInfo, selfEffects, targetEffects);

        temp = AbilityTooltipHandler.ParseProcChance(temp, "<%>", procChance);

        temp = AbilityTooltipHandler.CriticalHit(temp, "<critical>");

        temp = AbilityTooltipHandler.GlancingHit(temp, "<glancing>");

        temp = AbilityTooltipHandler.InsertRed(temp);

        temp = AbilityTooltipHandler.ParseName(temp, "<specific>", specificEffect);

        if (!triggersPassives || !triggeredByPassives || !triggeredByEffects)
            temp = AbilityTooltipHandler.TriggerPassive(temp, triggersPassives, triggeredByPassives, triggeredByEffects);

        return temp;
    }
}