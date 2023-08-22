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