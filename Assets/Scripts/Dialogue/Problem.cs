using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Problem", menuName = "Systems/Dialogue/Problem")]
public class Problem : ScriptableObject
{
    public new string name;
    public int actIndex;
    public int problemIndex;
    public Choice[] choices;
}

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string text;
    public int choiceIndex;
    public Conversation conversation;
}
