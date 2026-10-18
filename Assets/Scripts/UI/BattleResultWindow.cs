using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultWindow : MonoBehaviour
{
    public GameObject title;
    public GameObject container;
    public GameObject remainingLootContainer;
    public GameObject rewardPrefab;
    public GameObject remainingLootPrefab;
    public GameObject experiencePrefab;
    public GameObject continueButtonPrefab;
    public GameObject discardButtonPrefab;
    public GameObject unlootedItemsTitle;

    private Button activeButton;

    private int experienceReward;
    private int rewardCount = 0;
    private List<RemainingLoot> remainingLoot = new List<RemainingLoot>();

    public void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            if (activeButton)
                activeButton.onClick?.Invoke();
        }
    }

    public void Setup(int experience, List<Currency> currencies, List<ItemDrop> itemDrops)
    {
        experienceReward = experience;
        rewardCount = 0;
        remainingLoot.Clear();

        if (experience > 0)
        {
            AddBattleReward(experience);

            rewardCount++;
        }

        if (currencies.Count > 0)
        {
            foreach (Currency currency in currencies)
            {
                if (currency.totalAmount > 0)
                {
                    AddBattleReward(currency);

                    GameManager.Instance.currencyHandler.IncreaseCurrency(currency);

                    rewardCount++;
                }
            }
        }

        if (itemDrops.Count > 0)
        {
            foreach (ItemDrop itemDrop in itemDrops)
            {
                var result = InventoryManager.Instance.AddItemToInventory(itemDrop.GetItem(), itemDrop.amount);

                if (result.added > 0)
                {
                    AddBattleReward(itemDrop, result.added);
                    rewardCount++;
                }

                if (result.remaining > 0)
                {
                    remainingLoot.Add(new RemainingLoot(itemDrop, result.remaining));
                }
            }
        }

        if (remainingLoot.Count > 0)
        {
            ShowRemainingLoot();
        }
        else
        {
            ShowContinueButton();
        }

        MoveBattleResult();
    }

    private void ShowRemainingLoot()
    {
        unlootedItemsTitle.SetActive(true);

        ObjectUtilities.ClearContainer(remainingLootContainer);

        foreach (RemainingLoot loot in remainingLoot)
        {
            AddRemainingLoot(loot);
        }

        GameObject obj = ObjectUtilities.CreateSimplePrefab(discardButtonPrefab, remainingLootContainer);

        Button discardButton = obj.GetComponent<Button>();
        discardButton.onClick.AddListener(DiscardRemainingLoot);

        activeButton = discardButton;
    }

    private void AddRemainingLoot(RemainingLoot loot)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(remainingLootPrefab, remainingLootContainer);

        BattleRewardObject battleReward = obj.GetComponent<BattleRewardObject>();
        battleReward.Setup(loot, () => CollectRemainingLoot(loot));
    }

    private void CollectRemainingLoot(RemainingLoot loot)
    {
        var result = InventoryManager.Instance.AddItemToInventory(loot.item, loot.amount);

        loot.amount = result.remaining;

        if (loot.amount <= 0)
        {
            remainingLoot.Remove(loot);
        }

        if (remainingLoot.Count == 0)
        {
            ObjectUtilities.ClearContainer(remainingLootContainer);
            ShowContinueButton();
        }
        else
        {
            ShowRemainingLoot();
        }
    }

    private void DiscardRemainingLoot()
    {
        remainingLoot.Clear();
        ObjectUtilities.ClearContainer(remainingLootContainer);

        ShowContinueButton();
    }

    private void ShowContinueButton()
    {
        unlootedItemsTitle.SetActive(false);

        Button continueButton = AddContinueButton();

        if (experienceReward > 0)
            continueButton.onClick.AddListener(() => NextWindow(experienceReward));
        else
            continueButton.onClick.AddListener(CloseWindow);

        activeButton = continueButton;
    }

    public static void SendBattleResult(int experience, List<Currency> currencies, List<ItemDrop> itemDrops)
    {
        GameObject container = GameObject.Find("Battle Result Container");

        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.battleResultPrefab.gameObject, container);

        BattleResultWindow battleResult = obj.GetComponent<BattleResultWindow>();
        battleResult.Setup(experience, currencies, itemDrops);
    }

    private void NextWindow(int experienceReward)
    {
        ObjectUtilities.ClearContainer(container);

        AddBattleReward(experienceReward);

        AddBattleExperienceBar(experienceReward);

        Button continueButton = AddContinueButton();

        continueButton.onClick.AddListener(delegate { CloseWindow(); });

        activeButton = continueButton;
    }

    private Button AddContinueButton()
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(continueButtonPrefab, container);

        return obj.GetComponent<Button>();
    }

    private void AddBattleReward(int experience)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(rewardPrefab, container);

        BattleRewardObject battleReward = obj.GetComponent<BattleRewardObject>();
        battleReward.Setup(experience);
    }

    private void AddBattleReward(Currency currency)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(rewardPrefab, container);

        BattleRewardObject battleReward = obj.GetComponent<BattleRewardObject>();
        battleReward.Setup(currency);
    }

    private void AddBattleReward(ItemDrop itemDrop, int amount)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(rewardPrefab, container);

        BattleRewardObject battleReward = obj.GetComponent<BattleRewardObject>();
        battleReward.Setup(itemDrop, amount);
    }

    private void AddBattleExperienceBar(int experienceReward)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(experiencePrefab, container);

        BattleExperienceBar battleExperienceBar = obj.GetComponent<BattleExperienceBar>();
        battleExperienceBar.Setup(experienceReward);
    }

    private void CloseWindow()
    {
        Destroy(gameObject);

        SaveManager.Instance.SaveGame();
    }

    private float GetTotalHeight()
    {
        var titleRectTransform = title.GetComponent<RectTransform>();
        float titleHeight = titleRectTransform.sizeDelta.y;

        var rewardRectTransform = rewardPrefab.GetComponent<RectTransform>();
        float rewardHeight = rewardRectTransform.sizeDelta.y;
        float totalRewardHeight = rewardHeight * rewardCount;

        var buttonTransform = activeButton.GetComponent<RectTransform>();
        float buttonHeight = buttonTransform.sizeDelta.y;

        return titleHeight + totalRewardHeight + buttonHeight;
    }

    private void MoveBattleResult()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, GetTotalHeight() * 0.5f);
    }
}