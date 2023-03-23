using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockAbilityButton : MonoBehaviour
{
    private CurrencyHandler currencyHandler;
    private TownManager townManager;

    public Image icon;
    public Button button;
    public TooltipObject tooltipObject;

    private readonly Currency cost = new Currency(CurrencyType.Spirit, 10);

    private void Awake()
    {
        currencyHandler = GameManager.Instance.currencyHandler;
        townManager = TownManager.Instance;
    }

    public void Setup(HeroPathObject heroPathObject, Hero hero)
    {
        name = heroPathObject.name;
        icon.sprite = heroPathObject.icon;
        button.onClick.AddListener(delegate { SelectPath(heroPathObject, hero); });
        tooltipObject.genericTooltip = PathTooltip(heroPathObject);
    }

    private string PathTooltip(HeroPathObject heroPathObject)
    {
        string pathColor = "#FFEEA8";

        string option1Color = "#00FF00";
        string option2Color = "#FFFF00";
        string option3Color = "#FF0000";

        return string.Format("<color={0}>{1}</color>" +
                             "\n" +
                             "\nOption 1: <color={2}>100%</color> {1}" +
                             "\nOption 2: <color={2}>100%</color> {1}" +
                             "\nOption 3: <color={4}>0%</color> {1}", pathColor, heroPathObject.name, option1Color, option2Color, option3Color);
    }

    private void SelectPath(HeroPathObject heroPathObject, Hero hero)
    {
        if (hero.spellbook.IsCollectionFull())
        {
            ShortMessage.SendMessage(Input.mousePosition, "Ability Collection is full.", 24, Color.red);
            return;
        }

        if (currencyHandler.CanBuy(cost))
        {
            if (!townManager.enchanter.abilityOptionsWindow.gameObject.activeSelf)
            {
                currencyHandler.Buy(cost);

                townManager.enchanter.abilityOptionsWindow.gameObject.SetActive(true);
                townManager.enchanter.abilityOptionsWindow.CreateOptions(hero, heroPathObject, cost);

                ShortMessage.SendMessage(Input.mousePosition, "Successful purchase!", 24, Color.green);
            }
            else
            {
                ShortMessage.SendMessage(Input.mousePosition, "Already in the process of buying.", 24, Color.red);
            }
        }
        else
        {
            ShortMessage.SendMessage(Input.mousePosition, "Not enough currency.", 24, Color.red);
        }
    }
}
