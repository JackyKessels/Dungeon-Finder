using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public ItemDrop(ItemObject _itemObject, int _level)
    {
        itemObject = _itemObject;
        amount = 1;
        weight = 1;

        level = _level;
    }

    public ItemObject itemObject;
    public int amount = 1;
    public int weight = 1;

    [HideInInspector] public int level = 1;

    public Item GetItem()
    {
        return Item.CreateItem(itemObject, level);
    }

    public static ItemDrop WeightedDrops(List<ItemDrop> itemDrops)
    {
        if (itemDrops.Count == 0)
            return null;

        List<int> weights = new List<int>();

        foreach (ItemDrop itemDrop in itemDrops)
        {
            weights.Add(itemDrop.weight);
        }

        return itemDrops[GeneralUtilities.RandomWeighted(weights)];
    }
}
