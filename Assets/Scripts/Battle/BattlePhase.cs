using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlePhase
{
    [Header("General")]
    public string name = "";
    [Range(0, 1)]
    [Tooltip("Value between 0 and 1 => 0% and 100%")]
    public float healthTransition;

    [Header("Abilities")]
    public bool randomAbility = false;
    public List<AbilityBehavior> abilityBehaviors;
    public List<PassiveAbility> passiveAbilities;

    [Header("Art")]
    public Sprite newSprite;
    public List<ParticleSystem> transitionEffects;
    public AudioClip transitionSound;
}