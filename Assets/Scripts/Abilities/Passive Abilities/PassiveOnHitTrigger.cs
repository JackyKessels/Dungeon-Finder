using System.Collections;
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

    public List<AbilitySchool> schoolTriggers;
    public bool triggersPassives = false;
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
                    abilitySources[i].TriggerSource(this, false, attacked, attacked, attacked.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, abilityType);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(this, false, attacked, attacker, attacked.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, abilityType);
                }
            }
        }

        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                castActiveAbilities[i].CastAbility(attacked, attacker);
            }
        }

        EffectManager.ApplyEffects(attacked, attacked, selfEffects, 1, this);
        EffectManager.ApplyEffects(attacked, attacker, targetEffects, 1, this);

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
                    abilitySources[i].TriggerSource(this, false, attacker, attacked, attacker.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, abilityType);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(this, false, attacker, attacker, attacker.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, abilityType);
                }
            }
        }

        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                castActiveAbilities[i].CastAbility(attacker, attacked);
            }
        }

        EffectManager.ApplyEffects(attacker, attacker, selfEffects, 1, this);
        EffectManager.ApplyEffects(attacker, attacked, targetEffects, 1, this);

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, attacker);
        ObjectUtilities.CreateSpecialEffects(targetSpecialEffects, attacked);
    }

    private bool SuccessfulTrigger(AbilityValue abilityValue)
    {
        if (abilityValue.ignorePassive == this)
            return false;

        if (!abilityValue.isUnitTrigger)
            return false;

        if (!triggeredByEffects && abilityValue.isEffect)
            return false;

        if (!IsTriggeredByHitType(abilityValue))
            return false;

        if (!IsTriggeredByAbilityType(abilityValue.abilityType))
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

        if (hitType == HitType.Glancing && abilityValue.isGlancing && !abilityValue.isCritical)
            return true;

        return false;
    }

    private bool IsTriggeredByAbilityType(AbilityType abilityType)
    {
        if (abilityTypeTrigger == AffectedAbility.AnyAbility)
            return true;

        if (abilityTypeTrigger == AffectedAbility.TypedAbility && 
            typeRequirement == abilityType)
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

        // Ability Source Value
        for (int i = 0; i < abilitySources.Count; i++)
        {
            temp = AbilityTooltipHandler.DetermineAbilityValue(temp, string.Format("<{0}>", i + 1), abilitySources[i], tooltipInfo);
            temp = AbilityTooltipHandler.DetermineAbilityModifierBaseline(temp, string.Format("<{0}modV>", i + 1), abilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityModifierAttribute(temp, string.Format("<{0}modP>", i + 1), abilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityConditionValue(temp, string.Format("<{0}conV>", i + 1), abilitySources[i].modifier);

        }

        for (int i = 0; i < castActiveAbilities.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseCastAbility(temp, string.Format("<castInfo{0}>", i + 1), string.Format("<castTooltip{0}>", i + 1), tooltipInfo, castActiveAbilities[i].activeAbility);
        }

        temp = AbilityTooltipHandler.ParseAllEffectTooltips(temp, tooltipInfo, selfEffects, targetEffects);

        temp = AbilityTooltipHandler.ParseProcChance(temp, "<%>", procChance);

        temp = AbilityTooltipHandler.CriticalHit(temp, "<critical>");

        temp = AbilityTooltipHandler.InsertRed(temp);

        if (!triggersPassives && abilitySources.Count > 0)
            temp = AbilityTooltipHandler.TriggerPassive(temp);

        return temp;
    }
}
