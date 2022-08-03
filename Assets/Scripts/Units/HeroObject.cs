using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Unit/Unit Object/Hero")]
public class HeroObject : UnitObject
{
    [Header("[ Hero Only ]")]
    public HeroClass heroClass = HeroClass.None;
    public bool dualWield = false;

    public ItemObject startingArmor;
    public ItemObject startingWeapon;

    public List<HeroPathObject> paths;

    [Header("[ Spellbook ]")]
    public List<AbilityObject> startingAbilities;
}

[System.Serializable]
public class ClassAbilityObject
{
    public ActiveAbility ability;
    public int levelRequirement;
}
