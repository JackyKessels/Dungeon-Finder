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

        if (effectObject is EffectAttributeModifier modifier)
        {
            if (modifier.modifierType == ModifierType.Flat)
            {
                storedModValue = CalculateModifiedValue(this);
            }
            else
            {
                storedModValue = modifier.multiplier + modifier.multiplierPerLevel * (level - 1);
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
    }

    public static Effect StackEffects(Effect sameEffect, Effect applyEffect)
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
                timedActions[i] = TimedAction.StackTimedActions(timedActions[i], applyEffect.timedActions[i]);
            }
        }

        Effect newEffect = new Effect(effectObject, newStacks, sameEffect.caster, sameEffect.target, level, sameEffect.sourceAbility);
        newEffect.storedModValue = storedValue;
        newEffect.timedActions = timedActions;
        if (!effectObject.refreshes)   
            newEffect.duration = sameEffect.duration;

        return newEffect;
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
