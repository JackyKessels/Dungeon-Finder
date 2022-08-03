using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public ItemDrop(ItemObject _itemObject)
    {
        itemObject = _itemObject;
        amount = 1;
        weight = 1;
    }

    public ItemObject itemObject;
    public int amount = 1;
    public int weight = 1;

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
