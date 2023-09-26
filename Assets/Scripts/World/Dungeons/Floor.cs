using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Floor", menuName = "World/Floor")]
public class Floor : ScriptableObject
{
    public new string name;
    public int level;
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
    public int maximumLocations = 30;
    public bool addOffset;
    public bool forceBossCenter = false;

    [Header("[ Location Specifications ]")]
    public int eliteCount = 0;
    public int campfireCount = 0;
    public int treasureCount = 0;
    public int mysteryCount = 0;

    [Header("[ Campfires ]")]
    public EffectObject wellRestedEffect;
    [Tooltip("Value in percentages (integer)")]
    public int wellRestedChance;

    [Header("[ Treasure Contents ]")]
    public List<ItemDrop> itemPool = new List<ItemDrop>();

    [Header("[ Encounters ]")]
    public List<Encounter> trashEncounters;
    public List<Encounter> eliteEncounters;
    public Encounter bossEncounter;

    [Header("[ Mystery Events ]")]
    public MysteryEvents mysteryEvents;
}
