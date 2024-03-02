using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ItemObject : ScriptableObject
{
    [Header("General")]
    new public string name = "New Item";
    public Sprite icon = null;
    public int value = 0;
    public bool stackable = false;
    public int maxStacks = 1;
    public bool sellable = true;
    [Tooltip("This item can only be looted once.")]
    public bool unique = false;

    public Item item = new Item();

    [TextArea(5, 5)] public string description;

    public Quality quality = Quality.Common;
}
