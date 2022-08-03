using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Town", menuName = "World/Town")]
public class Town : ScriptableObject
{
    public new string name;
    public Sprite background;
    public List<Dungeon> availableChapters;

    [Header("Shop")]
    public List<ItemObject> availableItems;
}
