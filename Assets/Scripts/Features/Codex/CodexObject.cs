using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Codex Database", menuName = "Systems/Codex Database Object")]
public class CodexObject : ScriptableObject
{
    public List<CodexDungeonEntry> dungeonEntries;

    [ContextMenu("Sort items alphabetically")]
    public void SortAlphabetically()
    {
        foreach (CodexDungeonEntry entry in dungeonEntries)
        {
            entry.equipments = SortItems(entry.equipments);
            entry.consumables = SortItems(entry.consumables);
        }
    }

    private List<ItemObject> SortItems(List<ItemObject> itemObjects)
    {
        List<ItemObject> noDupes = new List<ItemObject>(new HashSet<ItemObject>(itemObjects));
        noDupes.RemoveAll(item => item == null);
        itemObjects.Clear();
        itemObjects.AddRange(noDupes);

        return itemObjects.OrderBy(i => i.name).ToList();
    }
}

[System.Serializable]
public class CodexDungeonEntry
{
    public Dungeon dungeon;

    public List<ItemObject> equipments;
    public List<ItemObject> consumables;
}
