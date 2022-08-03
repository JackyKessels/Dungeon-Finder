using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveAbility : AbilityObject
{
    [Header("[ Passive ]")]
    public bool isPositive = true;

    public abstract void ActivatePassive(Unit unit);

    public abstract void DeactivatePassive(Unit unit);


    public override string GetDescription(TooltipObject tooltipInfo)
    {
        string positiveColor = "#70FF62";
        string negativeColor = "#FF1717";

        string temp = base.GetDescription(tooltipInfo);

        if (isPositive)
        {
            temp += string.Format("<color={0}>\nPassive Effect: </color>{1}", positiveColor, ParseDescription(description, tooltipInfo));
        }
        else
        {
            temp += string.Format("<color={0}>\nPassive Effect: </color>{1}", negativeColor, ParseDescription(description, tooltipInfo));
        }

        return temp;
    }

    public virtual string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        temp = AbilityTooltipHandler.ColorAllSchools(temp);

        temp = AbilityTooltipHandler.ColorAllAttributes(temp);

        return temp;
    }
}
