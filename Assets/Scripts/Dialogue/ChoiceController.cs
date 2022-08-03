using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ConversationChangeEvent : UnityEvent<Conversation> { }

public class ChoiceController : MonoBehaviour
{
    public Choice choice;

    public static ChoiceController AddChoiceButton(Button choiceButtonPrefab, GameObject parent, Problem problem, int index)
    {
        Button button = Instantiate(choiceButtonPrefab);

        button.transform.SetParent(parent.transform);
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = Vector3.zero;
        button.name = "Choice " + (index + 1);
        button.gameObject.SetActive(true);

        button.GetComponentInChildren<TextMeshProUGUI>().text = problem.choices[index].text;

        ChoiceController choiceController = button.GetComponent<ChoiceController>();
        choiceController.choice = problem.choices[index];
        return choiceController;
    }

    public void MakeChoice(Problem problem, int choiceIndex)
    {
        ActProblemsHandler.Instance.GetActProblems(problem.actIndex).problems[problem.problemIndex](choiceIndex);
    }
}
