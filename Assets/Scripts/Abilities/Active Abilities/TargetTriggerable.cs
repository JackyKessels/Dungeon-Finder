using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetTriggerable : AbilityTriggerable
{
    [HideInInspector] public TargetTargets abilityTarget;
    [HideInInspector] public float adjacentModifier;

    public override void Trigger(Unit target, int level)
    {
        bool selfEffectBool = active.activeAbility.selfEffectsPerTarget;

        ObjectUtilities.CreateSpecialEffects(active.activeAbility.casterSpecialEffects, caster);

        float abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(active.activeAbility, target);

        AbilityActions(target, level, selfEffectBool, 1, abilityMultiplier);

        if (abilityTarget == TargetTargets.Adjacent)
        {
            foreach (Unit unit in AbilityUtilities.GetAdjacentUnits(target))
            {
                abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(active.activeAbility, unit);

                AbilityActions(unit, level, selfEffectBool, adjacentModifier, abilityMultiplier);
            }
        }

        // Do self effects after the actions
        SelfEffectOnly(level, selfEffectBool);
    }
}
