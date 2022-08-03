using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charge Target", menuName = "Unit/Effect Object/Charge Target")]
public class EffectChargeTarget : EffectObject
{
    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        string check = "<caster>";

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.CasterColor() + ">{0}</color>");

            temp = string.Format(temp, tooltipInfo.effect.caster.name);
        }

        return temp;
    }
}
