using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OnUseTriggerType
{
    AnyAbility,
    TypedAbility,
    SpecificAbility,
}

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Unit/Ability Object/Passive On Use")]
public class PassiveOnUse : PassiveAbility
{
    [Header("[ On Use Functionality ]")]
    public OnUseTriggerType triggerType;
    public AbilityType typeRequirement;
    public List<ActiveAbility> specificActiveAbilityList;

    [Space(10)]
    [Range(0, 100)] public int procChance;

    [Header("[ On Use Events ]")]
    public List<CastActiveAbility> castActiveAbilities;
    public List<ApplyEffectObject> applyEffectObjects;

    public override void ActivatePassive(Unit unit)
    {
        unit.OnAbilityCast += TriggerUseEvent;
    }

    public override void DeactivatePassive(Unit unit)
    {
        unit.OnAbilityCast -= TriggerUseEvent;
    }
    
    private void TriggerUseEvent(Unit caster, ActiveAbility activeAbility)
    {
        if (!SuccessfulTrigger(activeAbility))
            return;

        if (castActiveAbilities.Count > 0)
        {
            for (int i = 0; i < castActiveAbilities.Count; i++)
            {
                castActiveAbilities[i].CastAbility(caster, null);
            }
        }

        foreach (ApplyEffectObject applyEffectObject in applyEffectObjects)
        {
            applyEffectObject.ApplyEffect(caster, activeAbility);
        }

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);
    }

    private bool SuccessfulTrigger(ActiveAbility activeAbility)
    {
        if (!IsTriggeredBySpecificAbility(activeAbility))
            return false;

        if (!IsTriggeredByAbilityType(activeAbility.abilityType))
            return false;

        if (!IsProcced())
            return false;

        return true;
    }

    private bool IsTriggeredBySpecificAbility(ActiveAbility activeAbility)
    {
        if (triggerType == OnUseTriggerType.SpecificAbility &&
            !specificActiveAbilityList.Contains(activeAbility))
        {
            return false;
        }

        return true; 
    }

    private bool IsTriggeredByAbilityType(AbilityType abilityType)
    {
        if (triggerType == OnUseTriggerType.AnyAbility)
            return true;

        if (triggerType == OnUseTriggerType.TypedAbility && 
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

        for (int i = 0; i < castActiveAbilities.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseCastAbility(temp, string.Format("<castInfo{0}>", i + 1), string.Format("<castTooltip{0}>", i + 1), tooltipInfo, castActiveAbilities[i].activeAbility);
        }

        temp = AbilityTooltipHandler.ParseApplyEffectTooltips(temp, tooltipInfo, applyEffectObjects);    

        temp = AbilityTooltipHandler.ParseProcChance(temp, "<%>", procChance);

        temp = AbilityTooltipHandler.InsertRed(temp);

        return temp;
    }
}
