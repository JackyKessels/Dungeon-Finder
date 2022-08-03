using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActProblemsHandler : MonoBehaviour
{
    #region Singleton
    public static ActProblemsHandler Instance { get; private set; }

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

    public List<ActProblems> actProblems = new List<ActProblems>();

    public ActProblems GetActProblems(int index)
    {
        return actProblems[index];
    }
}
