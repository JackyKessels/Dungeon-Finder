using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI problemText;
    [SerializeField] GameObject consequencesContainer;
    [SerializeField] GameObject consequencePrefab;

    private List<ConsequenceStructure> consequenceStructures = new List<ConsequenceStructure>();

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

    private void CreateConsequence(ConsequenceStructure consequenceStructure)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(consequencePrefab, consequencesContainer);

        ConsequenceObject consequence = obj.GetComponent<ConsequenceObject>();
        consequence.Setup(consequenceStructure);
    }

    public void ContinueButton()
    {
        gameObject.SetActive(false);
    }
}
