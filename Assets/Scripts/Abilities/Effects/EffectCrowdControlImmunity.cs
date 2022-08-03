using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crowd Control Immunity", menuName = "Unit/Effect Object/Crowd Control Immunity")]
public class EffectCrowdControlImmunity : EffectObject
{
    public List<CrowdControlType> immuneTypes;

    public bool HasImmunityType(CrowdControlType crowdControlType)
    {
        for (int i = 0; i < immuneTypes.Count; i++)
        {
            if (immuneTypes[i] == crowdControlType)
                return true;
        }

        return false;
    }

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }
}
