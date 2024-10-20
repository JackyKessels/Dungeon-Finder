﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ProcType { Turn, Round }

[System.Serializable]
public abstract class EffectObject : ScriptableObject, IDescribable, ITriggerSource
{
    [Header("[ General ]")]
    public new string name = "New Effect";
    public Sprite icon;
    public bool hidden = false;
    [TextArea(10, 10)] public string description;
    public List<ParticleSystem> specialEffects = new List<ParticleSystem>();

    [Header("[ Mechanics ]")]
    public bool isBuff;
    public ProcType procType = ProcType.Turn;
    public int duration = 1;
    public bool permanent = false;
    public bool stackable = false;
    public int maxStacks = 0;
    public bool refreshes = true;
    [Tooltip("True = Effect is removed when caster dies.\nFalse = Effect persists when caster dies.")]
    public bool aura = false;
    [Tooltip("True = This effect can only be active on 1 target.\nFalse = No limitations.")]
    public bool unique = false;
    [Tooltip("The number of stacks applied on effect application.")]
    public int applyStacks = 1;
    [Tooltip("The number of stacks lost per Turn/Round.")]
    public int loseStacks = 0;
    public List<EffectObject> removeEffects;

    public string GetDescription(TooltipObject tooltipInfo)
    {
        return name + "\nEffect: " + ParseDescription(description, tooltipInfo);
    }

    protected abstract string ParseDescription(string s, TooltipObject tooltipInfo);

    public static string DurationText(EffectObject effectObject)
    {
        if (effectObject.permanent)
        {
            return "Full Battle";
        }
        else
        {
            if (effectObject.procType == ProcType.Turn)
            {
                if (effectObject.duration > 1)
                {
                    return effectObject.duration + " Turns";
                }
                else
                {
                    return "1 Turn";
                }
            }
            else
            {
                if (effectObject.duration > 1)
                {
                    return effectObject.duration + " Rounds";
                }
                else
                {
                    return "1 Round";
                }
            }   
        }
    }

    public ITriggerSource GetTriggerSource()
    {
        return this;
    }
}
