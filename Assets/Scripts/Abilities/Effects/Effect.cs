using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public EffectObject data;

    public Active parentAbility;

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

    public void Setup(EffectObject d, Unit c, Unit t, int l)
    {
        data = d;
        name = data.name;
        level = l;
        duration = data.duration;
        procType = data.procType;
        caster = c;
        target = t;

        if (data is EffectOverTime timed)
        {
            foreach (TimedAction timedAction in timed.timedActions)
            {
                timedActions.Add(new TimedAction(timedAction));
            }
        }
    }
}
