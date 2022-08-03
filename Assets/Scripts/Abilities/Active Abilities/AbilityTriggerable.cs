using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityTriggerable : MonoBehaviour
{
    [HideInInspector] public Unit caster;

    [HideInInspector] public Active active;

    protected TeamManager teamManager;

    public void Start()
    {
        teamManager = TeamManager.Instance;

        caster = GetComponent<Unit>();
    }

    public abstract void Trigger(Unit target, int level);

    public static void ApplyEffects(Unit caster, Unit target, List<EffectObject> list, int level)
    {
        if (list.Count <= 0)
            return;

        foreach (EffectObject e in list)
        {
            if (!target.statsManager.isDead)
            {
                EffectManager.ApplyEffect(e, caster, target, level);
            }
        }
    }

    public void SelfEffectOnly(int level, bool selfEffectPerTarget)
    {
        if (!selfEffectPerTarget)
        {
            if (active.activeAbility.selfEffects.Count > 0)
            {
                ApplyEffects(caster, caster, active.activeAbility.selfEffects, level);
            }
        }
    }

    // When an ability is triggered, this function will fire
    // 1. Causing the special effects on the target
    // 2. Triggering the ability source to deal damage/healing
    // 3. Applying effects if necessary
    public void AbilityActions(Unit target, int level, bool selfEffectPerTarget, float adjacentModifier, float abilityMultiplier)
    {
        ObjectUtilities.CreateSpecialEffects(active.activeAbility.targetSpecialEffects, target);

        AbilityType abilityType = active.activeAbility.type;

        if (caster.IsTargetEnemy(target))
        {
            // Trigger the ability's hostile sources to do damage/heal
            foreach (AbilitySource source in active.enemyAbilitySources)
            {
                source.TriggerSource(caster, target, level, adjacentModifier, true, abilityMultiplier, abilityType);
            }
        }
        else
        {
            // Trigger the ability's friendly sources to do damage/heal
            foreach (AbilitySource source in active.allyAbilitySources)
            {
                source.TriggerSource(caster, target, level, adjacentModifier, true, abilityMultiplier, abilityType);
            }
        }

        // Apply self effects to the caster
        if (selfEffectPerTarget)
        {
            if (active.activeAbility.selfEffects.Count > 0)
            {
                ApplyEffects(caster, caster, active.activeAbility.selfEffects, level);
            }
        }

        // Apply effects to the target
        if (active.activeAbility.targetEffects.Count > 0)
        {
            ApplyEffects(caster, target, active.activeAbility.targetEffects, level);
        }
    }
}
