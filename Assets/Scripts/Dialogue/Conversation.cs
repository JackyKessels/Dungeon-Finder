using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Systems/Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public UnitObject leftSpeaker;
    public UnitObject rightSpeaker;
    public Line[] lines;
    public Problem problem;
    public Conversation nextConversation;
}

[System.Serializable]
public struct Line
{
    public UnitObject speaker;

    [TextArea(2, 5)]
    public string text;
}
