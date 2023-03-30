using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetCondition
{
    Random,
    LowestAttribute,
    HighestAttribute
}

public enum TargetAttribute
{
    CurrentHealth,
    MaximumHealth,
    Power,
    Wisdom,
    Armor,
    Resistance,
    Speed
}

public enum CastCondition
{
    Nothing,
    BelowHalfHealth,
    AboveHalfHealth
}

[System.Serializable]
public class AbilityBehavior
{
    public ActiveAbility ability;
    public TargetCondition target;
    public TargetAttribute targetAttribute;
    public CastCondition condition;
    public int startCooldown = 0;
    public bool charged = false;
}


