using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instant Ability", menuName = "Unit/Ability Object/Instant Ability")]
public class InstantAbility : ActiveAbility
{
    [Header("Targeting")]
    public AbilityTargets abilityTargets = AbilityTargets.SelfOnly;

    public override void TriggerAbility(Unit caster, Unit target, int level)
    {
        bool selfEffectBool = selfEffectsPerTarget;

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);

        foreach (Unit unit in AbilityUtilities.GetAbilityTargets(abilityTargets, caster))
        {
            float abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(this, unit);

            AbilityActions(caster, target, level, selfEffectBool, 1, abilityMultiplier);
        }

        // Do self effects after the actions
        SelfEffectOnly(caster, level, selfEffectBool);
    }
}
