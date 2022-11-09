using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstantTriggerable : AbilityTriggerable
{
    [HideInInspector] public AbilityTargets abilityTarget;

    public override void Trigger(Unit t, int level)
    {
        bool selfEffectBool = active.activeAbility.selfEffectsPerTarget;

        ObjectUtilities.CreateSpecialEffects(active.activeAbility.casterSpecialEffects, caster);

        foreach (Unit unit in AbilityUtilities.GetAbilityTargets(abilityTarget, caster))
        {
            float abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(active.activeAbility, unit);

            AbilityActions(unit, level, selfEffectBool, 1, abilityMultiplier);
        }

        // Do self effects after the actions
        SelfEffectOnly(level, selfEffectBool);
    }
}
