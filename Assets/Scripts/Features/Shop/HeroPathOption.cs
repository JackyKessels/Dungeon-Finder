using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroPathOption : MonoBehaviour
{
    private CurrencyHandler currencyHandler;
    private TownManager townManager;

    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] GameObject container;
    [SerializeField] GameObject pathPrefab;

    private Hero hero;
    private Currency cost;

    private void Awake()
    {
        currencyHandler = GameManager.Instance.currencyHandler;
        townManager = TownManager.Instance;

        cost = new Currency(CurrencyType.Spirit, 10);
    }

    public void CreatePathOptions(Hero _hero)
    {
        hero = _hero;

        if (hero.heroObject.lastName == "")
            heroName.text = hero.heroObject.name;
        else
            heroName.text = hero.heroObject.name + " " + hero.heroObject.lastName;

        ObjectUtilities.ClearContainer(container);

        foreach (HeroPath heroPath in hero.heroPathManager.paths)
        {
            CreatePath(heroPath.path);
        }
    }

    private void CreatePath(HeroPathObject heroPathObject)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(pathPrefab, container);

        obj.GetComponent<TooltipObject>().genericTooltip = PathTooltip(heroPathObject);
        obj.GetComponent<Button>().onClick.AddListener(delegate { SelectPath(heroPathObject); });
        obj.GetComponent<Image>().sprite = heroPathObject.icon;
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
                             "\nOption 2: <color={3}><100%</color> {1}" +
                             "\nOption 3: <color={4}>0%</color> {1}", pathColor, heroPathObject.name, option1Color, option2Color, option3Color);  
    }

    private void SelectPath(HeroPathObject heroPathObject)
    {
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
