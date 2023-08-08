using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Shop : MonoBehaviour
{
    protected TownManager townManager;
    protected TeamManager teamManager;

    [Header("[ Generic Shop ]")]
    public string shopName;

    public Button shopButton;
    public GameObject shopObject;
    public GameObject shopContainer;
    public TextMeshProUGUI shopTitle;

    public bool unlocked = false;

    public int currentHero = 0;

    public abstract void SetupShop();

    private void Start()
    {
        townManager = TownManager.Instance;
        teamManager = TeamManager.Instance;

        SetName();
    }

    public void SetActive(bool active)
    {
        shopObject.SetActive(active);
    }

    public bool ActiveSelf()
    {
        return shopObject.activeSelf;
    }

    public void SetButton(bool active)
    {
        if (shopButton != null)
            shopButton.interactable = active;
    }

    public bool ActiveButton()
    {
        if (shopButton == null)
            return false;

        if (shopButton.interactable && shopButton.gameObject.activeSelf)
            return true;

        return false;
    }

    public void UnlockShop()
    {
        unlocked = true;
        SetName();
    }

    private void SetName()
    {
        if (unlocked)
        {
            shopTitle.color = Color.white;
            shopTitle.text = shopName;
        }
        else
        {
            shopTitle.color = Color.red;
            shopTitle.text = "LOCKED";
        }
    }

    public static int GetAbilityCost(AbilityObject ability, Hero hero)
    {
        int level = hero == null ? 0 : hero.spellbook.LevelOfAbility(ability);

        return AbilityCost(ability, level);
    }

    public static int GetAbilityCost(Active active)
    {
        return AbilityCost(active.activeAbility, active.level);
    }

    private static int AbilityCost(AbilityObject ability, int level)
    {
        int qualityCost;

        switch (ability.quality)
        {
            case Quality.Common:
                qualityCost = 1;
                break;
            case Quality.Mystical:
                qualityCost = 2;
                break;
            case Quality.Legendary:
                qualityCost = 3;
                break;
            default:
                qualityCost = 1;
                break;
        }

        return qualityCost * (10 + (level - 1) * 5);
    }

    public void RemoveItemFromList(List<CurrentShopItem> currentShopItems, ShopItem shopItem)
    {
        if (shopItem.rewardObject.reward.type == RewardType.Item)
        {
            for (int i = currentShopItems.Count; i-- > 0;)
            {
                if (currentShopItems[i].reward == shopItem.rewardObject.reward)
                {
                    currentShopItems.Remove(currentShopItems[i]);
                    return;
                }
            }
        }
        else if (shopItem.rewardObject.reward.type == RewardType.Ability)
        {
            for (int i = currentShopItems.Count; i-- > 0;)
            {
                if (currentShopItems[i].reward.ability.activeAbility == shopItem.rewardObject.reward.ability.activeAbility)
                {
                    currentShopItems.Remove(currentShopItems[i]);
                    return;
                }
            }
        }
    }

    public static void AddItemToShop(CurrentShopItem currentShopItem, Shop shop, bool remove)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.shopItemPrefab, shop.shopContainer);

        ShopItem shopItem = obj.GetComponent<ShopItem>();
        shopItem.SetData(currentShopItem, shop, remove);
    }
}

[System.Serializable]
public class CurrentShopItem
{
    public Reward reward;
    public Currency cost;

    public CurrentShopItem(Reward _reward, Currency _cost)
    {
        reward = _reward;
        cost = _cost;
    }

    public int GetRewardID()
    {
        if (reward.type == RewardType.Ability)
            return reward.ability.activeAbility.id;
        else if (reward.type == RewardType.Item)
            return reward.itemDrop.itemObject.item.id;
        else return -1;
    }

    public void UpdateItem(Hero buyer)
    {
        if (reward.ability.activeAbility != null)
        {
            reward.ability.level++;
            cost.totalAmount = Shop.GetAbilityCost(reward.ability.activeAbility, buyer);
        }
    }
}