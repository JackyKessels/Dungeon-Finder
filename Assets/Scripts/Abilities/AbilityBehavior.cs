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
    Speed,
    Vitality
}

public enum CastCondition
{
    Nothing,
    BelowHealthThreshold,
    AboveHealthThreshold
}

[System.Serializable]
public class AbilityBehavior
{
    public ActiveAbility ability;
    public TargetCondition target;
    public TargetAttribute targetAttribute;
    public CastCondition condition;
    [Range(0, 100)]
    public int healthThreshold = 100;
    public int startCooldown = 0;
    public bool charged = false;
}


