using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EquipmentSlot
{
    Helmet,
    Armor,
    TwoHand,
    OneHand,
    Shield,
    Relic,
    Necklace,
    Ring,
    Trinket,
    Flask,
    Nothing
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/Equipment")]
[System.Serializable]
public class EquipmentObject : ItemObject
{
    public EquipmentSlot slot;

    [Header("[ General Attributes ]")]
    public float healthFactor = 0f;
    public float powerFactor = 0f;
    public float wisdomFactor = 0f;
    public int armor = 0;
    public int resistance = 0;
    public int vitality = 0;
    public int speed = 0;
    public int accuracy = 0;
    public int crit = 0;

    [Header("[ School Modifiers ]")]
    public int healingMultiplier = 0;
    public int physicalMultiplier = 0;
    public int fireMultiplier = 0;
    public int iceMultiplier = 0;
    public int natureMultiplier = 0;
    public int arcaneMultiplier = 0;
    public int holyMultiplier = 0;
    public int shadowMultiplier = 0;
    public int critMultiplier = 0;
    public int sacrificialMultiplier = 0;

    [Header("[ Additional ]")]
    public List<PassiveAbility> passives = new List<PassiveAbility>();
    public ActiveAbility useAbility;

    public string ParseSlot()
    {
        if (slot == EquipmentSlot.OneHand)
        {
            return string.Format("\nSlot: One-hand");
        }
        else if (slot == EquipmentSlot.TwoHand)
        {
            return string.Format("\nSlot: Two-hand");
        }
        else
        {
            return string.Format("\nSlot: {0}", slot);
        }
    }
}
