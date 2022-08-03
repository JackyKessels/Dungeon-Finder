using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityShop : Shop
{
    [Header("[ Ability Shop ]")]
    public AbilityOptionsWindow abilityOptionsWindow;

    public GameObject heroPathOptionPrefab;

    public override void SetupShop()
    {
        ObjectUtilities.ClearContainer(shopContainer);

        for (int i = 0; i < teamManager.heroes.Members.Count; i++)
        {
            if (teamManager.heroes.Members[i] != null)
            {
                CreateHeroPathOptions(teamManager.heroes.Members[i] as Hero);
            }
        }
    }

    private void CreateHeroPathOptions(Hero hero)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(heroPathOptionPrefab, shopContainer);

        obj.GetComponent<HeroPathOption>().CreatePathOptions(hero);
    }

    public override void RemoveFromShop(ShopItem shopItem)
    {
        //
    }
}
