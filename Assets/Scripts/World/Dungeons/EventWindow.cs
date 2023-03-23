using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public TextMeshProUGUI problemText;
    public GameObject consequencesContainer;
    public GameObject consequencePrefab;
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
        problemText.text = mysteryEvent.flavorText;

        ObjectUtilities.ClearContainer(consequencesContainer);

        consequenceStructures = new List<ConsequenceStructure>();

        mysteryEvent.TriggerEvent(consequenceStructures);

        foreach (ConsequenceStructure consequenceStructure in consequenceStructures)
        {
            CreateConsequence(consequenceStructure);
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
