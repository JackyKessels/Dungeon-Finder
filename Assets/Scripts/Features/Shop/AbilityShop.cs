using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityShop : Shop
{
    [Header("[ Shop Settings ]")]
    public int abilityOptions = 3;
    public int offspecOptions = 1;

    public int priceMultiplier = 5;

    [Header("[ Ability Shop ]")]
    public AbilityOptionsWindow abilityOptionsWindow;

    public GameObject heroPathOptionPrefab;

    public GameObject abilityOverview;
    public GameObject abilityOverviewPrefab;

    private bool overviewCreated = false;

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

        if (!overviewCreated)
        {
            SetupAbilityOverview();
            overviewCreated = true;
        }
    }

    private void CreateHeroPathOptions(Hero hero)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(heroPathOptionPrefab, shopContainer);

        obj.GetComponent<HeroPathOption>().CreatePathOptions(hero);
    }

    private void SetupAbilityOverview()
    {
        foreach (Hero hero in TeamManager.Instance.heroes.Members)
        {
            GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityOverviewPrefab, abilityOverview);

            HeroAbilityOverview heroAbilityOverview = obj.GetComponent<HeroAbilityOverview>();
            heroAbilityOverview.Setup(hero);
        }
    }
}
