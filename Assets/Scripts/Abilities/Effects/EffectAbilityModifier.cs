using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum AffectedAbility
{
    AnyAbility,
    TypedAbility,
    SpecificAbility,
    SpecificEffect
}

//public enum ModifierType
//{
//    Multiplier,
//    Effect,
//    AbilitySource
//}

[CreateAssetMenu(fileName = "New Ability Modifier", menuName = "Unit/Effect Object/Ability Modifier")]
public class EffectAbilityModifier : EffectObject
{
    [Header("[ Ability Modifier ]")]
    public AffectedAbility affectedAbility;
    public bool getConsumed;

    [Header("[ Typed Ability ]")]
    public AbilityType abilityType;

    [Header("[ Specific Abilities ]")]
    public List<AbilityObject> specificAbilities;

    [Header("[ Modification ]")]
    public float bonusMultiplier;
    public float bonusPerLevel;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        string check = "<caster>";

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, "<color=#FF0000>{0}</color>");

            temp = string.Format(temp, tooltipInfo.effect.caster.name);
        }

        temp = AbilityTooltipHandler.DetermineBonusMultipler(temp, "<bonus>", tooltipInfo.effect.storedModValue);

        temp = AbilityTooltipHandler.DetermineSpecificAbilities(temp, "<specific>", specificAbilities);

        temp = AbilityTooltipHandler.DetermineTypedEffect(temp, "<typed>", abilityType);

        return temp;
    }

    public float GetBonusMultiplier(int level)
    {
        return bonusMultiplier + bonusPerLevel * (level - 1);
    }

    public bool IsValidAbility(AbilityObject castAbility)
    {
        return specificAbilities.Contains(castAbility);
    }
}
