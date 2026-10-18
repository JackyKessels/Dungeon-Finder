using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleRewardObject : MonoBehaviour
{
    public TooltipObject tooltipObject;
    public TextMeshProUGUI amount;

    public void Setup(int experience = 0)
    {
        tooltipObject.experience = true;
        amount.text = experience.ToString();
    }

    public void Setup(Currency currency)
    {
        tooltipObject.gameObject.AddComponent<CurrencyObject>();
        tooltipObject.currency = currency;
        tooltipObject.GetComponent<Image>().sprite = Currency.GetCurrencyIcon(currency.currencyType, false);

        amount.text = currency.totalAmount.ToString();
    }

    public void Setup(ItemDrop itemDrop, int amountAdded)
    {
        tooltipObject.item = itemDrop.GetItem();
        tooltipObject.GetComponent<Image>().sprite = itemDrop.itemObject.icon;
        amount.text = amountAdded.ToString();
    }

    public void Setup(RemainingLoot loot, System.Action onClick)
    {
        tooltipObject.item = loot.item;
        tooltipObject.GetComponent<Image>().sprite = loot.itemObject.icon;
        amount.text = loot.amount.ToString();

        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
