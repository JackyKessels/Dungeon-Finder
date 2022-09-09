using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Unit
{
    [HideInInspector] public EnemyObject enemyObject;

    public List<Active> chargedAbility;
    private int chargeTarget;

    public void UpdateUnit(EnemyObject unitObject)
    {
        // set correct data from the UnitObject
        enemyObject = unitObject;
        name = enemyObject.name;
        unitType = enemyObject.unitType;
        icon = enemyObject.icon;
        sprite = enemyObject.sprite;
        unitRenderer.sprite = unitObject.sprite;

        statsManager = new StatsManager(this, enemyObject);

        spellbook = new Spellbook(this);
        spellbook.SetDefaultAbilities(enemyObject);

        isEnemy = true;
        unitRenderer.flipX = true;

        chargedAbility = new List<Active>();
        chargeTarget = -1;
    }

    public override UnitObject GetUnitObject()
    {
        return enemyObject;
    }

    public int GetCurrencyAmount(CurrencyType currencyType)
    {
        int totalCurrency = 0;

        for (int i = 0; i < enemyObject.currencyRewards.Count; i++)
        {
            if (enemyObject.currencyRewards[i].currencyType == currencyType)
            {
                totalCurrency += enemyObject.currencyRewards[i].totalAmount;
                break;
            }
        }

        return totalCurrency;
    }

    private bool IsChargedAbility(Active active)
    {
        AbilityBehavior abilityBehavior = GetAbilityBehavior(active);

        if (abilityBehavior != null && abilityBehavior.charged)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCharging(Active active)
    {
        return IsChargedAbility(active) && chargedAbility.Count == 0;
    }

    public void ChargeTarget(Active active)
    {
        chargedAbility.Add(active);

        if (active.activeAbility is TargetAbility t)
        {
            chargeTarget = GetTarget(t);

            Effect effect = new Effect();
            effect.Setup(GameAssets.i.chargeTargetEffect, this, TeamManager.Instance.heroes.GetUnit(chargeTarget), 1);
            effect.IconOverride = active.activeAbility.icon;
            effect.target.effectManager.OnApplication(effect);
        }
        else
        {
            Effect effect = new Effect();
            effect.Setup(GameAssets.i.chargeInstantEffect, this, this, 1);
            effect.IconOverride = active.activeAbility.icon;
            effect.target.effectManager.OnApplication(effect);
        }
    }

    public int CheckTarget(TargetAbility t)
    {
        // If there is no target or if the target is dead, change target
        if (chargeTarget == -1 || TeamManager.Instance.heroes.GetUnit(chargeTarget).statsManager.isDead)
            return GetTarget(t);
        else
            return chargeTarget;

    }

    public void ResetChargedAbility()
    {
        chargedAbility.Clear();
        chargeTarget = -1;
        effectManager.RemoveCharging();
    }

    private AbilityBehavior GetAbilityBehavior(Active active)
    {
        foreach (AbilityBehavior abilityBehavior in enemyObject.abilities)
        {
            if (abilityBehavior.ability == active.activeAbility)
                return abilityBehavior;
        }

        return null;
    }

    public Active ChooseValidAbility()
    {
        // If no ability is being currently charged
        if (chargedAbility.Count == 0)
        {
            if (enemyObject.randomAbility)
            {
                List<Active> validAbilities = new List<Active>();

                foreach (Active active in spellbook.abilityCollection)
                {
                    if (!active.OnCooldown())
                    {
                        validAbilities.Add(active);
                    }
                }

                if (validAbilities.Count > 0)
                {
                    int selectRandomAbility = Random.Range(0, validAbilities.Count);

                    return validAbilities[selectRandomAbility];
                }
            }
            else
            {
                for (int i = 0; i < spellbook.abilityCollection.Count; i++)
                {
                    if (!spellbook.abilityCollection[i].OnCooldown() && MeetsCastCondition(spellbook.abilityCollection[i].activeAbility))
                    {
                        return spellbook.abilityCollection[i];
                    }
                }
            }
        }
        else
        {
            return chargedAbility[0];
        }

        return new Active(null, 0);
    }

    public Active GetOffCooldownSwiftAbility()
    {
        List<Active> swiftAbilities = new List<Active>();

        foreach (Active active in spellbook.abilityCollection)
        {
            if (!active.activeAbility.endTurn)
            {
                swiftAbilities.Add(active);
            }
        }

        // No swift abilities at all
        if (swiftAbilities.Count <= 0)
        {
            return null;
        }

        List<Active> offCooldown = new List<Active>();

        foreach (Active active in swiftAbilities)
        {
            if (!active.OnCooldown() && MeetsCastCondition(active.activeAbility))
            {
                offCooldown.Add(active);
            }    
        }

        // Use first swift ability -> will not use multiple swift abilities for now
        if (offCooldown.Count <= 0)
            return null;
        else
            return offCooldown[0];
    }

    public int GetTarget(TargetAbility t)
    {
        int target = 0;
        List<Unit> targetGroup = new List<Unit>();

        if (t.targetsAllies)
        {
            targetGroup = TeamManager.Instance.enemies.LivingMembers;
        }
        else if (t.targetsEnemies)
        {
            targetGroup = TeamManager.Instance.heroes.LivingMembers;
        }

        if (effectManager.TauntedBy() != null)
        {
            target = effectManager.TauntedBy().battleNumber;
        }
        else
        {
            TargetCondition targetCondition = GetTargetCondition(t);

            switch (targetCondition)
            {
                case TargetCondition.Random:
                    {
                        target = Random.Range(0, targetGroup.Count);
                    }
                    break;
                case TargetCondition.LowestHealth:
                    {
                        if (targetGroup.Count == 1)
                        {
                            target = 0;
                        }
                        else
                        {
                            int lowest = 0;

                            for (int i = 0; i < targetGroup.Count; i++)
                            {
                                if (targetGroup[i].statsManager.currentHealth < targetGroup[lowest].statsManager.currentHealth)
                                    lowest = i;
                            }

                            target = lowest;
                        }
                    }
                    break;
                case TargetCondition.HighestHealth:
                    {
                        if (targetGroup.Count == 1)
                        {
                            target = 0;
                        }
                        else
                        {
                            int highest = 0;

                            for (int i = 0; i < targetGroup.Count; i++)
                            {
                                if (targetGroup[i].statsManager.currentHealth > targetGroup[highest].statsManager.currentHealth)
                                    highest = i;
                            }

                            target = highest;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        return target;
    }

    private bool MeetsCastCondition(ActiveAbility ability)
    {
        CastCondition castCondition = GetCastCondition(ability);

        switch (castCondition)
        {
            case CastCondition.Nothing:
                {
                    return true;
                }
            case CastCondition.BelowHalfHealth:
                {
                    if (statsManager.currentHealth < statsManager.GetAttributeValue((int)AttributeType.Health) * .5)
                        return true;
                    else
                        return false;
                }
            case CastCondition.AboveHalfHealth:
                {
                    if (statsManager.currentHealth > statsManager.GetAttributeValue((int)AttributeType.Health) * .5)
                        return true;
                    else
                        return false;
                }
            default:
                return true;
        }
    }

    private TargetCondition GetTargetCondition(TargetAbility ability)
    {
        foreach (AbilityBehavior abilityBehavior in enemyObject.abilities)
        {
            if (abilityBehavior.ability == ability)
            {
                return abilityBehavior.target;
            }
        }

        return TargetCondition.Random;
    }

    private CastCondition GetCastCondition(ActiveAbility ability)
    {
        foreach (AbilityBehavior abilityBehavior in enemyObject.abilities)
        {
            if (abilityBehavior.ability == ability)
            {
                return abilityBehavior.condition;
            }
        }

        return CastCondition.Nothing;
    }

    public void SetStartCooldowns()
    {
        foreach (AbilityBehavior abilityBehavior in enemyObject.abilities)
        {
            Active active = spellbook.FindCollectionAbility(abilityBehavior.ability);

            active.SetCurrentCooldown(abilityBehavior.startCooldown);
        }
    }
}
