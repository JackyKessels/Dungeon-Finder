using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class EndlessManager : MonoBehaviour
{
    public Dungeon endlessDungeon;
    public TextMeshProUGUI levelCounter;

    public int EndlessLevel 
    {
        get
        {
            return ProgressionManager.Instance.endlessLevel;
        }
        set
        {
            ProgressionManager.Instance.endlessLevel = value;
            UpdateCounter();
        }
    }

    private Floor lastFloor;

    private void Start()
    {   
        lastFloor = null;
    }

    public void StartNextFloor()
    {
        TownManager.Instance.StartDungeon(endlessDungeon);
    }

    public Floor NextFloor()
    {
        List<Floor> floors = endlessDungeon.floors.Where(f => f != lastFloor).ToList();

        int randomFloor = Random.Range(0, floors.Count);

        Floor nextFloor = floors[randomFloor];

        lastFloor = nextFloor;

        return nextFloor;
    }

    private void UpdateCounter()
    {
        levelCounter.text = EndlessLevel.ToString();
    }
}
