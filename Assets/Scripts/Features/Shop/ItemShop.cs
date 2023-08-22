using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemShop : Shop
{
    [Header("[ Item Shop ]")]
    public LevelBracketShopCollection shopCollection;
    public int itemsDisplayed = 5;
    public bool limitedStock = false;
    public bool variablePrice = false;
    public float variableMinimum = 1f;
    public float variableMaximum = 1f;

    public List<CurrentShopItem> currentShopItems;

    [Header("[ Rotation ]")]
    public TextMeshProUGUI rotationCounterText;
    public int rotationCounter = 0;
    public int rotationCounterTotal = 5;

    public override void SetupShop()
    {
        ObjectUtilities.ClearContainer(shopContainer);

        for (int i = 0; i < currentShopItems.Count; i++)
        {
            AddItemToShop(currentShopItems[i], this, limitedStock);
        }
    }

    public void BuildShop(int level)
    {
        if (shopCollection == null)
            return;

        currentShopItems = new List<CurrentShopItem>();

        ObjectUtilities.ClearContainer(shopContainer);

        List<ItemObject> tempList = new List<ItemObject>(shopCollection.GetCorrectBracketItemObjects(level));
        List<ItemObject> newList = new List<ItemObject>();

        if (itemsDisplayed > tempList.Count)
        {
            newList = new List<ItemObject>(tempList);
        }
        else
        {
            for (int i = 0; i < itemsDisplayed; i++)
            {
                int randomItem = Random.Range(0, tempList.Count);

                newList.Add(tempList[randomItem]); ;
                tempList.RemoveAt(randomItem);
            }
        }

        foreach (ItemObject item in newList)
        {
            int price;

            if (variablePrice)
            {
                int minCost = (int)(item.value * variableMinimum);
                int maxCost = (int)(item.value * variableMaximum);

                price = Random.Range(minCost, maxCost);
            }
            else
            {
                price = item.value;
            }

            currentShopItems.Add(new CurrentShopItem(new Reward(new ItemDrop(item, 1)), new Currency(CurrencyType.Gold, price)));
        }
    }

    public void RemoveFromShop(ShopItem shopItem)
    {
        RemoveItemFromList(currentShopItems, shopItem);
    }

    public void RotateShopDisplay(int level)
    {
        rotationCounter++;

        if (rotationCounter == rotationCounterTotal)
        {
            BuildShop(level);
            rotationCounter = 0;
        }

        UpdateRotationDisplay();
    }

    public void UpdateRotationDisplay()
    {
        rotationCounterText.text = rotationCounter + "/" + rotationCounterTotal;
    }

    // Save & Load
    public void LoadShop(List<int> itemIDs, List<int> itemCosts, int rotation)
    {
        DatabaseHandler databaseHandler = DatabaseHandler.Instance;

        currentShopItems = new List<CurrentShopItem>();

        for (int i = 0; i < itemIDs.Count; i++)
        {
            currentShopItems.Add(new CurrentShopItem(new Reward(new ItemDrop(databaseHandler.itemDatabase.itemObjects[itemIDs[i]], 1)), new Currency(CurrencyType.Gold, itemCosts[i])));
        }

        rotationCounter = rotation;
    }
}
