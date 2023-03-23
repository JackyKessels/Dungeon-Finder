using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    #region Singleton
    public static RewardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private TeamManager teamManager;

    public RewardDatabase database;

    [Header("Variables")]
    public GameObject rewardsObject;
    public TextMeshProUGUI rewardTitle;
    public GameObject rewardContainer;
    public GameObject rewardPrefab;

    private List<Reward> rewards;

    private int heroesToReward;
    private int nextHero;

    private Button activeButton;

    private void Start()
    {
        teamManager = TeamManager.Instance;
    }

    public void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            if (activeButton)
                activeButton.onClick?.Invoke();
        }
    }

    public void Setup()
    {
        rewardsObject.SetActive(true);
        rewards = new List<Reward>();

        heroesToReward = teamManager.heroes.Members.Count;
        nextHero = 0;
    }

    public void SetupBattleResult()
    {
        int experience = GetTotalExperienceReward();

        List<Currency> currencies = new List<Currency>();

        currencies.Add(GetCurrencyReward(CurrencyType.Gold));
        currencies.Add(GetCurrencyReward(CurrencyType.Spirit));

        List<ItemDrop> itemDrops = GetItemDrops();

        BattleResultWindow.SendBattleResult(experience, currencies, itemDrops);
    }

    public List<ItemDrop> GetItemDrops()
    {
        List<ItemDrop> itemDrops = new List<ItemDrop>();

        foreach (Unit unit in teamManager.enemies.killedMembers)
        {
            Enemy enemy = unit as Enemy;

            ItemDrop droppedItem = ItemDrop.WeightedDrops(enemy.enemyObject.itemDrops);

            if (droppedItem != null && droppedItem.itemObject != null)
            {
                if (itemDrops.Count == 0)
                {
                    itemDrops.Add(droppedItem);
                }
                else
                {
                    for (int i = 0; i < itemDrops.Count; i++)
                    {
                        if (itemDrops[i].itemObject == droppedItem.itemObject)
                        {
                            itemDrops[i].amount += droppedItem.amount;
                        }
                        else
                        {
                            itemDrops.Add(droppedItem);
                        }
                    }
                }
            }
        }

        return itemDrops;
    }


    // Calculates the total experience points from all defeated enemies.
    public int GetTotalExperienceReward()
    {
        if (teamManager.enemies.killedMembers.Count == 0)
            return 0;

        int totalExperience = 0;

        foreach (Unit unit in teamManager.enemies.killedMembers)
        {
            Enemy enemy = unit as Enemy;

            totalExperience += enemy.enemyObject.experienceReward;
        }

        return totalExperience;
    }

    public Currency GetCurrencyReward(CurrencyType currencyType)
    {
        if (teamManager.enemies.killedMembers.Count == 0)
            return new Currency(currencyType, 0);

        int totalCurrency = 0;

        foreach (Unit unit in teamManager.enemies.killedMembers)
        {
            Enemy enemy = unit as Enemy;

            totalCurrency += enemy.GetCurrencyAmount(currencyType);
        }

        return new Currency(currencyType, totalCurrency);
    }

    public void GenerateLootTable(bool isChoice, List<ItemDrop> lootTable, int min, int max)
    {
        Setup();

        rewardTitle.text = "Choose an item";

        if (lootTable.Count == 0)
            return;

        // isChoice NYI

        int amount = Random.Range(min, max + 1);

        while (rewards.Count < Mathf.Min(amount, lootTable.Count))
        {
            ItemDrop droppedItem = ItemDrop.WeightedDrops(lootTable);

            if (droppedItem.itemObject != null)
            {
                AddItem(droppedItem);
            }
        }

        ListToRewards();
    }

    public ActiveAbility GenerateRandomAbility(Unit unit, List<ActiveAbility> abilityTable)
    {
        int randomAbility = Random.Range(0, abilityTable.Count);
        ActiveAbility ability = abilityTable[randomAbility];
        SelectAbility(unit, ability);
        Debug.Log("Added " + ability.name + " to " + unit.name);
        rewardsObject.SetActive(false);

        return ability;
    }

    private void AddAbility(ActiveAbility ability)
    {
        bool isInList = false;

        for (int i = 0; i < rewards.Count; i++)
        {
            if (rewards[i].ability.activeAbility == ability)
            {
                isInList = true;
            }
        }

        if (isInList == false)
        {
            Reward abilityReward = new Reward(ability);
            rewards.Add(abilityReward);
        }
    }

    private void AddItem(ItemDrop itemDrop)
    {
        bool isInList = false;

        for (int i = 0; i < rewards.Count; i++)
        {
            if (rewards[i].itemDrop == itemDrop)
            {
                isInList = true;
            }
        }

        if (isInList == false)
        {
            Reward itemReward = new Reward(itemDrop);
            rewards.Add(itemReward);
        }
    }

    private void ListToRewards()
    {
        foreach (Reward r in rewards)
        {
            AddRewardButton(r);
        }
    }

    private void AddRewardButton(Reward reward)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(rewardPrefab, rewardContainer);

        RewardObject r = obj.GetComponent<RewardObject>();
        r.SetData(reward);

        Button b = obj.GetComponent<Button>();
        if (reward.itemDrop != null && reward.itemDrop.amount > 1)
        {
            b.GetComponentInChildren<TextMeshProUGUI>().text = reward.itemDrop.amount.ToString();
        }
        else if (reward.attribute != null)
        {
            b.GetComponentInChildren<TextMeshProUGUI>().text = reward.attribute.baseValue.ToString();
        }
        b.onClick.AddListener(delegate { SelectReward(b, reward.type); });
    }

    private int GenerateNumberOfRewards(int min, int max)
    {
        return Random.Range(min, max);
    }

    private void SelectReward(Button button, RewardType type)
    {
        switch (type)
        {
            case RewardType.Ability:
                //SelectAbility(button.GetComponent<RewardObject>().reward.ability.activeAbility);
                break;
            case RewardType.Item:
                SelectItem(button.GetComponent<RewardObject>().reward.itemDrop);
                break;
            case RewardType.Currency:

                Debug.Log("Added 500 Squidpounds.");
                break;
            case RewardType.Attribute:
                SelectAttribute(button.GetComponent<RewardObject>().reward.attribute);
                break;
        }

        ObjectUtilities.ClearContainer(rewardContainer);

        if (type == RewardType.Item || type == RewardType.Currency)
        {
            rewardsObject.SetActive(false);
            TooltipHandler.Instance.HideTooltip();
        }
        else
        {
            heroesToReward--;
            nextHero++;

            if (heroesToReward == 0)
            {
                // Close menu
                rewardsObject.SetActive(false);
                TooltipHandler.Instance.HideTooltip();
            }
            else
            {
                rewardTitle.text = teamManager.heroes.Members[nextHero].name;
            }
        }
    }

    public void SkipRewards()
    {
        rewardsObject.SetActive(false);
        TooltipHandler.Instance.HideTooltip();
    }

    private void SelectAbility(Unit unit, ActiveAbility ability)
    {
        unit.spellbook.LearnAbility(new Active(ability, 1));
    }

    private void SelectItem(ItemDrop itemDrop)
    {
        InventoryManager.Instance.AddItemToInventory(itemDrop.itemObject, itemDrop.amount);
        InventoryManager.Instance.inventory.UpdateItems();
    }

    private void SelectAttribute(Attribute attribute)
    {
        Unit hero = teamManager.heroes.Members[nextHero];

        hero.statsManager.ModifyAttribute(attribute.attributeType, AttributeValue.bonusValue, attribute.bonusValue);
    }

    public Hero CurrentHero()
    {
        return teamManager.heroes.Members[nextHero] as Hero;
    }
}

[System.Serializable]
public class Reward
{
    public string name;
    public Sprite icon;
    public RewardType type;
    public Active ability = null;
    public ItemDrop itemDrop = null;
    public Attribute attribute = null;
    public int currency = 0;
    public CurrencyType currencyType;

    public Reward(ActiveAbility a)
    {
        ability = new Active(a, 1);

        name = a.name;
        icon = a.icon;
        type = RewardType.Ability;
    }

    public Reward(AttributeType t)
    {
        attribute = new Attribute(t);

        name = t.ToString();
        icon = GeneralUtilities.GetAttributeIcon(t);
        type = RewardType.Attribute;
    }

    public Reward(ItemDrop i)
    {
        itemDrop = i;

        name = i.itemObject.name;
        icon = i.itemObject.icon;
        type = RewardType.Item;
    }

    public Reward(Currency c)
    {
        currency = c.totalAmount;
        currencyType = c.currencyType;

        name = Currency.GetCurrencyName(c.currencyType);
        icon = Currency.GetCurrencyIcon(c.currencyType, false);
        type = RewardType.Currency;
    }

    public bool IsCurrency()
    {
        if (currency == 0)
            return false;
        else
            return true;
    }
}
