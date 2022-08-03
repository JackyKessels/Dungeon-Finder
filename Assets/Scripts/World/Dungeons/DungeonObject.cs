using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonObject : MonoBehaviour, IDescribable
{
    public Dungeon dungeon;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(delegate { TownManager.Instance.StartRun(dungeon); } );
    }

    public string GetDescription(TooltipObject tooltipInfo)
    {
        return dungeon.name;
    }

}
