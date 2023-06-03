using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private SaveManager saveManager;

    public string id;

    public GameObject emptySlot;
    public GameObject usedSlot;

    public Image hero_1;
    public Image hero_2;
    public Image hero_3;

    public TextMeshProUGUI slot;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI spirit;
    public TextMeshProUGUI experience;
    public TextMeshProUGUI date;

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

            emptySlot.SetActive(false);
            usedSlot.SetActive(true);

            ParseTeamData();
        }
        else
        {
            emptySlot.SetActive(true);
            usedSlot.SetActive(false);
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

    private void ParseTeamData()
    {
        slot.SetText("Slot " + id);

        hero_1.sprite = TeamManager.Instance.heroObjects[teamData.heroIndex_0].icon;
        hero_2.sprite = TeamManager.Instance.heroObjects[teamData.heroIndex_1].icon;
        hero_3.sprite = TeamManager.Instance.heroObjects[teamData.heroIndex_2].icon;

        gold.SetText(teamData.currency_0.ToString());
        spirit.SetText(teamData.currency_1.ToString());
        experience.SetText(teamData.teamExperience.ToString());

        date.SetText(SaveSystem.GetLastSaved(id));
    }
}
