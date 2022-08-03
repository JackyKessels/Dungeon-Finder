using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive", menuName = "Unit/Effect Object/Passive")]
public class EffectActivatePassive : EffectObject
{
    [Header("[ Passive ]")]
    public PassiveAbility passiveAbility;
    public List<ParticleSystem> specialEffects = new List<ParticleSystem>();

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
