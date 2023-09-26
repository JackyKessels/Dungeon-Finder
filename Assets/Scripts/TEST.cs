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

    private void Start()
    {
        gameManager = GameManager.Instance;

        hero_0_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_0); });
        hero_1_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_1); });
        hero_2_button.onClick.AddListener(delegate { OpenAbilityWindow(hero_2); });
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
        int value = ExperienceManager.ExperienceToNextLevel();

        TeamManager.Instance.experienceManager.GainExperience(value, false);
    }

    public void TestDialogue()
    {
        DialogueManager.Instance.Setup(testConversation);
    }

    public void SpawnTest()
    {
        TeamManager.Instance.SpawnEnemy(testEnemy, 1, true, null);
    }

    public void Flee()
    {
        BattleManager.Instance.OnFleeButton();
    }

    public void Suicide()
    {
        foreach (Unit unit in TeamManager.Instance.heroes.Members)
        {
            unit.statsManager.TakeDamage(AbilitySchool.Shadow, 9999);
        }

        BattleManager.Instance.CheckWinCondition();
    }

    public void PowerOverwhelming()
    {
        foreach (Unit unit in TeamManager.Instance.heroes.Members)
        {
            unit.statsManager.ModifyAttribute(AttributeType.Power, AttributeValue.bonusValue, 5000);
            unit.statsManager.ModifyAttribute(AttributeType.Wisdom, AttributeValue.bonusValue, 5000);
        }
    }

    public void DamageEnemies()
    {
        foreach (Unit unit in TeamManager.Instance.enemies.LivingMembers)
        {
            unit.statsManager.TakeDamage(AbilitySchool.Physical, 1000);
        }

        BattleManager.Instance.CheckWinCondition();
    } 

    public void ResetPathPoints()
    {
        foreach (Hero hero in TeamManager.Instance.heroes.Members)
        {
            hero.heroPathManager.points = 0;
        }
    }

    public void NextFloor()
    {
        if (GameManager.Instance.gameState == GameState.RUN)
        {
            DungeonManager.Instance.NextFloor();
        }
        else
        {
            Debug.Log("No dungeon active.");
        }
    }

    public void TestGloomstalkThicket()
    {
        int level = 5;
        int spiritShards = 60;

        //

        ProgressionManager.Instance.FirstDeath();
        ProgressionManager.Instance.unlockedPaths = true;
        ProgressionManager.Instance.SetPathButtonState();
        ProgressionManager.Instance.UnlockFourthAbility();
        ProgressionManager.Instance.UnlockEnchanterUpgrade();
        ProgressionManager.Instance.campaignManager.SetDungeonList(true);

        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, spiritShards));

        int exp = ExperienceManager.TotalExperienceToLevel(level);

        TeamManager.Instance.experienceManager.GainExperience(exp);
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


}


