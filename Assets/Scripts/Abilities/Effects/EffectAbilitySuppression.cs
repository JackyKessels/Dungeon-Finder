using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Suppression", menuName = "Unit/Effect Object/Ability Suppression")]
public class EffectAbilitySuppression : EffectObject
{
    [Header("[ Ability Suppression ]")]
    public List<AbilityType> types;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }
}
