using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Systems/Inventory Database Object")]
public class ItemDatabaseObject : ScriptableObject
{
    public List<ItemObject> itemObjects;

    [ContextMenu("Update IDs")]
    public void UpdateId()
    {
        List<ItemObject> noDupes = new List<ItemObject>(new HashSet<ItemObject>(itemObjects));
        itemObjects.Clear();
        itemObjects.AddRange(noDupes);

        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i].item.id != i)
                itemObjects[i].item.id = i;
        }
    }

}