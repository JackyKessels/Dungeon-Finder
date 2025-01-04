using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Key", menuName = "Item/Consumable/Dungeon Key")]
public class DungeonKey : Consumable
{
    public Dungeon dungeon;

    public override bool Consume(int i)
    {
        if (GameManager.Instance.gameState == GameState.TOWN)
        {
            TownManager.Instance.CloseAllWindows();
            TownManager.Instance.StartDungeon(dungeon);
            Debug.Log($"Starting {dungeon.name}...");
            return true;
        }
        else
        {
            Debug.Log($"Can only start a new dungeon when in town.");
            return false;
        }
    }

    public override string GetTooltip(TooltipObject tooltipInfo)
    {
        return $"\nLoot Level: {dungeon.recommendedMinimumLevel} - {dungeon.recommendedMaximumLevel}" +
               $"\nUse: Enter {dungeon.name}." +
               $"\n\n<color={ColorDatabase.EffectColor()}>Can only be used in town.</color>";
    }
}
