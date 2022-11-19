using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public EffectObject data;

    public AbilityObject sourceAbility;

    public string name;
    public int level;
    public ProcType procType;
    public int duration;
    public Unit caster;
    public Unit target;

    public float storedModValue;
    public List<TimedAction> timedActions = new List<TimedAction>();
    public Passive storedPassive;

    public Sprite IconOverride { get; set; }

    public void Setup(EffectObject _data, Unit _caster, Unit _target, int _level, AbilityObject _sourceAbility)
    {
        data = _data;
        name = data.name;
        level = _level;
        duration = data.duration;
        procType = data.procType;
        caster = _caster;
        target = _target;
        sourceAbility = _sourceAbility;

        if (data is EffectOverTime timed)
        {
            foreach (TimedAction timedAction in timed.timedActions)
            {
                timedActions.Add(new TimedAction(timedAction));
            }
        }
    }
}
