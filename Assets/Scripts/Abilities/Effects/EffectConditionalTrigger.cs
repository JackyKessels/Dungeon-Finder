using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionalEffectType
{
    None,
    CrowdControl,
    SpecificEffect
}

public enum ConditionalEffectTarget
{
    Target,
    Caster
}

[CreateAssetMenu(fileName = "New Conditional Trigger", menuName = "Unit/Effect Object/Conditional Trigger")]
public class EffectConditionalTrigger : EffectObject
{
    [Header("[ Conditional Effect ]")]
    public ConditionalEffectType conditionalEffectType = ConditionalEffectType.None;
    public List<EffectObject> appliedEffects;

    [Header("[ Conditions ]")]
    public List<CrowdControlType> ccTypes;
    public List<EffectObject> specificEffects;
    [Tooltip("False = at least 1 to trigger")]
    public bool hasAllEffects = false;
    public ConditionalEffectTarget checkTarget;
    public ConditionalEffectTarget effectTarget;
    public bool consumeEffect = false;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }

    public string GetConditionalDescription(TooltipObject tooltipInfo)
    {
        string temp = description;

        for (int i = 0; i < appliedEffects.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseEffectTooltips(temp, "applied", appliedEffects, tooltipInfo);
        }

        for (int i = 0; i < specificEffects.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseEffectTooltips(temp, "specific", specificEffects, tooltipInfo);
        }

        temp = AbilityTooltipHandler.ColorAllSchools(temp);
        temp = AbilityTooltipHandler.ColorAllAttributes(temp);

        return temp;
    }
}
