using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charge Target", menuName = "Unit/Effect Object/Charge Target")]
public class EffectChargeTarget : EffectObject
{
    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        string caster = "<caster>";
        string ability = "<ability>";

        if (temp.Contains(caster))
        {
            temp = temp.Replace(caster, "<color=" + ColorDatabase.CasterColor() + ">{0}</color>");

            temp = string.Format(temp, tooltipInfo.effect.caster.name);
        }

        if (temp.Contains(ability))
        {
            temp = temp.Replace(ability, "<color=" + ColorDatabase.CasterColor() + ">{0}</color>");

            Enemy enemy = tooltipInfo.effect.caster as Enemy;

            temp = string.Format(temp, enemy.chargedAbility[0].activeAbility.name);
        }

        return temp;
    }
}
