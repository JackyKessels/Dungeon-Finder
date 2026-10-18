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

public enum AbilityModifierType
{
    Multiplier,
    ApplySwiftness,
    ApplyEffect
}

[CreateAssetMenu(fileName = "New Ability Modifier", menuName = "Unit/Effect Object/Ability Modifier")]
public class EffectAbilityModifier : EffectObject
{
    [Header("[ Ability Modifier ]")]
    public AbilityModifierType modifierType = AbilityModifierType.Multiplier;
    public AffectedAbility affectedAbility;
    public bool getConsumed;
    public List<ParticleSystem> targetSpecialEffects = new List<ParticleSystem>();

    [Header("[ Typed Ability ]")]
    public AbilityType abilityType;

    [Header("[ Specific Abilities ]")]
    public List<AbilityObject> specificAbilities;

    [Header("[ Multiplier ]")]
    public float bonusMultiplier;
    public float bonusPerLevel;

    [Header("[ Apply Effect ]")]
    public List<EffectObject> appliedEffects;

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

        temp = AbilityTooltipHandler.ParseEffectTooltips(temp, "applied", appliedEffects, tooltipInfo);

        return temp;
    }

    public float GetBonusMultiplier(int level)
    {
        if (modifierType != AbilityModifierType.Multiplier)
        {
            return 1;
        }

        return bonusMultiplier + bonusPerLevel * (level - 1);
    }

    public bool IsValidAbility(AbilityObject castAbility)
    {
        if (castAbility == null)
        {
            return false;
        }

        return affectedAbility == AffectedAbility.AnyAbility ||
              (affectedAbility == AffectedAbility.TypedAbility && castAbility.abilityType == abilityType) ||
              (affectedAbility == AffectedAbility.SpecificAbility && specificAbilities.Contains(castAbility));
    }
}
