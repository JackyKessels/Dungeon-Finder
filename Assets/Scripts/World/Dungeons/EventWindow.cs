using System.Collections;
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

    public void Setup(MysteryEvent mysteryEvent, int level)
    {
        ObjectUtilities.ClearContainer(consequencesContainer);

        consequenceStructures = new List<ConsequenceStructure>();

        MysteryResult mysteryResult = mysteryEvent.TriggerEvent(consequenceStructures, level);
        if (mysteryResult != null)
        {
            if (consequenceStructures.Count > 0)
            {
                textOnly.SetActive(false);

                textAndConsequenceText.SetText(mysteryResult.flavorText);

                foreach (ConsequenceStructure consequenceStructure in consequenceStructures)
                {
                    CreateConsequence(consequenceStructure, level);
                }
            }
            else
            {
                textAndConsequence.SetActive(false);

                textOnlyText.SetText(mysteryResult.flavorText);
            }
        }
        else
        {
            textAndConsequence.SetActive(false);

            textOnlyText.SetText("Nothing of interest occurred.");
        }
    }

    public static void CreateEventWindow(MysteryEvent mysteryEvent, int level)
    {
        GameObject container = GameObject.Find("Event Container");

        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.eventPrefab.gameObject, container);

        EventWindow eventWindow = obj.GetComponent<EventWindow>();
        eventWindow.Setup(mysteryEvent, level);
    }

    private void CreateConsequence(ConsequenceStructure consequenceStructure, int level)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(consequencePrefab, consequencesContainer);

        ConsequenceObject consequence = obj.GetComponent<ConsequenceObject>();
        consequence.Setup(consequenceStructure, level);
    }

    public void ContinueButton()
    {
        Destroy(gameObject);
    }
}
