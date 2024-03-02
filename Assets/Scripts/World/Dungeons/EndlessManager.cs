using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class EndlessManager : MonoBehaviour
{
    public Dungeon endlessDungeon;
    public TextMeshProUGUI levelCounter;

    public int endlessLevel = 1;

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

    public void CompleteFloor()
    {
        endlessLevel++;
        UpdateCounter();
        // max lvl is currently 50, probably uncap for endless
        TeamManager.Instance.experienceManager.LevelUpTeam(true);
    }

    private void UpdateCounter()
    {
        levelCounter.text = endlessLevel.ToString();
    }
}
