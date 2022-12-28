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
    public List<AttributeModification> modifications;

    public override void ActivatePassive(Unit unit)
    {
        foreach (AttributeModification modification in modifications)
        {
            int positiveOrNegative = modification.isIncrease ? 1 : -1;

            if (modification.modifierType == ModifierType.Flat)
                unit.statsManager.GetAttribute((int)modification.attributeModified).bonusValue += modification.flatValue * positiveOrNegative;
            else if (modification.modifierType == ModifierType.Multiplier)
                unit.statsManager.GetAttribute((int)modification.attributeModified).multiplier += modification.multiplierValue * positiveOrNegative;
        }
    }

    public override void DeactivatePassive(Unit unit)
    {
        foreach (AttributeModification modification in modifications)
        {
            int positiveOrNegative = modification.isIncrease ? -1 : 1;

            if (modification.modifierType == ModifierType.Flat)
                unit.statsManager.GetAttribute((int)modification.attributeModified).bonusValue += modification.flatValue * positiveOrNegative;
            else if (modification.modifierType == ModifierType.Multiplier)
                unit.statsManager.GetAttribute((int)modification.attributeModified).multiplier += modification.multiplierValue * positiveOrNegative;
        }
    }

    public override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = base.ParseDescription(s, tooltipInfo);

        for (int i = 0; i < modifications.Count; i++)
        {
            temp = DetermineFlatValue(temp, "<" + (i + 1) + ">", i);
            temp = DetermineMultiplierValue(temp, "<" + (i + 1) + "%>", i);
        }

        return temp;
    }

    private string DetermineFlatValue(string temp, string check, int i)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(modifications[i].attributeModified) + ">{0}</color>");

            return string.Format(temp, modifications[i].flatValue);
        }

        return temp;
    }

    private string DetermineMultiplierValue(string temp, string check, int i)
    {
        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(modifications[i].attributeModified) + ">{0}</color>%");

            return string.Format(temp, modifications[i].multiplierValue * 100);
        }

        return temp;
    }

    [System.Serializable]
    public class AttributeModification
    {
        public AttributeType attributeModified;
        public bool isIncrease = true;
        public ModifierType modifierType;
        public int flatValue = 0;
        public float multiplierValue = 0;
    }
}