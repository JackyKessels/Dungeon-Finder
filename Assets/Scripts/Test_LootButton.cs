using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LootButton : MonoBehaviour
{
    public List<ItemObject> items = new List<ItemObject>();

    public void AddItemsFunction()
    {
        foreach (ItemObject obj in items)
        {
            InventoryManager.Instance.AddItemToInventory(obj);
        }
    }
}
