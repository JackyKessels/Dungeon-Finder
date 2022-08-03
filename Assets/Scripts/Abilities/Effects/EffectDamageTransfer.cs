using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Transfer", menuName = "Unit/Effect Object/Damage Transfer")]
public class EffectDamageTransfer : EffectObject
{
    public float percentage;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        temp = GetValue(temp, "<1>");
        temp = GetCaster(temp, "<caster>", tooltipInfo);

        return temp;
    }

    private string GetValue(string temp, string check)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.CasterColor() + ">" + percentage * 100 + "</color>%");
        }

        return temp;
    }

    private string GetCaster(string temp, string check, TooltipObject tooltipInfo)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.CasterColor() + ">{0}</color>");

            temp = string.Format(temp, tooltipInfo.effect.caster.name);
        }

        return temp;
    }
}
