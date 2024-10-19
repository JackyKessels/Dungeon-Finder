using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Passive", menuName = "Unit/Effect Object/Passive")]
public class EffectActivatePassive : EffectObject
{
    [Header("[ Passive ]")]
    public PassiveAbility passiveAbility;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        tooltipInfo.active.level = tooltipInfo.effect.level;

        string temp = s;

        string check = "<passive>";

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, passiveAbility.ParseDescription(passiveAbility.description, tooltipInfo));
        }

        return temp;
    }
}
