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

    [Space(10)]
    [Range(0, 100)] public int procChance;
    [Space(10)]
    public List<AbilitySource> abilitySources;

    [Header("Effects")]
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

    private void TriggerReceiveEvent(AbilityValue abilityValue)
    {
        if (!abilityValue.isUnitTrigger)
            return;

        if (!IsTriggeredByHitType(abilityValue))
            return;

        if (!IsTriggeredByAbilityType(abilityValue.abilityType))
            return;

        if (!IsTriggeredBySchool(abilityValue.school))
            return;

        if (!IsProcced())
            return;

        Unit attacked = abilityValue.target;
        Unit attacker = abilityValue.caster;

        if (abilitySources.Count > 0)
        {
            for (int i = 0; i < abilitySources.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    abilitySources[i].TriggerSource(attacked, attacked, attacked.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, type);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(attacked, attacker, attacked.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, type);
                }
            }
        }

        AbilityTriggerable.ApplyEffects(attacked, attacked, selfEffects, 1);
        AbilityTriggerable.ApplyEffects(attacked, attacker, targetEffects, 1);

        ObjectUtilities.CreateSpecialEffects(this.casterSpecialEffects, attacked);
        ObjectUtilities.CreateSpecialEffects(this.targetSpecialEffects, attacker);
    }

    private void TriggerCauseEvent(AbilityValue abilityValue)
    {
        if (!abilityValue.isUnitTrigger)
            return;

        if (!IsTriggeredByHitType(abilityValue))
            return;

        if (!IsTriggeredByAbilityType(abilityValue.abilityType))
            return;

        if (!IsTriggeredBySchool(abilityValue.school))
            return;

        if (!IsProcced())
            return;

        Unit attacker = abilityValue.caster;
        Unit attacked = abilityValue.target;

        if (abilitySources.Count > 0)
        {
            for (int i = 0; i < abilitySources.Count; i++)
            {
                if (targetTrigger == TargetTrigger.Attacked)
                {
                    abilitySources[i].TriggerSource(attacker, attacked, attacker.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, type);
                }
                else if (targetTrigger == TargetTrigger.Attacker)
                {
                    abilitySources[i].TriggerSource(attacker, attacker, attacker.spellbook.GetPassiveLevel(this), 1, triggersPassives, 1, type);
                }
            }
        }

        AbilityTriggerable.ApplyEffects(attacker, attacker, selfEffects, 1);
        AbilityTriggerable.ApplyEffects(attacker, attacked, targetEffects, 1);

        ObjectUtilities.CreateSpecialEffects(this.casterSpecialEffects, attacker);
        ObjectUtilities.CreateSpecialEffects(this.targetSpecialEffects, attacked);
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

        temp = AbilityTooltipHandler.ParseAllEffectTooltips(temp, tooltipInfo, selfEffects, targetEffects);

        temp = AbilityTooltipHandler.ParseProcChance(temp, "<%>", procChance);

        temp = AbilityTooltipHandler.CriticalHit(temp, "<critical>");

        temp = AbilityTooltipHandler.InsertRed(temp);

        if (!triggersPassives && abilitySources.Count > 0)
            temp = AbilityTooltipHandler.TriggerPassive(temp);

        return temp;
    }
}
