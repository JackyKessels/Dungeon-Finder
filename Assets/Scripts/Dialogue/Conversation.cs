using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Speaker
{
    None,
    Hero1,
    Hero2,
    Hero3,
    Enemy1,
    Enemy2,
    Enemy3
}

public enum ActiveSpeaker
{
    None,
    Left,
    Right
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Systems/Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public Line[] lines;
    public Conversation nextConversation;
}

[System.Serializable]
public struct Line
{
    public Speaker leftSpeaker;
    public Speaker rightSpeaker;
    public ActiveSpeaker activeSpeaker;

    [TextArea(2, 5)]
    public string text;
}
