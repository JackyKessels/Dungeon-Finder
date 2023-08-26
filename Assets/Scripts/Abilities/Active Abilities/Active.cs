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

    public Active()
    {
        activeAbility = null;
        level = 0;
    }

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

    public void Trigger(Unit caster, Unit target, float effectiveness)
    {
        activeAbility.TriggerAbility(caster, target, level, effectiveness);
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
        if (currentCooldown == ActiveAbility.SINGLE_USE_COOLDOWN)
        {
            return;
        }
        else
        {
            currentCooldown -= rounds;

            if (currentCooldown < 0)
            {
                currentCooldown = 0;
            }
        }
    }

    public bool IsOnCooldown()
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
        if (activeAbility != null && activeAbility.singleUse)
        {
            currentCooldown = ActiveAbility.SINGLE_USE_COOLDOWN;
        }
        else
        {
            currentCooldown = cooldown + 1;
        }
    }

    public void SetCurrentCooldown(int amount)
    {
        currentCooldown = amount;
    }
}
