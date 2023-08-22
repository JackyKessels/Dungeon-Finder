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
    public float healthFactor;
    public float powerFactor;
    public float wisdomFactor;
    public int armor;
    public int resistance;
    public int vitality;
    public int speed;
    public int accuracy;
    public int crit;

    [Header("[ School Modifiers ]")]
    public int healingMultiplier;
    public int physicalMultiplier;
    public int fireMultiplier;
    public int iceMultiplier;
    public int natureMultiplier;
    public int arcaneMultiplier;
    public int holyMultiplier;
    public int shadowMultiplier;
    public int critMultiplier;

    [Header("[ Additional ]")]
    public List<PassiveAbility> passives;
    public ActiveAbility useAbility;
}
