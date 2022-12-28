using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attribute Modifier", menuName = "Unit/Effect Object/Attribute Modifier")]
public class EffectAttributeModifier : EffectObject
{
    [Header("[ Attribute Information ]")]
    public AttributeType attributeModified;
    public bool isIncrease = true;
    public ModifierType modifierType;

    [Header("[ Flat ]")]
    public AbilitySource modifierSource;

    [Header("[ Multiplier ]")]
    public float multiplier;
    public float multiplierPerLevel;

    public List<ParticleSystem> specialEffects = new List<ParticleSystem>();

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        temp = DetermineValue(temp, "<1>", tooltipInfo);

        // ONLY USE FOR NON-LEVELING AND ENEMIES
        string checkPercentage = "<1%>";
        if (temp.Contains(checkPercentage))
        {
            temp = temp.Replace(checkPercentage, "<color=" + ColorDatabase.ScalingColor(modifierSource.attributeType) + ">{0}</color>");

            temp = string.Format(temp, GeneralUtilities.RoundFloat(modifierSource.scaling * 100, 1));
        }

        temp = DetermineColor(temp, "<Heal>", AbilitySchool.Healing);
        temp = DetermineColor(temp, "<P>", AbilitySchool.Physical);
        temp = DetermineColor(temp, "<F>", AbilitySchool.Fire);
        temp = DetermineColor(temp, "<I>", AbilitySchool.Ice);
        temp = DetermineColor(temp, "<N>", AbilitySchool.Nature);
        temp = DetermineColor(temp, "<A>", AbilitySchool.Arcane);
        temp = DetermineColor(temp, "<H>", AbilitySchool.Holy);
        temp = DetermineColor(temp, "<S>", AbilitySchool.Shadow);

        return temp;
    }

    private string DetermineValue(string temp, string check, TooltipObject tooltipInfo)
    {
        float value;

        if (tooltipInfo.state == CurrentState.Values)
        {
            value = GetFlatValue(tooltipInfo);
        }
        else
        {
            value = tooltipInfo.effect.storedModValue;
        }

        if (temp.Contains(check))
        {
            if (modifierType == ModifierType.Flat)
            {
                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(modifierSource.attributeType) + ">{0}</color>");

                temp = string.Format(temp, GeneralUtilities.RoundFloat(value, 0));
            }
            else
            {
                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(modifierSource.attributeType) + ">{0}</color>%");

                temp = string.Format(temp, GeneralUtilities.RoundFloat(value * 100f, 1));
            }
        }

        return temp;
    }

    public float GetFlatValue(TooltipObject tooltipInfo)
    {
        float value;

        if (modifierType == ModifierType.Flat)
        {
            value = modifierSource.baseValue;
        }
        else
        {
            value = multiplier;
        }

        return value;
    }
}
