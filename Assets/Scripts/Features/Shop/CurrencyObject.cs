using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyObject : MonoBehaviour
{
    public CurrencyType currencyType;

    public Image icon;
    public TextMeshProUGUI totalAmount;

    public bool isActive;

    private TooltipObject iconTooltip;
    private TooltipObject totalTooltip;

    public void UpdateData(Currency total)
    {
        currencyType = total.currencyType;

        iconTooltip = icon.GetComponent<TooltipObject>();
        totalTooltip = totalAmount.GetComponent<TooltipObject>();

        iconTooltip.currency = total;
        totalTooltip.useGenericTooltip = true;
        totalTooltip.genericTooltip = string.Format("This is your total amount of <color={0}><b>{1}</b></color>.", Currency.GetCurrencyColor(total.currencyType), Currency.GetCurrencyName(total.currencyType));

        totalAmount.text = total.totalAmount.ToString();
        totalAmount.color = GeneralUtilities.ConvertString2Color(Currency.GetCurrencyColor(currencyType));

        icon.sprite = Currency.GetCurrencyIcon(currencyType, false);
    }
}
