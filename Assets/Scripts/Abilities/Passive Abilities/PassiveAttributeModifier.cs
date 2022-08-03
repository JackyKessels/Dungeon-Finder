using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    Flat,
    Multiplier
}

[CreateAssetMenu(fileName = "New Passive Ability", menuName = "Unit/Ability Object/Passive Attribute Modifier")]
public class PassiveAttributeModifier : PassiveAbility
{
    [Header("[ Attribute Modifier ]")]
    public AttributeType attributeModified;
    public bool isIncrease = true;
    public ModifierType modifierType;

    public int flatValue = 0;
    public float multiplierValue = 0;

    //public AbilitySource modifierSource;

    public override void ActivatePassive(Unit unit)
    {
        int positiveOrNegative = isIncrease ? 1 : -1;

        if (modifierType == ModifierType.Flat)
            unit.statsManager.GetAttribute((int)attributeModified).bonusValue += flatValue * positiveOrNegative;
        else if (modifierType == ModifierType.Multiplier)
            unit.statsManager.GetAttribute((int)attributeModified).mulitplier += multiplierValue * positiveOrNegative;
    }

    public override void DeactivatePassive(Unit unit)
    {
        int positiveOrNegative = isIncrease ? -1 : 1;

        if (modifierType == ModifierType.Flat)
            unit.statsManager.GetAttribute((int)attributeModified).bonusValue += flatValue * positiveOrNegative;
        else if (modifierType == ModifierType.Multiplier)
            unit.statsManager.GetAttribute((int)attributeModified).mulitplier += multiplierValue * positiveOrNegative;
    }

    public override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = base.ParseDescription(s, tooltipInfo);

        temp = DetermineFlatValue(temp, "<1>");
        temp = DetermineMultiplierValue(temp, "<1%>");

        return temp;
    }

    private string DetermineFlatValue(string temp, string check)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(attributeModified) + ">{0}</color>");

            return string.Format(temp, flatValue);
        }

        return temp;
    }

    private string DetermineMultiplierValue(string temp, string check)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(attributeModified) + ">{0}</color>%");

            return string.Format(temp, multiplierValue * 100);
        }

        return temp;
    }
}
