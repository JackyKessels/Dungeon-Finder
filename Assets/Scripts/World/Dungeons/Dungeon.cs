using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "World/Dungeon")]
public class Dungeon : ScriptableObject
{
    [Header("[ Dungeon Information ]")]
    public new string name;
    public int recommendedMinimumLevel;
    public int recommendedMaximumLevel;
    public Color nameColor;

    [Header("[ Dungeon Floors ]")]
    public List<Floor> floors;
}

[System.Serializable]
public class Floor
{
    public string name;
    public Sprite dungeonBackground;
    public Sprite battleBackground;
    public AudioClip backgroundSound;

    [Header("[ Floor Dimensions ]")]
    public int columns;
    public int rows;
    public int startingPoints;
    public int endPoints;
    public int removalThreshold;
    public int minimumLocations = 1;
    public bool addOffset;
    public bool forceBossCenter = false;

    [Header("[ Location Specifications ]")]
    public int eliteCount = 0;
    public int campfireCount = 0;
    public int treasureCount = 0;
    public int mysteryCount = 0;
    public List<int> spiritColumns = new List<int>();

    [Header("[ Treasure Contents ]")]
    public List<ItemDrop> itemPool = new List<ItemDrop>();

    [Header("[ Encounters ]")]
    public List<Encounter> trashEncounters;
    public List<Encounter> eliteEncounters;
    public BossEncounter bossEncounter;

    [Header("[ Mystery Events ]")]
    public MysteryEvents mysteryEvents;
}
