using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Passive
{
    public PassiveAbility passiveAbility;

    public int level;

    public Passive(PassiveAbility passiveAbility, int abilityLevel)
    {
        this.passiveAbility = passiveAbility;
        level = abilityLevel;
    }

    public void ActivatePassive(Unit unit)
    {
        passiveAbility.ActivatePassive(unit);
    }

    public void DeactivatePassive(Unit unit)
    {
        passiveAbility.DeactivatePassive(unit);
    }

}
