﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute
{
    private Sprite icon;
    public AttributeType attributeType;
    public int baseValue;
    public int bonusValue;
    public float mulitplier;

    public Attribute(AttributeType type)
    {
        attributeType = type;
        baseValue = 0;
        bonusValue = 0;
        mulitplier = 1f;
    }

    public Attribute(AttributeType type, int value)
    {
        attributeType = type;
        baseValue = value;
        bonusValue = 0;
        mulitplier = 1f;

        icon = GeneralUtilities.GetAttributeIcon(attributeType);
    }

    public int GetTotalValue()
    {
        return GeneralUtilities.RoundFloat((baseValue + bonusValue) * mulitplier);
    }
}

