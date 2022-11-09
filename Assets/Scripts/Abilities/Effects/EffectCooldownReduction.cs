using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CooldownReductionType
{
    Random,
    Specific,
    All,
    Typed
}

[CreateAssetMenu(fileName = "New Cooldown Reduction", menuName = "Unit/Effect Object/Cooldown Reduction")]
public class EffectCooldownReduction : EffectObject
{
    [Header("Cooldown Reduction")]
    public CooldownReductionType type;
    public int roundsReduction;
    public List<ParticleSystem> casterSpecialEffects;

    [Header("Specific")]
    public AbilityObject specificAbility;

    [Header("Typed")]
    public AbilityType abilityType;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }

    public void ReduceCooldown(Unit caster)
    {
        List<Active> validAbilities = new List<Active>();

        foreach (Active active in caster.spellbook.activeSpellbook)
        {
            if (active != null && active.currentCooldown > 0)
                validAbilities.Add(active);
        }

        if (validAbilities.Count == 0)
            return;

        switch (type)
        {
            case CooldownReductionType.Random:
                {
                    int randomAbility = Random.Range(0, validAbilities.Count);

                    ReduceCooldown(caster, validAbilities[randomAbility]);
                }
                break;
            case CooldownReductionType.Specific:
                {
                    foreach (Active active in validAbilities)
                    {
                        if (active.activeAbility == specificAbility)
                            ReduceCooldown(caster, active);
                    }
                }
                break;
            case CooldownReductionType.All:
                {
                    foreach (Active active in validAbilities)
                    {
                        ReduceCooldown(caster, active);
                    }
                }
                break;
            case CooldownReductionType.Typed:
                {
                    for (int i = 0; i < validAbilities.Count; i++)
                    {
                        if (validAbilities[i].activeAbility.abilityType == abilityType)
                            ReduceCooldown(caster, validAbilities[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ReduceCooldown(Unit caster, Active active)
    {
        active.CoolDown(roundsReduction);

        FCTDataSprite fctSprite = new FCTDataSprite(caster, active, true);
        caster.fctHandler.AddToFCTQueue(fctSprite);

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);
    }
}
