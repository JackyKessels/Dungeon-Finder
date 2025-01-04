using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonObject : MonoBehaviour, IHasTooltip
{
    public Dungeon dungeon;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(delegate { TownManager.Instance.StartDungeon(dungeon); } );
    }

    public string GetCompleteTooltip(TooltipObject tooltipInfo)
    {
        return dungeon.name;
    }

}
