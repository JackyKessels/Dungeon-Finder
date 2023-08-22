using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test_LootButton : MonoBehaviour
{
    public List<ItemObject> items = new List<ItemObject>();

    public void AddItemsFunction()
    {
        foreach (ItemObject obj in items)
        {
            Item item = Item.CreateItem(obj, 1);

            InventoryManager.Instance.AddItemToInventory(item);
        }
    }

    public void AddRandomItemsHalf()
    {
        List<ItemObject> randomItems = new List<ItemObject>();

        for (int i = 0; i < items.Count / 2; i++)
        {
            int index = Random.Range(0, items.Count);
            randomItems.Add(items[index]);
        }

        foreach (ItemObject obj in randomItems)
        {
            Item item = Item.CreateItem(obj, 1);

            InventoryManager.Instance.AddItemToInventory(item);
        }
    }
}
