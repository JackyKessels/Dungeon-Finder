using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignManager : MonoBehaviour
{
    [SerializeField] GameObject dungeonPrefab;

    public List<DungeonListEntry> dungeons;

    public void AddDungeonsToMap(GameObject container)
    {
        ObjectUtilities.ClearContainer(container);

        for (int i = 0; i < dungeons.Count; i++)
        {
            if (!dungeons[i].testing || dungeons[i].testing && GameManager.Instance.TEST_MODE)
            {
                GameObject obj = ObjectUtilities.CreateSimplePrefab(dungeonPrefab, container);

                DungeonButton dungeonButton = obj.GetComponent<DungeonButton>();
                dungeonButton.Setup(dungeons[i]);

                dungeons[i].button = dungeonButton;
            }
        }
    }

    public void SetDungeonList(bool active)
    {
        foreach (DungeonListEntry entry in dungeons)
        {
            entry.locked = !active;
        }
    }

    public void UnlockDungeon(Dungeon completedDungeon)
    {
        if (completedDungeon == null)
            return;

        List<string> unlockedDungeons = new List<string>();

        foreach (DungeonListEntry dungeon in dungeons)
        {
            if (dungeon.unlockedBy == completedDungeon)
            {
                dungeon.locked = false;

                unlockedDungeons.Add(dungeon.dungeon.name);
            }
        }

        if (unlockedDungeons.Count > 0)
        {
            NotificationObject.SendNotification("You have unlocked " + AbilityTooltipHandler.JoinString(unlockedDungeons, ", ", " and ", ColorDatabase.MagicalColor()));
        }
    }
}

[System.Serializable]
public class DungeonListEntry
{
    public Dungeon dungeon;
    public bool locked = false;
    public Dungeon unlockedBy = null;
    public bool testing = false;
    [HideInInspector] public DungeonButton button;



}
