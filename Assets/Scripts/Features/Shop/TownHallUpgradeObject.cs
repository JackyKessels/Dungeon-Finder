using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TownHallUpgradeObject : MonoBehaviour
{
    private CurrencyHandler currencyHandler;
    private TownManager townManager;

    private TownHallUpgrade townHallUpgrade;

    public TextMeshProUGUI townHallUpgradeTitle;
    public TextMeshProUGUI townHallUpgradeCost;

    private void Awake()
    {
        currencyHandler = GameManager.Instance.currencyHandler;
        townManager = TownManager.Instance;
    }

    public void SetData(TownHallUpgrade _townHallUpgrade)
    {
        townHallUpgrade = _townHallUpgrade;

        townHallUpgradeTitle.text = townHallUpgrade.name;
        townHallUpgradeCost.text = townHallUpgrade.cost.totalAmount.ToString();
    }

    public void BuyUpgrade()
    {
        if (currencyHandler.CanBuy(townHallUpgrade.cost))
        {
            currencyHandler.Buy(townHallUpgrade.cost);

            ShortMessage.SendMessage(Input.mousePosition, "Successful purchase!", 24, Color.green);
            townHallUpgrade.unlockAction?.Invoke();
            townHallUpgrade.unlocked = true;

            Destroy(gameObject);
        }
        else
        {
            ShortMessage.SendMessage(Input.mousePosition, "Not enough currency.", 24, Color.red);
        }
    }
}
