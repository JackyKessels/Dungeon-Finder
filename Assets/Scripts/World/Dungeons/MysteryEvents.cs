using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mystery Event", menuName = "World/Mystery Event")]
public class MysteryEvents : ScriptableObject
{
    public List<MysteryEvent> events;

    public MysteryEvent GetRandomEvent()
    {
        if (events.Count == 0)
            return null;

        List<int> weights = new List<int>();

        foreach (MysteryEvent mysteryEvent in events)
        {
            weights.Add(mysteryEvent.eventWeight);
        }

        return events[GeneralUtilities.RandomWeighted(weights)];
    }
}

[System.Serializable]
public class MysteryEvent
{
    public int eventWeight = 1;

    [TextArea(5, 5)] public string flavorText;

    public List<MysteryAction> actions;

    public void TriggerEvent(List<ConsequenceStructure> consequenceStructures)
    {
        foreach (MysteryAction mysteryAction in actions)
        {
            mysteryAction.TriggerAction(consequenceStructures);

        }
    }
}

public enum MysteryActionType
{
    Currency,
    Experience,
    Item,
    Source,
    Effect,
}

public enum MysteryActionTarget
{
    Team,
    Random
}

[System.Serializable]
public class MysteryAction
{
    public MysteryActionType type;

    [Header("Currency")]
    public int currencyAmount;
    public CurrencyType currencyType;

    [Header("Experience")]
    public int experienceAmount;

    [Header("Item")]
    public ItemObject itemObject;

    [Header("Source")]
    public MysteryActionTarget sourceTargets; 
    public AbilitySource abilitySource;

    [Header("Effect")]
    public MysteryActionTarget effectTargets;
    public EffectObject effect;

    public void TriggerAction(List<ConsequenceStructure> consequenceStructures)
    {
        Debug.Log("Mystery action: " + type.ToString());

        switch (type)
        {
            case MysteryActionType.Currency:
                {
                    if (currencyAmount == 0)
                        return;

                    Currency currency = new Currency(currencyType, currencyAmount);

                    if (currencyAmount > 0)
                    {
                        GameManager.Instance.currencyHandler.IncreaseCurrency(currency);

                        Debug.Log("You gained " + currencyAmount + " " + Currency.GetCurrencyName(currencyType) + ".");

                        consequenceStructures.Add(new ConsequenceStructure("+" + currencyAmount, Currency.GetCurrencyIcon(currencyType, true), null));
                    }
                    else
                    {
                        GameManager.Instance.currencyHandler.DecreaseCurrency(currency);

                        Debug.Log("You lost " + currencyAmount + " " + Currency.GetCurrencyName(currencyType) + ".");

                        consequenceStructures.Add(new ConsequenceStructure("-" + currencyAmount, Currency.GetCurrencyIcon(currencyType, true), null));
                    }                   
                }
                break;
            case MysteryActionType.Experience:
                {
                    if (experienceAmount < 0)
                        return;

                    TeamManager.Instance.RewardExperienceToTeam(experienceAmount);

                    Debug.Log("You gained " + experienceAmount + " experience.");

                    consequenceStructures.Add(new ConsequenceStructure(experienceAmount.ToString(), GameAssets.i.experienceIcon, null));
                }
                break;
            case MysteryActionType.Item:
                {
                    if (itemObject == null)
                        return;

                    InventoryManager.Instance.AddItemToInventory(itemObject);

                    Debug.Log("You gained " + itemObject.name + ".");

                    consequenceStructures.Add(new ConsequenceStructure("1", itemObject.icon, null));
                }
                break;
            case MysteryActionType.Source:
                {
                    List<Unit> targets = new List<Unit>();

                    switch (sourceTargets)
                    {
                        case MysteryActionTarget.Team:
                            {
                                targets = AbilityUtilities.GetInstantTargets(InstantTargets.Allies, TeamManager.Instance.heroes.LivingMembers[0]);

                                consequenceStructures.Add(new ConsequenceStructure(abilitySource.CalculateValue(TeamManager.Instance.heroes.LivingMembers[0], 1, 1, 1).ToString(), GeneralUtilities.GetSchoolIcon(abilitySource.school), GameAssets.i.threeMembers));
                            }
                            break;
                        case MysteryActionTarget.Random:
                            {
                                targets = AbilityUtilities.GetInstantTargets(InstantTargets.RandomAlly, TeamManager.Instance.heroes.LivingMembers[0]);

                                consequenceStructures.Add(new ConsequenceStructure(abilitySource.CalculateValue(targets[0], 1, 1, 1).ToString(), GeneralUtilities.GetSchoolIcon(abilitySource.school), targets[0].icon));
                            }
                            break;
                        default:
                            break;
                    }

                    foreach (Unit target in targets)
                    {
                        int current = target.statsManager.currentHealth;
                        Debug.Log(target.name + " has " + current + " health.");

                        abilitySource.TriggerSource(target, target, 1, 1, true, 1, AbilityType.Assault);

                        Debug.Log(target.name + " takes " + abilitySource.CalculateValue(target, 1, 1, 1) + " " + abilitySource.school.ToString() + " damage.");
                    }

                    CheckParty();
                }
                break;
            case MysteryActionType.Effect:
                {
                    if (effect != null)
                    {
                        List<Unit> targets = new List<Unit>();

                        switch (effectTargets)
                        {
                            case MysteryActionTarget.Team:
                                {
                                    targets = AbilityUtilities.GetInstantTargets(InstantTargets.Allies, TeamManager.Instance.heroes.LivingMembers[0]);

                                    consequenceStructures.Add(new ConsequenceStructure(EffectObject.DurationText(effect), effect.icon, GameAssets.i.threeMembers, true, effect));
                                }
                                break;
                            case MysteryActionTarget.Random:
                                {
                                    targets = AbilityUtilities.GetInstantTargets(InstantTargets.RandomAlly, TeamManager.Instance.heroes.LivingMembers[0]);

                                    consequenceStructures.Add(new ConsequenceStructure(EffectObject.DurationText(effect), effect.icon, targets[0].icon, true, effect));
                                }
                                break;
                            default:
                                break;
                        }

                        foreach (Unit target in targets)
                        {
                            target.effectManager.ApplyPreBattleEffect(effect, target, 1);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    private void CheckParty()
    {
        if (TeamManager.Instance.heroes.LivingMembers.Count == 0)
        {
            GameManager.Instance.BattleLost();
        }
    }
}