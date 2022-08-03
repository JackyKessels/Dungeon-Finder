using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : Shop
{
    public GameObject townHallUpgradePrefab;

    [SerializeField] private TownHallUpgrade[] townHallUpgrades;

    public void Initialize()
    {
        townHallUpgrades = new TownHallUpgrade[3];

        townHallUpgrades[0] = new TownHallUpgrade(0, "Blacksmith", new Currency(CurrencyType.Gold, 0), UnlockBlacksmith);
        townHallUpgrades[1] = new TownHallUpgrade(1, "Enchanter", new Currency(CurrencyType.Gold, 0), UnlockEnchanter);
        townHallUpgrades[2] = new TownHallUpgrade(2, "Trophy Hunter", new Currency(CurrencyType.Gold, 0), UnlockTrophyHunter);
    }

    public override void SetupShop()
    {
        ObjectUtilities.ClearContainer(shopContainer);

        foreach (TownHallUpgrade townHallUpgrade in townHallUpgrades)
        {
            if (!townHallUpgrade.unlocked)
                AddUpgradeToShop(townHallUpgrade);
        }
    }


    private void UnlockBlacksmith()
    {
        townManager.blacksmith.UnlockShop();

        // Add blacksmith upgrades to new townhall tab
    }

    private void UnlockEnchanter()
    {
        townManager.enchanter.UnlockShop();

        // Add enchanter upgrades to new townhall tab
    }

    private void UnlockTrophyHunter()
    {
        townManager.trophyHunter.UnlockShop();

        // Add trophy hunter upgrades to new townhall tab
    }

    private void AddUpgradeToShop(TownHallUpgrade townHallUpgrade)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(townHallUpgradePrefab, shopContainer);

        TownHallUpgradeObject townHallUpgradeObject = obj.GetComponent<TownHallUpgradeObject>();
        townHallUpgradeObject.SetData(townHallUpgrade);
    }

    public override void RemoveFromShop(ShopItem shopItem)
    {

    }
}

[Serializable]
public class TownHallUpgrade
{
    public bool unlocked;

    public int id;
    public string name;
    public Currency cost;

    public Action unlockAction;

    public TownHallUpgrade(int _id, string _name, Currency _cost, Action _unlockAction)
    {
        unlocked = false;

        id = _id;
        name = _name;
        cost = _cost;

        unlockAction = _unlockAction;
    }
}
