using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EventType
{
    Battle_Easy,
    Battle_Normal,
    Battle_Hard,
    Boss,
    Puzzle,
    Rest
}

public interface IEvent
{
    EventType GetEventType(); 
}

[System.Serializable]
public class Battle : IEvent
{
    public EventType GetEventType()
    {
        return EventType.Battle_Easy;
    }
}

public class Rest : IEvent
{
    public EventType GetEventType()
    {
        return EventType.Rest;
    }
}
