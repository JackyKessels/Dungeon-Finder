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

    [Header("[ Dialogue ]")]
    public GameObject dialogueContainer;
    public Speaker leftSpeaker;
    public Speaker rightSpeaker;

    [Header("[ Problem ]")]
    public ProblemController problemController;

    [HideInInspector] public Conversation currentConversation;

    private int activeLineIndex = 0;
    private bool activeConversation = false;

    private void Start()
    {
        actManager = ActManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && activeConversation)
        {
            AdvanceLine();
        }
    }

    public void Setup(Conversation conversation)
    {
        activeLineIndex = 0;
        activeConversation = true;

        currentConversation = conversation;

        DialogueStatus(true);

        if (currentConversation.leftSpeaker != null)
        {
            leftSpeaker.Show(true);
            leftSpeaker.Setup(currentConversation.leftSpeaker);
        }
        else
        {
            leftSpeaker.Show(false);
        }

        if (currentConversation.rightSpeaker != null)
        {
            rightSpeaker.Show(true);
            rightSpeaker.Setup(currentConversation.rightSpeaker);
        }
        else
        {
            rightSpeaker.Show(false);
        }

        AdvanceLine();
    }

    // Shows and hides the dialogue
    private void DialogueStatus(bool setActive)
    {
        dialogueContainer.SetActive(setActive);
    }

    // Shows the next line if there is one, otherwise turn off the objects and reset the index
    public void AdvanceLine()
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

    // Handles the correct positioning of the speakers
    private void DisplayLine()
    {
        Line line = currentConversation.lines[activeLineIndex];
        UnitObject speaker = line.speaker;

        if (leftSpeaker.SpeakerIs(speaker))
        {
            SetDialogue(leftSpeaker, rightSpeaker, line.text);
        }
        else
        {
            SetDialogue(rightSpeaker, leftSpeaker, line.text);
        }

        activeLineIndex++;
    }

    // Go to the problem if there is one, go to the next conversation if there is one, 
    // or end the current conversation
    private void AdvanceConversation()
    {
        if (currentConversation.problem != null)
        {
            problemController.Setup(currentConversation.problem);
            EndConversation();
        }
        else if (currentConversation.nextConversation != null)
        {
            Setup(currentConversation.nextConversation);
        }
        else
        {
            EndConversation();
        }
    }

    // End the current conversation
    private void EndConversation()
    {
        currentConversation = null;
        DialogueStatus(false);
        activeConversation = false;
    }

    // Sets the content of the speakers and deactivates the one that is not speaking
    private void SetDialogue(Speaker activeSpeaker, Speaker inactiveSpeaker, string text)
    {
        activeSpeaker.SetText(text);

        activeSpeaker.SetActiveSpeaker();

        inactiveSpeaker.SetInactiveSpeaker();
    }











    //public void ChangeConversation(Conversation nextConversation)
    //{
    //    conversationStarted = false;
    //    currentConversation = nextConversation;
    //    AdvanceLine();
    //}

    //private void EndConversation()
    //{
    //    currentConversation = null;
    //    conversationStarted = false;
    //    DialogueStatus(false);
    //}

    //private void Initialize()
    //{
    //    conversationStarted = true;
    //    activeLineIndex = 0;

    //    if (currentConversation.leftSpeaker != null)
    //    {
    //        leftSpeaker.Setup(currentConversation.leftSpeaker);
    //        leftSpeaker.Show(true);
    //    }
    //    else
    //    {
    //        leftSpeaker.Show(false);
    //    }

    //    if (currentConversation.rightSpeaker != null)
    //    {
    //        rightSpeaker.Setup(currentConversation.rightSpeaker);
    //        rightSpeaker.Show(true);
    //    }
    //    else
    //    {
    //        rightSpeaker.Show(false);
    //    }
    //}

    //private void AdvanceLine()
    //{
    //    if (currentConversation = null) return;
    //    if (!conversationStarted) Initialize();

    //    if (activeLineIndex < currentConversation.lines.Length)
    //    {
    //        DisplayLine();
    //    }
    //    else
    //    {
    //        AdvanceConversation();
    //    }
    //}

    //private void DisplayLine()
    //{
    //    Line line = currentConversation.lines[activeLineIndex];
    //    UnitObject speaker = line.speaker;

    //    if (leftSpeaker.SpeakerIs(speaker))
    //    {
    //        SetDialogue(leftSpeaker, rightSpeaker, line.text);
    //    }
    //    else
    //    {
    //        SetDialogue(rightSpeaker, leftSpeaker, line.text);
    //    }

    //    activeLineIndex++;
    //}

    //private void AdvanceConversation()
    //{
    //    if (currentConversation.problem != null)
    //    {
    //        problemEvent.Invoke
    //    }
    //    else if (currentConversation.nextConversation != null)
    //    {
    //        ChangeConversation(currentConversation.nextConversation);
    //    }
    //    else
    //    {
    //        EndConversation();
    //    }
    //}



}
