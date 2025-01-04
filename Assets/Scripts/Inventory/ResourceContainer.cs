using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Container", menuName = "Item/Consumable/Resource Container")]
public class ResourceContainer : Consumable
{
    public Currency currency;

    public override bool Consume(int i)
    {
        GameManager.Instance.currencyHandler.IncreaseCurrency(currency);

        return true;
    }

    public override string GetTooltip(TooltipObject tooltipInfo)
    {
        return $"\nUse: Gain {currency.totalAmount} <color={Currency.GetCurrencyColor(currency.currencyType)}>{Currency.GetCurrencyName(currency.currencyType)}</color>.";
    }
}
