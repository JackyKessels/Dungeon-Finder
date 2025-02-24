using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private ActManager actManager;
    private TeamManager teamManager;

    [Header("[ Dialogue ]")]
    public GameObject dialogueContainer;
    public SpeakerObject leftSpeaker;
    public SpeakerObject rightSpeaker;

    [Header("[ Problem ]")]
    public ProblemController problemController;

    [HideInInspector] public Conversation currentConversation;

    private int activeLineIndex = 0;
    private bool activeConversation = false;

    private void Start()
    {
        actManager = ActManager.Instance;
        teamManager = TeamManager.Instance;
    }

    private void Update()
    {
        if (KeyboardHandler.ProgressWindow() && activeConversation)
        {
            AdvanceLine();
        }
    }

    public void StartConversation(Conversation conversation)
    {
        activeLineIndex = 0;
        activeConversation = true;

        currentConversation = conversation;

        DialogueVisibility(true);

        AdvanceLine();
    }

    private UnitObject GetSpeakerUnitObject(Speaker activeSpeaker)
    {
        switch (activeSpeaker)
        {
            case Speaker.Hero1:
                return teamManager.heroes.Members[0].GetUnitObject();
            case Speaker.Hero2:
                return teamManager.heroes.Members[1].GetUnitObject();
            case Speaker.Hero3:
                return teamManager.heroes.Members[2].GetUnitObject();
            case Speaker.Enemy1:
                return teamManager.enemies.Members[0].GetUnitObject();
            case Speaker.Enemy2:
                return teamManager.enemies.Members[1].GetUnitObject();
            case Speaker.Enemy3:
                return teamManager.enemies.Members[2].GetUnitObject();
            default:
                return null;
        }
    }

    // Shows and hides the dialogue
    private void DialogueVisibility(bool setActive)
    {
        dialogueContainer.SetActive(setActive);
    }

    // Shows the next line if there is one, otherwise turn off the objects and reset the index
    private void AdvanceLine()
    {
        if (activeLineIndex < currentConversation.lines.Length)
        {
            DisplayLine();
        }
        else
        {
            AdvanceConversation();
        }
    }

    private void DisplayLine()
    {
        Line line = currentConversation.lines[activeLineIndex];
        UnitObject leftUnit = GetSpeakerUnitObject(line.leftSpeaker);
        UnitObject rightUnit = GetSpeakerUnitObject(line.rightSpeaker);

        if (line.leftSpeaker != Speaker.None)
        {
            leftSpeaker.Show(true);
            leftSpeaker.Setup(leftUnit);

            if (line.activeSpeaker == ActiveSpeaker.Left)
            {
                leftSpeaker.SetActiveSpeaker();
                rightSpeaker.SetInactiveSpeaker();

                leftSpeaker.SetText(line.text);
            }
        }
        else
        {
            leftSpeaker.Show(false);
        }

        if (line.rightSpeaker != Speaker.None)
        {
            rightSpeaker.Show(true);
            rightSpeaker.Setup(rightUnit);

            if (line.activeSpeaker == ActiveSpeaker.Right)
            {
                rightSpeaker.SetActiveSpeaker();
                leftSpeaker.SetInactiveSpeaker();

                rightSpeaker.SetText(line.text);
            }
        }
        else
        {
            rightSpeaker.Show(false);
        }

        activeLineIndex++;
    }

    private void AdvanceConversation()
    {
        if (currentConversation.nextConversation != null)
        {
            StartConversation(currentConversation.nextConversation);
        }
        else
        {
            EndConversation();
        }
    }

    private void EndConversation()
    {
        currentConversation = null;
        DialogueVisibility(false);
        activeConversation = false;
    }
}
