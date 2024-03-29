﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public GameObject consequencesContainer;
    public GameObject consequencePrefab;

    public GameObject textAndConsequence;
    public TextMeshProUGUI textAndConsequenceText;
    public GameObject textOnly;
    public TextMeshProUGUI textOnlyText;

    public Button continueButton;

    private List<ConsequenceStructure> consequenceStructures = new List<ConsequenceStructure>();

    public void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            if (continueButton)
                continueButton.onClick?.Invoke();
        }
    }

    public void Setup(MysteryEvent mysteryEvent)
    {
        ObjectUtilities.ClearContainer(consequencesContainer);

        consequenceStructures = new List<ConsequenceStructure>();

        mysteryEvent.TriggerEvent(consequenceStructures);

        if (consequenceStructures.Count > 0)
        {
            textOnly.SetActive(false);

            textAndConsequenceText.SetText(mysteryEvent.flavorText);

            foreach (ConsequenceStructure consequenceStructure in consequenceStructures)
            {
                CreateConsequence(consequenceStructure);
            }
        }
        else
        {
            textAndConsequence.SetActive(false);

            textOnlyText.SetText(mysteryEvent.flavorText);
        }
    }

    public static void SendEventWindow(MysteryEvent mysteryEvent)
    {
        GameObject container = GameObject.Find("Event Container");

        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.eventPrefab.gameObject, container);

        EventWindow eventWindow = obj.GetComponent<EventWindow>();
        eventWindow.Setup(mysteryEvent);
    }

    private void CreateConsequence(ConsequenceStructure consequenceStructure)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(consequencePrefab, consequencesContainer);

        ConsequenceObject consequence = obj.GetComponent<ConsequenceObject>();
        consequence.Setup(consequenceStructure);
    }

    public void ContinueButton()
    {
        Destroy(gameObject);
    }
}
