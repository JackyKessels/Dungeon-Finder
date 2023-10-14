using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterShop : Shop
{
    [Header("[ Chapter Shop ]")]
    public int itemsDisplayed = 3;

    public TextMeshProUGUI currentHeroName;

    public TextMeshProUGUI noAbilities;

    public List<CurrentShopItem> currentChapterAbilities;

    public override void SetupShop()
    {
        currentHeroName.text = teamManager.heroes.GetUnit(currentHero).name;

        ObjectUtilities.ClearContainer(shopContainer);

        UpdateCosts();

        if (currentChapterAbilities.Count > 0)
        {
            NoAbilities(false);

            for (int i = 0; i < currentChapterAbilities.Count; i++)
            {
                AddItemToShop(currentChapterAbilities[i], this, true);
            }
        }
        else
        {
            NoAbilities(true);
        }
    }

    public void BuildShop(List<AbilityObject> abilities)
    {
        currentChapterAbilities = new List<CurrentShopItem>();

        Hero hero = teamManager.heroes.GetUnit(currentHero) as Hero;

        List<AbilityObject> tempList = new List<AbilityObject>(abilities);
        List<AbilityObject> newList = new List<AbilityObject>();

        if (itemsDisplayed > tempList.Count)
        {
            newList = new List<AbilityObject>(tempList);
        }
        else
        {
            for (int i = 0; i < itemsDisplayed; i++)
            {
                int randomItem = Random.Range(0, tempList.Count);

                newList.Add(tempList[randomItem]); ;
                tempList.RemoveAt(randomItem);
            }
        }

        foreach (AbilityObject ability in newList)
        {
            CurrentShopItem currentShopItem = new CurrentShopItem(new Reward(ability as ActiveAbility), new Currency(CurrencyType.Spirit, GetAbilityCost(ability, hero)));
            currentShopItem.reward.ability.level = hero.spellbook.GetAbilityLevel(ability);

            currentChapterAbilities.Add(currentShopItem);
        }
    }

    private void UpdateCosts()
    {
        Hero hero = teamManager.heroes.GetUnit(currentHero) as Hero;

        foreach (CurrentShopItem currentShopItem in currentChapterAbilities)
        {
            AbilityObject abilityObject = currentShopItem.reward.ability.activeAbility;

            currentShopItem.cost.totalAmount = GetAbilityCost(abilityObject, hero);
            currentShopItem.reward.ability.level = hero.spellbook.GetAbilityLevel(abilityObject);
        }
    }

    public void RemoveFromShop(ShopItem shopItem)
    {
        RemoveItemFromList(currentChapterAbilities, shopItem);

        if (currentChapterAbilities.Count <= 0)
        {
            NoAbilities(true);
        }
    }

    public void NextHero()
    {
        if (currentHero < teamManager.heroes.Members.Count - 1)
        {
            currentHero++;
        }
        else
        {
            currentHero = 0;
        }

        SetupShop();
    }

    public void PreviousHero()
    {
        if (currentHero > 0)
        {
            currentHero--;
        }
        else
        {
            currentHero = teamManager.heroes.Members.Count - 1;
        }

        SetupShop();
    }

    private void NoAbilities(bool show)
    {
        noAbilities.gameObject.SetActive(show);
    }

    // Save & Load
    public void LoadShop(List<int> abilityIDs)
    {
        DatabaseHandler databaseHandler = DatabaseHandler.Instance;

        currentChapterAbilities = new List<CurrentShopItem>();

        for (int i = 0; i < abilityIDs.Count; i++)
        {
            ActiveAbility ability = databaseHandler.abilityDatabase.abilityObjects[abilityIDs[i]] as ActiveAbility;

            CurrentShopItem currentShopItem = new CurrentShopItem(new Reward(ability), new Currency(CurrencyType.Spirit, 0));

            currentChapterAbilities.Add(currentShopItem);
        }
    }
}
