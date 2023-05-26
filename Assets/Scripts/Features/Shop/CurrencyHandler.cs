using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType
{
    Gold,
    Spirit
}

public delegate void CurrencyUpdated();

public class CurrencyHandler : MonoBehaviour
{
    private GameManager gameManager;

    public CurrencyObject currencyGold;
    public CurrencyObject currencySpirit;

    public Currency totalGold;
    public Currency totalSpirit;

    private void Start()
    {
        gameManager = GameManager.Instance;

        Debug.Log("Initialize Currencies");

        InitializeCurrencies();
    }

    public void UpdateCurrencies()
    {
        currencyGold.UpdateData(totalGold);
        currencySpirit.UpdateData(totalSpirit);
    }

    public void SetState(bool state)
    {
        currencyGold.isActive = state;
        currencySpirit.isActive = state;
    }

    public void InitializeCurrencies()
    {
        totalGold = new Currency(CurrencyType.Gold, 0);
        totalSpirit = new Currency(CurrencyType.Spirit, 0);
    }

    private Currency GetCorrectCurrency(CurrencyType type)
    {
        if (type == CurrencyType.Gold)
        {
            return totalGold;
        }
        else if (type == CurrencyType.Spirit)
        {
            return totalSpirit;
        }
        else
        {
            return null;
        }
    }

    public void IncreaseCurrency(Currency currency)
    {
        if (currency.totalAmount == 0)
            return;

        GetCorrectCurrency(currency.currencyType).totalAmount += currency.totalAmount;

        UpdateCurrencies();

        Debug.Log(currency.currencyType + " increased by " + currency.totalAmount + " to " + GetAmount(currency.currencyType));
    }

    public void DecreaseCurrency(Currency currency)
    {
        if (currency.totalAmount == 0)
            return;

        Currency currentCurrency = GetCorrectCurrency(currency.currencyType);

        int currentAmount = currentCurrency.totalAmount;

        if (currentAmount - currency.totalAmount < 0)
            currentCurrency.totalAmount = 0;
        else
            currentCurrency.totalAmount -= currency.totalAmount;

        UpdateCurrencies();

        Debug.Log(currency.currencyType + " decreased by " + currency.totalAmount + " to " + GetAmount(currency.currencyType));
    }

    public int GetAmount(CurrencyType type)
    {
        return GetCorrectCurrency(type).totalAmount;
    }

    public bool CanBuy(Currency cost)
    {
        if (GetCorrectCurrency(cost.currencyType).totalAmount >= cost.totalAmount)
            return true;
        else
            return false;
    }

    public void Buy(Currency cost)
    {
        GetCorrectCurrency(cost.currencyType).totalAmount -= cost.totalAmount;

        UpdateCurrencies();
    }
}

[System.Serializable]
public class Currency : IDescribable
{
    public CurrencyType currencyType;
    public int totalAmount;

    public Currency(CurrencyType type, int amount)
    {
        currencyType = type;
        totalAmount = amount;
    }

    public string GetDescription(TooltipObject tooltipInfo)
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                return string.Format("<color={0}><b>{1}</b></color>\n\nThis is a common currency that can be used to buy items.", GetCurrencyColor(CurrencyType.Gold), GetCurrencyName(CurrencyType.Gold));
            case CurrencyType.Spirit:
                return string.Format("<color={0}><b>{1}</b></color>\n\nThis is a rare currency that can be used to unlock new abilities or upgrade ones you have.", GetCurrencyColor(CurrencyType.Spirit), GetCurrencyName(CurrencyType.Spirit));
            default:
                return "This is a currency.";
        }
    }

    public static string GetCurrencyName(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                return "Gold";
            case CurrencyType.Spirit:
                return "Spirit";
            default:
                return "";
        }
    }

    public static Sprite GetCurrencyIcon(CurrencyType type, bool border)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                if (border)
                    return GameAssets.i.goldIcon;
                else
                    return GameAssets.i.goldIconBorderless;
            case CurrencyType.Spirit:
                if (border)
                    return GameAssets.i.spiritOrbIcon;
                else
                    return GameAssets.i.spiritOrbIconBorderless;
            default:
                return GameAssets.i.goldIconBorderless;
        }
    }

    public static string GetCurrencyColor(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                return "#FFC100";
            case CurrencyType.Spirit:
                return "#00FF87";
            default:
                return "#FFFFFF";
        }
    }
}


