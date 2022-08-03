using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProblemController : MonoBehaviour
{
    public Problem currentProblem;
    public GameObject problemContainer;
    public TextMeshProUGUI problemTitle;
    public Button choiceButtonPrefab;
    public GameObject buttonContainer;

    public void Setup(Problem problem)
    {
        Show(true);
        ObjectUtilities.ClearContainer(buttonContainer);
        currentProblem = problem;
        Initialize();
    }

    public void ResetProblem()
    {
        ObjectUtilities.ClearContainer(buttonContainer);
        Show(false);
    }

    private void Show(bool show)
    {
        problemContainer.SetActive(show);
    }

    private void Initialize()
    {
        problemTitle.text = currentProblem.name;

        for (int i = 0; i < currentProblem.choices.Length; i++)
        {
            SetupButton(currentProblem, i);
        }

        // Refreshes the choice box to have the correct dimensions
        Canvas.ForceUpdateCanvases();
        problemContainer.transform.GetComponent<VerticalLayoutGroup>().enabled = false;
        problemContainer.transform.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    private void SetupButton(Problem problem, int choiceIndex)
    {
        Button button = Instantiate(choiceButtonPrefab);

        button.transform.SetParent(buttonContainer.transform);
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = Vector3.zero;
        button.name = "Choice " + (choiceIndex + 1);
        button.gameObject.SetActive(true);

        button.GetComponentInChildren<TextMeshProUGUI>().text = problem.choices[choiceIndex].text;

        button.onClick.AddListener(delegate { MakeChoice(problem, choiceIndex); });
    }

    public void MakeChoice(Problem problem, int choiceIndex)
    {
        if (problem.choices[choiceIndex].conversation != null)
        {
            DialogueManager.Instance.Setup(problem.choices[choiceIndex].conversation);
        }
        else if (problem.actIndex >= 0)
        {
            ActProblemsHandler.Instance.GetActProblems(problem.actIndex).problems[problem.problemIndex](choiceIndex);

        }

        ResetProblem();
    }
}
