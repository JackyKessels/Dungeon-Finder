using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Unit
{
    [HideInInspector] public EnemyObject enemyObject;

    public int Level { get; set; }

    public List<Active> chargedAbility = new();
    private int chargeTarget = -1;
    private bool isCharging => chargedAbility.Count > 0;

    public void Setup(EnemyObject unitObject, int level)
    {
        // set correct data from the UnitObject
        enemyObject = unitObject;
        Level = level;
        name = enemyObject.name;
        unitType = enemyObject.unitType;
        icon = enemyObject.icon;
        sprite = enemyObject.sprite;
        unitRenderer.sprite = unitObject.sprite;

        statsManager = new StatsManager(this, enemyObject, Level);

        spellbook = new Spellbook(this);
        spellbook.SetDefaultAbilities(enemyObject);

        isEnemy = true;
        unitRenderer.flipX = true;

        chargedAbility = new();
        chargeTarget = -1;

        battlePhaseController?.Initialize(this);
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

    public bool CanStartCharging(Active active)
    {
        return IsChargedAbility(active) && !isCharging;
    }

    public void StartCharging(Active active)
    {
        chargedAbility.Add(active);

        if (active.activeAbility is TargetAbility t)
        {
            chargeTarget = GetTarget(t);

            ApplyChargeEffect(this, TeamManager.Instance.heroes.LivingMembers[chargeTarget]);
        }
        else
        {
            ApplyChargeEffect(this, this);
        }
    }

    public void ApplyChargeEffect(Unit caster, Unit target)
    {
        if (chargedAbility.Count == 0)
        {
            return;
        }

        var activeAbility = chargedAbility[0].activeAbility;

        Effect effect = new Effect(activeAbility.GetChargeEffectObject(), 1, caster, target, 1, activeAbility);
        effect.IconOverride = activeAbility.icon;
        effect.target.effectManager.OnApplication(effect);
    }

    public int CheckTarget(TargetAbility t)
    {
        if (effectManager.TauntedBy() != null)
        {
            return effectManager.TauntedBy().battleNumber;
        }

        if (chargeTarget == -1 || TeamManager.Instance.heroes.GetUnitAlive(chargeTarget) == null)
        {
            return GetTarget(t);
        }

        return chargeTarget;
    }

    public void ResetChargedAbility()
    {
        chargedAbility.Clear();
        chargeTarget = -1;
        EffectManager.RemoveChargeEffects();
    }

    private AbilityBehavior GetAbilityBehavior(Active active)
    {
        var abilityBehaviors = battlePhaseController.GetCurrentActiveAbilities();

        foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
        {
            if (abilityBehavior.ability == active.activeAbility)
                return abilityBehavior;
        }

        return null;
    }

    private bool UseRandomAbilities()
    {
        return battlePhaseController.UseRandomAbilities();
    }

    public Active ChooseValidAbility()
    {
        if (!isCharging)
        {
            if (UseRandomAbilities())
            {
                var suppressions = effectManager.GetAbilitySuppressions();

                List<Active> validAbilities = new List<Active>();

                foreach (Active active in spellbook.abilityCollection)
                {
                    if (!active.IsOnCooldown() && !AbilityUtilities.IsSuppressed(active, suppressions))
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
                    if (!spellbook.abilityCollection[i].IsOnCooldown() && MeetsCastCondition(spellbook.abilityCollection[i].activeAbility))
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

        return null;
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

        var suppressions = effectManager.GetAbilitySuppressions();

        foreach (Active active in swiftAbilities)
        {
            if (!active.IsOnCooldown() && MeetsCastCondition(active.activeAbility) && !AbilityUtilities.IsSuppressed(active, suppressions))
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
            (TargetCondition targetCondition, TargetAttribute targetAttribute) = GetTargetCondition(t);

            switch (targetCondition)
            {
                case TargetCondition.Random:
                    {
                        target = Random.Range(0, targetGroup.Count);
                    }
                    break;
                case TargetCondition.LowestAttribute:
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
                                if (targetGroup[i].statsManager.GetAttributeValue(targetAttribute) < targetGroup[lowest].statsManager.GetAttributeValue(targetAttribute))
                                    lowest = i;
                            }

                            target = lowest;
                        }
                    }
                    break;
                case TargetCondition.HighestAttribute:
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
                                if (targetGroup[i].statsManager.GetAttributeValue(targetAttribute) > targetGroup[highest].statsManager.GetAttributeValue(targetAttribute))
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
        (CastCondition castCondition, int healthThreshold) = GetCastCondition(ability);

        switch (castCondition)
        {
            case CastCondition.Nothing:
                {
                    return true;
                }
            case CastCondition.BelowHealthThreshold:
                {
                    float threshold = statsManager.GetAttributeValue(AttributeType.Health) * (float)healthThreshold / 100;

                    if (statsManager.currentHealth <= threshold)
                        return true;
                    else
                        return false;
                }
            case CastCondition.AboveHealthThreshold:
                {
                    float threshold = statsManager.GetAttributeValue(AttributeType.Health) * (float)healthThreshold / 100;

                    if (statsManager.currentHealth >= threshold)
                        return true;
                    else
                        return false;
                }
            default:
                return true;
        }
    }

    private (TargetCondition, TargetAttribute) GetTargetCondition(TargetAbility ability)
    {
        var abilityBehaviors = battlePhaseController.GetCurrentActiveAbilities();

        foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
        {
            if (abilityBehavior.ability == ability)
            {
                return (abilityBehavior.target, abilityBehavior.targetAttribute);
            }
        }

        return (TargetCondition.Random, TargetAttribute.CurrentHealth);
    }

    private (CastCondition, int) GetCastCondition(ActiveAbility ability)
    {
        var abilityBehaviors = battlePhaseController.GetCurrentActiveAbilities();

        foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
        {
            if (abilityBehavior.ability == ability)
            {
                return (abilityBehavior.condition, abilityBehavior.healthThreshold);
            }
        }

        return (CastCondition.Nothing, 0);
    }

    public void SetStartCooldowns()
    {
        var abilityBehaviors = battlePhaseController.GetCurrentActiveAbilities();

        foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
        {
            Active active = spellbook.FindCollectionAbility(abilityBehavior.ability);

            active.cooldown = active.activeAbility.cooldown;
            active.SetCurrentCooldown(abilityBehavior.startCooldown);
        }
    }

    public void CancelChargedAbility(bool putOnCooldown)
    {
        if (!isCharging)
        {
            return;
        }

        Active active = chargedAbility[0];

        ResetChargedAbility();

        if (putOnCooldown)
        {
            active.PutOnCooldown();
        }
    }
}
