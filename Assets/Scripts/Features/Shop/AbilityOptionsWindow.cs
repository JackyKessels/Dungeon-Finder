using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOptionsWindow : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject abilityOptionPrefab;
    [SerializeField] private GameObject extraCostPrefab;
    [SerializeField] private GameObject upgradeIconPrefab;

    private Hero hero;
    private Currency cost;

    private readonly int abilityOptions = 3;
    private readonly int offspecOptions = 1;

    private readonly int priceMultiplier = 5;

    private void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            Refund();
        }
    }

    public void CreateOptions(Hero _hero, HeroPathObject heroPathObject, Currency _cost)
    {
        hero = _hero;
        cost = _cost;

        HeroManager.Instance.SetCurrentHero(hero);

        ObjectUtilities.ClearContainer(container);

        List<ActiveAbility> options = heroPathObject.GetRandomActiveAbilities(abilityOptions);

        for (int i = 0; i < abilityOptions; i++)
        {
            Active active = new Active(options[i], hero.spellbook.LevelOfAbility(options[i]) + 1);

            AddAbilityOption(active);
        }

        for (int i = 0; i < offspecOptions; i++)
        {
            ActiveAbility activeAbility = hero.heroPathManager.GetRandomAbilityFromOtherPaths(heroPathObject);

            Active active = new Active(activeAbility, hero.spellbook.LevelOfAbility(activeAbility) + 1);

            AddAbilityOption(active);
        }
    }

    private void AddAbilityOption(Active active)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityOptionPrefab, container);

        obj.GetComponent<TooltipObject>().active = active;
        obj.GetComponent<TooltipObject>().state = CurrentState.HeroInformation;

        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(96, 96);

        obj.GetComponent<Image>().sprite = active.activeAbility.icon;
        obj.GetComponent<Button>().onClick.AddListener(delegate { SelectOption(active); });

        GameObject extra = ObjectUtilities.CreateSimplePrefab(extraCostPrefab, obj);
        extra.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);
        ExtraCostObject extraCost = extra.GetComponent<ExtraCostObject>();

        if (active.level > 1)
        {
            extraCost.Setup(new Currency(CurrencyType.Spirit, UpgradeCost(active.level)), true);

            GameObject upgrade = ObjectUtilities.CreateSimplePrefab(upgradeIconPrefab, obj);
            upgrade.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            extraCost.Setup(new Currency(CurrencyType.Spirit, 0), false);
        }
    }

    public void SelectOption(Active active)
    {
        CurrencyHandler currencyHandler = GameManager.Instance.currencyHandler;

        // It is an upgrade
        if (active.level > 1)
        {
            Currency cost = new Currency(CurrencyType.Spirit, UpgradeCost(active.level));

            if (currencyHandler.CanBuy(cost))
            {
                currencyHandler.Buy(cost);

                SuccessfulPurchase(active);

                //ShortMessage.SendMessage(Input.mousePosition, "Successful purchase!", 24, Color.green);
            }
            else
            {
                ShortMessage.SendMessage(Input.mousePosition, "Not enough currency.", 24, Color.red);
            }
        }
        // It is a new ability
        else
        {
            SuccessfulPurchase(active);
        }
    }

    private int UpgradeCost(int level)
    {
        return priceMultiplier * level;
    }

    private void SuccessfulPurchase(Active active)
    {
        hero.spellbook.LearnAbility(active);

        TooltipHandler.Instance.HideTooltip();
        gameObject.SetActive(false);
    }

    public void Refund()
    {
        Currency refund = new Currency(cost.currencyType, cost.totalAmount / 2);

        GameManager.Instance.currencyHandler.IncreaseCurrency(refund);

        gameObject.SetActive(false);
    }

}
