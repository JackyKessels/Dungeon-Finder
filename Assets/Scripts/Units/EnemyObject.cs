using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Unit/Unit Object/Enemy")]
public class EnemyObject : UnitObject
{
    [Header("[ Enemy Only ]")]
    public int level = 1;
    [Range(0, 10)]
    public int difficulty;
    public int experienceReward;
    public List<Currency> currencyRewards;
    public List<ItemDrop> itemDrops;

    [Header("[ Abilities ]")]
    public bool randomAbility = true;
    public List<AbilityBehavior> abilities;
    public List<PassiveAbility> passiveAbilities;
}
