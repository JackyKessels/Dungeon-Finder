using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveAbility : AbilityObject
{
    [Header("[ Passive ]")]
    public bool isPositive = true;

    public abstract void ActivatePassive(Unit unit);

    public abstract void DeactivatePassive(Unit unit);


    public override string GetCompleteTooltip(TooltipObject tooltipInfo)
    {
        string temp = base.GetCompleteTooltip(tooltipInfo);

        var color = isPositive ? ColorDatabase.Positive : ColorDatabase.Negative;

        temp += $"<color={color}>\nPassive Effect: </color>{ParseDescription(description, tooltipInfo)}";

        return temp;
    }

    public string GetEquipmentDescription(TooltipObject tooltipInfo)
    {
        string temp = DescriptionExcludeLevel();

        var color = isPositive ? ColorDatabase.Positive : ColorDatabase.Negative;

        temp += $"<color={color}>\nPassive Effect: </color>{ParseDescription(description, tooltipInfo)}";

        return temp;
    }

    public virtual string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        temp = AbilityTooltipHandler.ParseName(temp, "<name>", this);
        temp = AbilityTooltipHandler.ParseAbilityType(temp);

        temp = AbilityTooltipHandler.ColorAllSchools(temp);

        temp = AbilityTooltipHandler.ColorAllAttributes(temp);

        temp = AbilityTooltipHandler.CurrentArmor(temp, "<currentarmor>", tooltipInfo);

        temp = AbilityTooltipHandler.InsertRed(temp);
        temp = AbilityTooltipHandler.InsertGeneric(temp);
        temp = AbilityTooltipHandler.InsertNonScaling(temp);

        return temp;
    }
}
