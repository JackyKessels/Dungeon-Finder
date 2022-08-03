using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipIcon : MonoBehaviour
{
    public Image icon;
    public TooltipObject tooltipObject;

    public void Setup(AbilityObject abilityObject)
    {
        if (abilityObject == null)
            return;      

        icon.sprite = abilityObject.icon;

        if (abilityObject is ActiveAbility active)
            tooltipObject.active = new Active(active, 1);

        if (abilityObject is PassiveAbility passive)
            tooltipObject.passive = new Passive(passive, 1);
    }

    public void Setup(ItemObject itemObject)
    {
        icon.sprite = itemObject.icon;
        tooltipObject.itemObject = itemObject;
    }

    public void Setup(Currency currency)
    {
        icon.sprite = Currency.GetCurrencyIcon(currency.currencyType, false);
        tooltipObject.currency = currency;
    }
}
