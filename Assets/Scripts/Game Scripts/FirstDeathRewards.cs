using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FirstDeathRewards
{
    private CurrencyHandler currencyHandler;

    [SerializeField] private Equipment flaskReward;

    public void Reward(CurrencyHandler _currencyHandler)
    {
        currencyHandler = _currencyHandler;

        // Reward first death stuff
        // Give each hero 1 option out of 3 random abilities
        // Give ability currency for now
        currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, 30));

        if (flaskReward != null)
        {
            foreach (Unit unit in TeamManager.Instance.heroes.Members)
            {
                (unit as Hero).ForceEquipItem(flaskReward);
            }
        }


        NotificationObject.SendNotification("That was rough... maybe these 30 Spirit Fragments will help you, and take these flasks just in case.", new List<Reward>() { new Reward(new Currency(CurrencyType.Spirit, 30)) });

        Debug.Log("This is your first death.");
    }
}
