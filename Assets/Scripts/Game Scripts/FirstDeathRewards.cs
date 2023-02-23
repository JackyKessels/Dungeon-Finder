using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FirstDeathRewards
{
    private CurrencyHandler currencyHandler;

    public void Reward(CurrencyHandler _currencyHandler)
    {
        currencyHandler = _currencyHandler;

        // Reward first death stuff
        // Give each hero 1 option out of 3 random abilities
        // Give ability currency for now
        currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, 30));


        NotificationObject.SendNotification("That was rough... maybe these 30 Spirit Fragments will help you.", new List<Reward>() { new Reward(new Currency(CurrencyType.Spirit, 30)) });

        Debug.Log("This is your first death.");
    }
}
