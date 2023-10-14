using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TargetTargets
{
    Single,
    Adjacent
}

[CreateAssetMenu(fileName = "New Target Ability", menuName = "Unit/Ability Object/Target Ability")]
public class TargetAbility : ActiveAbility
{
    [Header("Targeting")]
    public bool targetsAllies = false;
    public bool targetsEnemies = true;

    public TargetTargets targetTargets = TargetTargets.Single;
    [Range(0, 1)]
    public float adjacentModifier = 0;

    public override void TriggerAbility(Unit caster, Unit target, int level, float effectiveness)
    {
        if (target == null)
        {
            Debug.Log("No target given.");
            return;
        }

        base.TriggerAbility(caster, target, level, effectiveness);

        bool selfEffectBool = selfEffectsPerTarget;

        ObjectUtilities.CreateSpecialEffects(casterSpecialEffects, caster);

        float abilityMultiplier = (1 + caster.effectManager.ApplyMultipliers(this, target)) * effectiveness;

        AbilityActions(caster, target, level, selfEffectBool, 1, abilityMultiplier);

        if (targetTargets == TargetTargets.Adjacent)
        {
            foreach (Unit unit in AbilityUtilities.GetAdjacentUnits(target))
            {
                abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(this, unit);

                AbilityActions(caster, unit, level, selfEffectBool, adjacentModifier, abilityMultiplier);
            }
        }

        // Do self effects after the actions
        SelfEffectOnly(caster, level, selfEffectBool);
    }
}
