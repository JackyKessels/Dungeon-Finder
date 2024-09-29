using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instant Ability", menuName = "Unit/Ability Object/Instant Ability")]
public class InstantAbility : ActiveAbility
{
    [Header("Targeting")]
    public AbilityTargets abilityTargets = AbilityTargets.SelfOnly;

    public override void TriggerAbility(Unit caster, Unit target, int level, float effectiveness)
    {
        base.TriggerAbility(caster, target, level, effectiveness);

        float abilityMultiplier = (1 + caster.effectManager.ApplyMultipliers(this, target)) * effectiveness;

        AbilityActions(caster, target, level, 1, abilityMultiplier);
    }
}
