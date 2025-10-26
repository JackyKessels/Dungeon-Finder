using System.Collections.Generic;
using UnityEngine;

public enum EffectInteractionType
{
    TargetOfAbility,
    TargetOfEffect,
    TargetOfSchool
}

public enum EffectInteractionResult
{
    RemoveThis
}

[System.Serializable]
public class EffectInteraction
{
    public EffectInteractionType interactionType = EffectInteractionType.TargetOfAbility;
    public EffectInteractionResult result = EffectInteractionResult.RemoveThis;

    [Header("[ Target of Ability ]")]
    public AbilityType abilityType;

    [Header("[ Target of School ]")]
    public List<AbilitySchool> abilitySchools;
}
