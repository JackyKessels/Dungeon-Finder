using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultWindow : MonoBehaviour
{
    public GameObject title;
    public GameObject container;
    public GameObject rewardPrefab;
    public GameObject experiencePrefab;
    public GameObject continueButtonPrefab;

    private Button activeButton;
    private int rewardCount = 0;

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
        rewardCount = 0;

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
                AddBattleReward(itemDrop);

                InventoryManager.Instance.AddItemToInventory(itemDrop.itemObject, itemDrop.amount);

                rewardCount++;
            }
        }

        Button continueButton = AddContinueButton();

        if (experience > 0)
            continueButton.onClick.AddListener(delegate { NextWindow(experience); });
        else
            continueButton.onClick.AddListener(delegate { CloseWindow(); });

        activeButton = continueButton;

        MoveBattleResult();
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

    private void AddBattleReward(ItemDrop itemDrop)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(rewardPrefab, container);

        BattleRewardObject battleReward = obj.GetComponent<BattleRewardObject>();
        battleReward.Setup(itemDrop);
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