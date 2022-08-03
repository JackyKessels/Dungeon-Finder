using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActProblems : MonoBehaviour
{
    private void Start()
    {
        CreateList();
    }

    public delegate void Problems(int index);
    public List<Problems> problems = new List<Problems>();

    public abstract void CreateList();
}
