using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrowdControlType
{
    Taunt,
    Stun,
    Incapacitate
}

[CreateAssetMenu(fileName = "New Crowd Control", menuName = "Unit/Effect Object/Crowd Control")]
public class EffectCrowdControl : EffectObject
{
    [Header("Crowd Control")]
    public CrowdControlType type;
    [Tooltip("True = When Taunt fades, make target immune to taunt for 1 Round.\nFalse = No taunt immunity.")]
    public bool addTauntImmune;

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
