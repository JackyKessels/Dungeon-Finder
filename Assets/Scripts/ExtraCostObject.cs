using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtraCostObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TextMeshProUGUI textNoIcon;

    public void Setup(Currency currency, bool isUpgrade)
    {
        if (isUpgrade)
        {
            textField.text = currency.totalAmount.ToString();
            textField.color = GeneralUtilities.ConvertString2Color(Currency.GetCurrencyColor(currency.currencyType));
            currencyIcon.sprite = Currency.GetCurrencyIcon(currency.currencyType, false);
        }
        else
        {
            textField.gameObject.SetActive(false);
            currencyIcon.gameObject.SetActive(false);

            textNoIcon.gameObject.SetActive(true);
            textNoIcon.color = GeneralUtilities.ConvertString2Color(Currency.GetCurrencyColor(currency.currencyType));
        }
    }
}
