using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Container", menuName = "Item/Consumable/Resource Container")]
public class ResourceContainer : Consumable
{
    public Currency currency;

    public override void Consume(int i)
    {
        GameManager.Instance.currencyHandler.IncreaseCurrency(currency);      
    }

    public string GetDescription(TooltipObject tooltipInfo, string itemName, string itemDescription)
    {
        return itemName + 
               string.Format("\nUse: Gain {1} <color={0}>{2}</color>.", 
                             Currency.GetCurrencyColor(currency.currencyType),
                             currency.totalAmount, 
                             Currency.GetCurrencyName(currency.currencyType)) +
               itemDescription;
    }
}
