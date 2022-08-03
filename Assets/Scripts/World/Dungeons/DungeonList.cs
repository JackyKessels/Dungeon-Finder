using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonList : MonoBehaviour
{
    [SerializeField] GameObject dungeonPrefab;

    [SerializeField] List<Dungeon> dungeons; 

    public void AddDungeonsToMap(GameObject container)
    {
        ObjectUtilities.ClearContainer(container);

        for (int i = 0; i < dungeons.Count; i++)
        {
            GameObject obj = ObjectUtilities.CreateSimplePrefab(dungeonPrefab, container);

            DungeonButton dungeonButton = obj.GetComponent<DungeonButton>();
            dungeonButton.Setup(dungeons[i]);
        }
    }



}
