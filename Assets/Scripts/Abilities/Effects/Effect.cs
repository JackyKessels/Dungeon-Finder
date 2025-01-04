using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public EffectObject effectObject;

    public AbilityObject sourceAbility;

    public string name;
    public int level;
    public ProcType procType;
    public int duration;
    public Unit caster;
    public Unit target;

    public int stacks = 1;

    public float storedModValue;
    public List<TimedAction> timedActions = new List<TimedAction>();
    public Passive storedPassive;

    public Sprite IconOverride { get; set; }

    public Effect(EffectObject _effectObject)
    {
        effectObject = _effectObject;
    }

    public Effect(EffectObject _effectObject, int _stacks, Unit _caster, Unit _target, int _level, AbilityObject _sourceAbility)
    {
        effectObject = _effectObject;
        name = effectObject.name;
        level = _level;
        duration = effectObject.duration;
        procType = effectObject.procType;
        caster = _caster;
        target = _target;
        stacks = _stacks;
        sourceAbility = _sourceAbility;

        if (effectObject is EffectAttributeModifier attributeModifier)
        {
            if (attributeModifier.modifierType == ModifierType.Flat)
            {
                storedModValue = CalculateModifiedValue(this);
            }
            else
            {
                storedModValue = attributeModifier.multiplier + attributeModifier.multiplierPerLevel * (level - 1);
            }
        }

        if (effectObject is EffectOverTime timed)
        {
            foreach (TimedAction timedAction in timed.timedActions)
            {
                TimedAction applyTimedAction = new TimedAction(timedAction);
                applyTimedAction.storedValue = applyTimedAction.abilitySource.CalculateValue(_caster, _level, 1, 1);

                timedActions.Add(applyTimedAction);
            }
        }

        if (effectObject is EffectAbilityModifier abilityModifier)
        {
            storedModValue = abilityModifier.GetBonusMultiplier(level);
        }
    }

    public static Effect ApplyStacks(Effect sameEffect, Effect applyEffect)
    {
        // Missing an effect
        if (sameEffect == null || applyEffect == null)
            return null;

        // Not the same effect
        if (sameEffect.effectObject != applyEffect.effectObject)
            return null;

        EffectObject effectObject = sameEffect.effectObject;

        int newStacks = sameEffect.stacks + applyEffect.stacks;
        int level = sameEffect.level;
        float storedValue = sameEffect.storedModValue;
        List<TimedAction> timedActions = new List<TimedAction>(sameEffect.timedActions);

        // If max stacks is reached, do not add the new value
        // If max stacks is 0, then stack freely
        if (newStacks > effectObject.maxStacks && effectObject.maxStacks != 0)
        {
            newStacks = effectObject.maxStacks;
        }
        else // Stack effects
        {
            level = Mathf.Max(sameEffect.level, applyEffect.level);
            storedValue = sameEffect.storedModValue + applyEffect.storedModValue;

            for (int i = 0; i < sameEffect.timedActions.Count; i++)
            {
                timedActions[i] = TimedAction.ApplyStacksTimedActions(timedActions[i], applyEffect.timedActions[i]);
            }
        }

        Effect newEffect = new Effect(effectObject, newStacks, sameEffect.caster, sameEffect.target, level, sameEffect.sourceAbility);
        newEffect.storedModValue = storedValue;
        newEffect.timedActions = timedActions;

        if (!effectObject.refreshes)
        {
            newEffect.duration = sameEffect.duration;
        }

        return newEffect;
    }

    public static void DropStacks(Effect effect)
    {
        // DOES NOT WORK YET
        // ApplyStacks should keep a list of every stack with its value in the effect
        // Then the value should just sum up the values of the list
        // If a stack is dropped, then this stack should also remove the stats from the actual unit

        //int newStacks = effect.stacks - effect.effectObject.loseStacks;
        //float storedValue = effect.storedModValue;
        //List<TimedAction> timedActions = new List<TimedAction>(effect.timedActions);

        //if (newStacks <= 0)
        //{
        //    newStacks = 0;
        //}
        //else // Drop effects
        //{
        //    storedValue = GeneralUtilities.RoundFloat(storedValue / newStacks, 1);

        //    for (int i = 0; i < effect.timedActions.Count; i++)
        //    {
        //        timedActions[i] = TimedAction.DropStacksTimedAction(timedActions[i], newStacks);
        //    }
        //}

        //effect.stacks = newStacks;
        //effect.storedModValue = storedValue;
        //effect.timedActions = timedActions;
    }

    public float CalculateModifiedValue(Effect e)
    {
        AbilitySource a = (e.effectObject as EffectAttributeModifier).modifierSource;

        int totalBase = a.baseValue + a.levelBase * (e.level - 1);

        float totalScaling = a.scaling + a.levelScaling * (e.level - 1);

        int totalAttribute = e.caster.statsManager.GetAttributeValue((int)a.attributeType);

        float totalValue = totalBase + totalScaling * totalAttribute;

        return totalValue;
    }
}

[System.Serializable]
public class ApplyEffectObject
{
    public EffectObject effectObject;
    public AbilityTargets abilityTarget;
    public AbilityObject sourceAbility;
    public bool stacksBasedOnCooldown = false;

    public void ApplyEffect(Unit caster, ActiveAbility activeAbility = null)
    {
        if (effectObject == null)
        {
            Debug.Log("No effect selected.");
            return;
        }

        int level = caster.spellbook.GetAbilityLevel(sourceAbility);

        List<Unit> targets = AbilityUtilities.GetAbilityTargets(abilityTarget, caster);

        int stacks = 1;

        if (activeAbility != null && effectObject.stackable && stacksBasedOnCooldown)
        {
            stacks = activeAbility.cooldown + 1;
        }

        foreach (Unit target in targets)
        {
            for (int i = 0; i < stacks; i++)
            {
                EffectManager.ApplyEffect(effectObject, caster, target, level, sourceAbility);
            }        
        }
    }
}

