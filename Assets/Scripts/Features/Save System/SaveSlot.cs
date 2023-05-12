using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    private SaveManager saveManager;

    public string id;
    public TextMeshProUGUI slotContent;

    private TeamData teamData;

    private void Start()
    {
        saveManager = SaveManager.Instance;
    }

    public void Setup()
    {
        if (SaveSystem.SaveExists(id))
        {
            teamData = SaveSystem.LoadTeamData(id);

            slotContent.SetText(ParseTeamData());
        }
        else
        {
            slotContent.SetText("Empty Save Slot");
        }
    }

    public void SendData(TeamData teamData)
    {
        this.teamData = teamData;
    }

    private void StartEmptySlot()
    {
        GameManager.Instance.StartNewGame();
        saveManager.currentId = id;
    }

    public void LoadSlot()
    {
        if (SaveSystem.SaveExists(id))
        {
            saveManager.LoadTeam(id, teamData);
        }
        else
        {
            StartEmptySlot();
        }
    }

    public void DeleteSlot()
    {
        SaveSystem.DeleteTeamData(id);
        Setup();
    }

    private string ParseTeamData()
    {
        string s = "Slot " + id;

        s += "\n";

        s += "\nGold: " + teamData.currency_0;
        s += "\nSpirit: " + teamData.currency_1;
        s += "\nExperience: " + teamData.teamExperience;

        s += "\n\nLast Saved: " + SaveSystem.GetLastSaved(id);

        return s;
    }
}
