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
    public EffectObject conditionalEffect;
    public List<CrowdControlType> ccTypes;
    public List<EffectObject> specificEffects;
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

        temp = AbilityTooltipHandler.ParseEffectTooltips(tooltipInfo, new List<EffectObject>() { conditionalEffect }, temp, "");

        for (int i = 0; i < specificEffects.Count; i++)
        {
            temp = AbilityTooltipHandler.ParseEffectTooltips(tooltipInfo, specificEffects, temp, "spec");
        }

        temp = AbilityTooltipHandler.ColorAllSchools(temp);

        return temp;
    }
}
