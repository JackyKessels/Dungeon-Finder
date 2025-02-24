﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipIcon : MonoBehaviour
{
    public Image icon;
    public TooltipObject tooltipObject;

    public void Setup(Sprite icon, string genericTooltip)
    {
        this.icon.sprite = icon;

        tooltipObject.useGenericTooltip = true;
        tooltipObject.genericTooltip = genericTooltip;
    }

    public void Setup(AbilityObject abilityObject, int level = 1)
    {
        if (abilityObject == null)
            return;      

        icon.sprite = abilityObject.icon;

        if (abilityObject is ActiveAbility active)
            tooltipObject.active = new Active(active, level);

        if (abilityObject is PassiveAbility passive)
            tooltipObject.passive = new Passive(passive, level);
    }

    public void Setup(EffectObject effectObject)
    {
        if (effectObject == null)
            return;

        icon.sprite = effectObject.icon;

        tooltipObject.state = CurrentState.Values;
        tooltipObject.effect = new Effect(effectObject);
    }

    public void Setup(ItemObject itemObject, int level)
    {
        icon.sprite = itemObject.icon;
        tooltipObject.item = Item.CreateItem(itemObject, level);
    }

    public void Setup(Currency currency)
    {
        icon.sprite = Currency.GetCurrencyIcon(currency.currencyType, false);
        tooltipObject.currency = currency;
    }


}
