using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Active
{
    public Unit owner;

    public ActiveAbility activeAbility;

    public List<AbilitySource> allyAbilitySources;
    public List<AbilitySource> enemyAbilitySources;

    public int level;
    public int cooldown;
    public int currentCooldown;

    private Active replacedAbility = null;

    public Active(ActiveAbility activeAbility, int abilityLevel)
    {
        this.activeAbility = activeAbility;
        level = abilityLevel;

        if (this.activeAbility != null)
        {
            allyAbilitySources = new List<AbilitySource>();
            enemyAbilitySources = new List<AbilitySource>();

            foreach (AbilitySource source in activeAbility.allyAbilitySources)
            {
                allyAbilitySources.Add(source);
            }

            foreach (AbilitySource source in activeAbility.enemyAbilitySources)
            {
                enemyAbilitySources.Add(source);
            }
        }
    }

    public Active()
    {
        activeAbility = null;
        level = 0;
    }

    public void SetReplacedAbility(Active replacedAbility)
    {
        this.replacedAbility = replacedAbility;
    }

    public Active GetReplacedAbility()
    {
        return replacedAbility;
    }

    public void Initialize()
    {
        cooldown = activeAbility.cooldown;
        currentCooldown = 0;
    }

    public void CoolDown(int rounds)
    {
        currentCooldown -= rounds;

        if (currentCooldown < 0)
        {
            currentCooldown = 0;
        }
    }

    public bool OnCooldown()
    {
        if (currentCooldown > 0)
        {
            return true;
        }
        else
        {
            currentCooldown = 0;
            return false;
        }
    }

    public void PutOnCooldown()
    {
        if (cooldown > 0)
        {
            currentCooldown = cooldown + 1;
        }
    }

    public void SetCurrentCooldown(int amount)
    {
        currentCooldown = amount;
    }
}
