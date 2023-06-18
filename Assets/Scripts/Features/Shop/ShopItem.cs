using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private CurrencyHandler currencyHandler;
    private InventoryManager inventoryManager;

    private CurrentShopItem currentShopItem;

    public RewardObject rewardObject;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCostText;

    public Shop shop;
    public bool remove;

    public TooltipObject tooltipIconItem;
    public TooltipObject tooltipIconCurrency;

    private void Awake()
    {
        currencyHandler = GameManager.Instance.currencyHandler;
        inventoryManager = InventoryManager.Instance;
    }

    public void SetData(CurrentShopItem _currentShopItem, Shop _shop, bool _remove)
    {
        currentShopItem = _currentShopItem;

        if (currentShopItem.reward.type == RewardType.Ability)
        {
            Reward tempReward = new Reward(currentShopItem.reward.ability.activeAbility);
            tempReward.ability.level = currentShopItem.reward.ability.level + 1;
            rewardObject.SetData(tempReward);
        }
        else
        {
            rewardObject.SetData(currentShopItem.reward);
        }

        string color = "";
        if (rewardObject.reward.type == RewardType.Item) 
            color = ColorDatabase.QualityColor(rewardObject.reward.itemDrop.itemObject.quality);
        else if (rewardObject.reward.type == RewardType.Ability) 
            color = ColorDatabase.QualityColor(rewardObject.reward.ability.activeAbility.quality);

        itemName.text = string.Format("<color={1}>{0}</color>", rewardObject.reward.name, color);
        itemCostText.text = currentShopItem.cost.totalAmount.ToString();

        tooltipIconItem.state = CurrentState.Values;
        tooltipIconCurrency.state = CurrentState.Town;

        rewardObject.GetComponent<Image>().sprite = rewardObject.reward.icon;

        shop = _shop;
        remove = _remove;

        SetTooltipIcon();

        tooltipIconCurrency.currency = currentShopItem.cost;
        tooltipIconCurrency.GetComponent<Image>().sprite = Currency.GetCurrencyIcon(currentShopItem.cost.currencyType, false);
    }

    private void UpdateItem(Hero buyer)
    {
        currentShopItem.UpdateItem(buyer);

        SetData(currentShopItem, shop, remove);
    }

    private void SetTooltipIcon()
    {
        switch (rewardObject.reward.type)
        {
            case RewardType.Ability:
                tooltipIconItem.active = rewardObject.reward.ability;
                break;
            case RewardType.Item:
                tooltipIconItem.itemObject = rewardObject.reward.itemDrop.itemObject;
                break;
            case RewardType.Attribute:
                tooltipIconItem.attribute = rewardObject.reward.attribute;
                break;
            case RewardType.Mix:
                break;
            default:
                break;
        }
    }

    public void BuyItem()
    {
        bool successfulBuy = false;

        if (currencyHandler.CanBuy(currentShopItem.cost))
        {
            switch (rewardObject.reward.type)
            {
                case RewardType.Ability:
                    {
                        Hero hero = TeamManager.Instance.heroes.Members[shop.currentHero] as Hero;

                        if (hero.spellbook.CanAddToCollection(rewardObject.reward.ability))
                        {
                            hero.spellbook.LearnAbility(rewardObject.reward.ability);

                            UpdateItem(hero);

                            successfulBuy = true;
                        }
                        else
                        {
                            ShortMessage.SendMessage(Input.mousePosition, "No available slots.", 24, Color.red);
                        }
                    }
                    break;
                case RewardType.Item:
                    {
                        successfulBuy = inventoryManager.AddItemToInventory(rewardObject.reward.itemDrop.itemObject);
                    }
                    break;
                case RewardType.Attribute:
                    break;
                case RewardType.Currency:
                    break;
                case RewardType.Mix:
                    break;
                default:
                    break;
            }

            if (successfulBuy)
            {
                currencyHandler.Buy(currentShopItem.cost);

                ShortMessage.SendMessage(Input.mousePosition, "Successful purchase!", 24, Color.green);

                if (remove)
                {
                    //shop.RemoveFromShop(this);
                    Destroy(gameObject);
                }
            }
            else
            {
                ShortMessage.SendMessage(Input.mousePosition, "Unsuccessful purchase!", 24, Color.red);
            }
        }
        else
        {
            ShortMessage.SendMessage(Input.mousePosition, "Not enough currency.", 24, Color.red);
        }
    }
}
