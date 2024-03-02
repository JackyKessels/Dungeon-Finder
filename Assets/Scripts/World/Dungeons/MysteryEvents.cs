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

    public List<MysteryResult> results;

    public MysteryResult TriggerEvent(List<ConsequenceStructure> consequenceStructures, int level)
    {
        foreach (MysteryResult mysteryResult in results)
        {
            if (mysteryResult.MeetAllConditions())
            {
                foreach (MysteryAction mysteryAction in mysteryResult.actions)
                {
                    mysteryAction.TriggerAction(consequenceStructures, level);
                }

                return mysteryResult;
            }
        }

        return null;
    }
}

[System.Serializable]
public class MysteryResult
{
    public List<MysteryEventCondition> conditions;

    [TextArea(5, 5)] public string flavorText;

    public List<MysteryAction> actions;

    public bool MeetAllConditions()
    {
        if (conditions == null || conditions.Count == 0)
        {
            return true;
        }
        else
        {
            return conditions.TrueForAll(c => c.MeetCondition());
        }
    }
}

public enum MysteryEventConditionType
{
    None = 0,
    HasItem = 1,
}

[System.Serializable]
public class MysteryEventCondition
{
    public MysteryEventConditionType type;
    public bool target;

    [Header("Have Item")]
    public ItemObject itemObject;
    public bool consumeItem = false;

    public bool MeetCondition()
    {
        switch (type)
        {
            case MysteryEventConditionType.None:
                {
                    return true;
                }
            case MysteryEventConditionType.HasItem:
                {
                    bool meetCondition = InventoryManager.Instance.HasItemInInventory(itemObject) == target;

                    if (meetCondition)
                    {
                        if (consumeItem)
                        {
                            InventoryManager.Instance.RemoveItemFromInventory(itemObject, 1);
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
        }

        return false;
    }
}

public enum MysteryActionType
{
    Currency,
    Experience,
    Item,
    Source,
    Effect,
    Relocate,
    Battle
}

public enum MysteryActionTarget
{
    Team,
    Random
}

public enum EncounterTeamSetup
{
    Team,
    SoloPicked,
    SoloRandom
}

[System.Serializable]
public class MysteryAction
{
    public MysteryActionType type;

    public MysteryEventCondition condition;

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

    [Header("Battle")]
    public Encounter encounter;
    public EncounterTeamSetup teamSetup = EncounterTeamSetup.Team;

    public void TriggerAction(List<ConsequenceStructure> consequenceStructures, int level)
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

                    Item item = Item.CreateItem(itemObject, level);
                    InventoryManager.Instance.AddItemToInventory(item);

                    Debug.Log("You gained " + itemObject.name + ".");

                    consequenceStructures.Add(new ConsequenceStructure("1", itemObject.icon, null, true, null, itemObject));
                }
                break;
            case MysteryActionType.Source:
                {
                    if (abilitySource == null)
                        return;

                    List<Unit> targets = new List<Unit>();

                    switch (sourceTargets)
                    {
                        case MysteryActionTarget.Team:
                            {
                                targets = AbilityUtilities.GetAbilityTargets(AbilityTargets.Allies, TeamManager.Instance.heroes.LivingMembers[0]);

                                string value = abilitySource.CalculateValue(TeamManager.Instance.heroes.LivingMembers[0], 1, 1, 1).ToString();

                                string sign = abilitySource.school == AbilitySchool.Healing ? "+ " : "- ";

                                consequenceStructures.Add(new ConsequenceStructure(sign + value + " Health", GeneralUtilities.GetSchoolIcon(abilitySource.school), GameAssets.i.threeMembers));
                            }
                            break;
                        case MysteryActionTarget.Random:
                            {
                                targets = AbilityUtilities.GetAbilityTargets(AbilityTargets.RandomAlly, TeamManager.Instance.heroes.LivingMembers[0]);

                                string value = abilitySource.CalculateValue(targets[0], 1, 1, 1).ToString();

                                string sign = abilitySource.school == AbilitySchool.Healing ? "+ " : "- ";

                                consequenceStructures.Add(new ConsequenceStructure(sign + value + " Health", GeneralUtilities.GetSchoolIcon(abilitySource.school), targets[0].icon));
                            }
                            break;
                        default:
                            break;
                    }

                    foreach (Unit target in targets)
                    {
                        target.statsManager.TakeDamage(abilitySource, false);
                    }

                    CheckParty();
                }
                break;
            case MysteryActionType.Effect:
                {
                    if (effect == null)
                        return;

                    List<Unit> targets = new List<Unit>();

                    switch (effectTargets)
                    {
                        case MysteryActionTarget.Team:
                            {
                                targets = AbilityUtilities.GetAbilityTargets(AbilityTargets.Allies, TeamManager.Instance.heroes.LivingMembers[0]);

                                consequenceStructures.Add(new ConsequenceStructure(EffectObject.DurationText(effect), effect.icon, GameAssets.i.threeMembers, true, effect));
                            }
                            break;
                        case MysteryActionTarget.Random:
                            {
                                targets = AbilityUtilities.GetAbilityTargets(AbilityTargets.RandomAlly, TeamManager.Instance.heroes.LivingMembers[0]);

                                consequenceStructures.Add(new ConsequenceStructure(EffectObject.DurationText(effect), effect.icon, targets[0].icon, true, effect));
                            }
                            break;
                        default:
                            break;
                    }

                    foreach (Unit target in targets)
                    {
                        target.effectManager.PreparePreBattleEffect(effect);
                    }

                }
                break;
            case MysteryActionType.Relocate:
                {
                    DungeonManager dungeonManager = DungeonManager.Instance;

                    Location randomLocation = dungeonManager.gridHandler.GetRandomFloorLocation(true);

                    dungeonManager.player.SetCurrentLocation(randomLocation);
                    dungeonManager.gridHandler.LockUnreachableLocations(randomLocation);
                    dungeonManager.gridHandler.RefreshLocations(randomLocation);

                    randomLocation.SetVisited();

                    consequenceStructures.Add(new ConsequenceStructure("You have been moved to a random location.", null, null));
                }
                break;
            case MysteryActionType.Battle:
                {
                    switch (teamSetup)
                    {
                        case EncounterTeamSetup.Team:
                            {
                                List<(EnemyObject, int)> enemyUnits = Encounter.SetupUnitObjects(encounter, level, level + 1);
                                BattleManager.Instance.StartBattle(enemyUnits);
                            }
                            break;
                        case EncounterTeamSetup.SoloPicked:
                            break;
                        case EncounterTeamSetup.SoloRandom:
                            {
                                Unit solo = AbilityUtilities.GetRandomUnit(TeamManager.Instance.heroes);
                            }
                            break;
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


