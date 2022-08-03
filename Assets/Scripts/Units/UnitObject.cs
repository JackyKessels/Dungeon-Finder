using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : ScriptableObject
{
    [Header("General")]
    new public string name;
    public string lastName;
    public UnitType unitType;
    public Sprite icon;
    public Sprite sprite;

    [Header("Animations")]
    public Sprite attackAnimation;
    public Sprite castAnimation;

    [Header("Stats")]
    public int baseHealth;
    public int basePower;
    public int baseWisdom;
    public int baseArmor;
    public int baseResistance;
    public int baseVitality;
    public int speed = 5;
    public int accuracy = 80;
    public int criticalHit = 0;

    public int healingMultiplier = 100;
    public int crushingMultiplier = 100;
    public int piercingMultiplier = 100;
    public int fireMultiplier = 100;
    public int iceMultiplier = 100;
    public int natureMultiplier = 100;
    public int arcaneMultiplier = 100;
    public int holyMultiplier = 100;
    public int shadowMultiplier = 100;
    public int critMultiplier = 150;
}
