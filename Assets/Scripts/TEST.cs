using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject window;

    public HeroObject addedHero;

    public Conversation testConversation;

    public EnemyObject testEnemy;

    [Header("[ Abilities ]")]
    public GameObject testPathPrefab;
    public GameObject testPathContainer;

    public Button hero_0_button;
    public Button hero_1_button;
    public Button hero_2_button;

    public HeroObject hero_0;
    public HeroObject hero_1;
    public HeroObject hero_2;


    public HeroObject currentHero = null;

    public Button fleeButton;

    private void Start()
    {
        gameManager = GameManager.Instance;

        hero_0_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_0); });
        hero_1_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_1); });
        hero_2_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_2); });

        fleeButton.gameObject.SetActive(true);
    }

    public void AddCurrency()
    {
        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Gold, 1000));
        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, 1000));

        gameManager.currencyHandler.UpdateCurrencies();
    }

    public void ClearInventory()
    {
        //foreach (InventoryObject inventory in InventoryManager.Instance.equipmentObjects)
        //{
        //    inventory.container.Clear();
        //}

        InventoryManager.Instance.inventoryObject.container.Clear();
    }

    public void AddPathPoints()
    {
        foreach (Hero hero in TeamManager.Instance.heroes.Members)
        {
            hero.heroPathManager.points += 40;
        }
    }

    public void TestBox()
    {
        ConfirmationBoxHandler.Instance.SetupConfirmationBox("Are you sure?", new List<ConfirmationButton>()
                                                                            { new ConfirmationButton("Yes", ButtonYes),
                                                                              new ConfirmationButton("No", ButtonNo) });
    }

    public void StarterPath()
    {
        HeroManager.Instance.SetupStarterPath(HeroManager.Instance.CurrentHero());
    }

    public void ButtonYes()
    {
        Debug.Log("YES");
    }

    public void ButtonNo()
    {
        Debug.Log("NO");
    }

    public void ToggleWindow()
    {
        window.SetActive(!window.activeSelf);
    }

    public void GainExperience()
    {
        TeamManager.Instance.experienceManager.LevelUpTeam(false);
    }

    public void SaveGame()
    {
        TeamManager.Instance.SaveTeam();
    }

    public void TestDialogue()
    {
        DialogueManager.Instance.Setup(testConversation);
    }

    public void SpawnTest()
    {
        TeamManager.Instance.SpawnEnemy(testEnemy, true, null);
    }

    public void Flee()
    {
        BattleManager.Instance.OnFleeButton();
    }

    public void ResetPathPoints()
    {

    }

    // ABILITIES

    public void OpenAbilityWindow(HeroObject heroObject)
    {
        if (currentHero == heroObject && testPathContainer.activeSelf)
            testPathContainer.SetActive(false);
        else if (currentHero == heroObject && !testPathContainer.activeSelf)
            testPathContainer.SetActive(true);
        else
        {
            testPathContainer.SetActive(true);

            SetupAbilityWindow(heroObject);
        }

        currentHero = heroObject;
    }

    private void SetupAbilityWindow(HeroObject heroObject)
    {
        ObjectUtilities.ClearContainer(testPathContainer);

        foreach (HeroPathObject heroPathObject in heroObject.paths)
        {
            CreateSpec(heroPathObject, testPathContainer);
        }
    }

    private void CreateSpec(HeroPathObject heroPathObject, GameObject container)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(testPathPrefab, container);

        TestSpec testSpec = obj.GetComponent<TestSpec>();
        testSpec.Setup(heroPathObject);
    }

    public void PowerOverwhelming()
    {
        foreach (Unit unit in TeamManager.Instance.heroes.Members)
        {
            unit.statsManager.GetAttribute(AttributeType.Power).bonusValue += 5000;
            unit.statsManager.GetAttribute(AttributeType.Wisdom).bonusValue += 5000;
        }
    }
}


