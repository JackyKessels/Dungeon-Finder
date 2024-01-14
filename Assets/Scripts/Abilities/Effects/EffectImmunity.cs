using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImmunityType
{
    Damage,
}

[CreateAssetMenu(fileName = "New Immunity", menuName = "Unit/Effect Object/Immunity")]
public class EffectImmunity : EffectObject
{
    [Header("Immunity")]
    public ImmunityType type;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        return temp;
    }
}
