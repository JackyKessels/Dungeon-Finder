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

        float abilityMultiplier = (1 + caster.effectManager.ApplyMultipliers(this, target)) * effectiveness;

        AbilityActions(caster, target, level, 1, abilityMultiplier);

        if (targetTargets == TargetTargets.Adjacent)
        {
            List<Unit> targets = AbilityUtilities.GetAdjacentUnits(target);

            foreach (Unit unit in targets)
            {
                abilityMultiplier = 1 + caster.effectManager.ApplyMultipliers(this, unit);

                AbilityActions(caster, unit, level, adjacentModifier, abilityMultiplier);
            }

            TriggerSelfEffects(targets.Count, caster, level);
        }
    }
}
