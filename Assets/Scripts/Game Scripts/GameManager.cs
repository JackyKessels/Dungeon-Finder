using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    NONE,
    TOWN,
    RUN,
    BATTLE,
    START_SCREEN
}

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private HeroManager heroManager;
    private DungeonManager dungeonManager;
    private TownManager townManager;
    private BattleManager battleManager;
    private TeamManager teamManager;
    private JourneyManager journeyManager;

    [HideInInspector] public CurrencyHandler currencyHandler;

    public AudioSource audioSourceAmbient;
    public AudioSource audioSourceSFX;

    public GameState gameState;

    [Header("TESTING STUFF")]
    public bool TEST_MODE = false;
    public Canvas TEST;
    public GameObject SKIP_BUTTON;

    [Header("[ Important Information ]")]
    public int totalBattles = 0;
    public int monstersDefeated = 0;
    public bool firstDeath = false;
    public bool unlockedPaths = false;

    [Header("[ Start & End Screen ]")]
    public Canvas startUI;
    public Canvas endUI;
    public Camera startCamera;
    public Camera endCamera;
    public Button loadButton;

    [Header("[ Introduction ]")]
    public TutorialManager tutorialManager;
    public GameObject introductionWindow1;
    public GameObject introductionWindow2;
    public GameObject introductionWindow3;
    public Conversation introductionSpeech;

    [Header("[ Team Setup ]")]
    [SerializeField] string teamName;
    [SerializeField] GameObject teamNameWindow;
    [SerializeField] TextMeshProUGUI inputField;
    [SerializeField] Button startButton;
    [SerializeField] GameObject heroSelectionContainer;
    [SerializeField] GameObject heroSelectionPrefab;

    [Header("[ Story ]")]
    public HeroObject startHero;
    public HeroObject SECOND_TEST_HERO;
    public HeroObject THIRD_TEST_HERO;
    public Town startTown;

    [Header("[ Hero Selection ]")]
    public GameObject spawnPosition;
    public HeroSelectionManager heroSelectionManager;
    private Transform titleElements;
    private Transform newGameElements;

    [Header("[ Main Systems ]")]
    public CameraScript cameraScript;
    public EnemyDatabase enemyDatabase;
    public MapButton mapButton;
    public OptionsManager optionsManager;

    [Header("[ Inventory ]")]
    public GameObject inventoryContainer;
    public InventoryObject inventory;

    private Canvas currentUI;

    private HeroSelectionObject heroSelectionObject_0;
    private HeroSelectionObject heroSelectionObject_1;
    private HeroSelectionObject heroSelectionObject_2;

    [Header("[ First Death Rewards ]")]
    [SerializeField] private FirstDeathRewards firstDeathRewards;

    private void Start()
    {
        DatabaseHandler.Instance.itemDatabase.UpdateId();
        DatabaseHandler.Instance.abilityDatabase.UpdateId();
        Debug.Log("Updated databases.");

        currentUI = startUI;

        heroManager = HeroManager.Instance;
        dungeonManager = DungeonManager.Instance;
        townManager = TownManager.Instance;
        battleManager = BattleManager.Instance;
        teamManager = TeamManager.Instance;

        journeyManager = new JourneyManager(endUI);
        currencyHandler = GetComponent<CurrencyHandler>();

        audioSourceAmbient = GetComponent<AudioSource>();

        titleElements = startUI.transform.GetChild(0);
        newGameElements = startUI.transform.GetChild(1);

        //if (SaveSystem.LoadTeam() == null)
        //    loadButton.interactable = false;
    }

    public void EnableUI(Canvas ui)
    {
        currentUI.gameObject.SetActive(false);

        ui.gameObject.SetActive(true);
        currentUI = ui;
    }

    public void ChooseTeam()
    {
        titleElements.gameObject.SetActive(false);

        teamNameWindow.SetActive(true);

        SetupTeamSelection();
    }

    private void SetupTeamSelection()
    {
        // Hero 1 stands in the middle
        heroSelectionObject_1 = ObjectUtilities.CreateSimplePrefab(heroSelectionPrefab, heroSelectionContainer).GetComponent<HeroSelectionObject>();
        heroSelectionObject_0 = ObjectUtilities.CreateSimplePrefab(heroSelectionPrefab, heroSelectionContainer).GetComponent<HeroSelectionObject>();
        heroSelectionObject_2 = ObjectUtilities.CreateSimplePrefab(heroSelectionPrefab, heroSelectionContainer).GetComponent<HeroSelectionObject>();

        heroSelectionObject_0.Setup(0);
        heroSelectionObject_1.Setup(1);
        heroSelectionObject_2.Setup(2);
    }

    public void SaveName()
    {
        teamName = inputField.text;

        startButton.interactable = false;

        ConfirmationBoxHandler.Instance.SetupConfirmationBox("Are you sure?", new List<ConfirmationButton>()
                                                                            { new ConfirmationButton("Yes", Introduction1),
                                                                              new ConfirmationButton("No", ReactiveStartButton) });
    }



    private void ReactiveStartButton()
    {
        startButton.interactable = true;
    }

    public void Introduction1()
    {
        ObjectUtilities.BlackTransition(true);

        introductionWindow1.SetActive(true);
    }

    public void Introduction2()
    {
        ObjectUtilities.BlackTransition(true);

        introductionWindow2.SetActive(true);
    }

    public void Introduction3()
    {
        ObjectUtilities.BlackTransition(true);

        introductionWindow3.SetActive(true);
    }

    // Start Story
    public void StartStory()
    {
        gameState = GameState.TOWN;

        ObjectUtilities.BlackTransition(true);

        teamNameWindow.SetActive(false);
        heroManager.SetTeamName(teamName);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        CreateTeam();

        //townManager.StartTutorial();
        townManager.SetupStratford();

        StartCoroutine(StartIntroductionDialogue());
    }

    IEnumerator StartIntroductionDialogue()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.Setup(introductionSpeech);
    }

    private void CreateTeam()
    {
        Hero hero_0 = teamManager.CreateHero(heroSelectionObject_0.GetData()[0], 0);
        Hero hero_1 = teamManager.CreateHero(heroSelectionObject_1.GetData()[0], 1);
        Hero hero_2 = teamManager.CreateHero(heroSelectionObject_2.GetData()[0], 2);

        hero_0.heroPathManager.SelectStarterPath(heroSelectionObject_0.GetData()[1]);
        hero_1.heroPathManager.SelectStarterPath(heroSelectionObject_1.GetData()[1]);
        hero_2.heroPathManager.SelectStarterPath(heroSelectionObject_2.GetData()[1]);
    }

    public void LoadGame()
    {
        gameState = GameState.TOWN;

        titleElements.gameObject.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        // Load Heroes
        teamManager.LoadTeam();
    }

    public void TestGame()
    {
        gameState = GameState.TOWN;

        TEST_MODE = true;

        TEST.gameObject.SetActive(true);
        SKIP_BUTTON.SetActive(true);

        unlockedPaths = true;

        tutorialManager.SkipTutorials = true;

        titleElements.gameObject.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        SetupTeamSelection();
        CreateTeam();     

        //// Create 3 Heroes
        //teamManager.CreateHero(0, 0);
        //teamManager.CreateHero(1, 1);
        //teamManager.CreateHero(2, 2);

        //townManager.StartTutorial();
        townManager.SetupStratford();
        townManager.dungeonList.SetDungeonList(true);
    }

    public void BackToTitle()
    {
        titleElements.gameObject.SetActive(true);
        newGameElements.gameObject.SetActive(false);

        startCamera.GetComponentInChildren<SpriteRenderer>().sprite = GameAssets.i.startBackground;
    }

    public void BattleWon()
    {
        //townManager.RotateShopDisplay();

        // Won last location -> Map is finished so return back to town
        if (dungeonManager.player.currentLocation.x == dungeonManager.gridHandler.columns - 1)
        {
            if (!unlockedPaths)
            {
                townManager.UnlockPaths(dungeonManager.currentDungeon, dungeonManager.currentFloor);

                NotificationObject.SendNotification("The Path system has been unlocked.");
            }

            if (dungeonManager.IsLastFloor())
            {
                townManager.dungeonList.UnlockDungeon(dungeonManager.currentDungeon);

                GoToTown();

                dungeonManager.EnableUI(false);

                RewardManager.Instance.SetupBattleResult();

                teamManager.heroes.FullRestoreTeam();
            }
            else
            {
                dungeonManager.NextFloor();

                RewardManager.Instance.SetupBattleResult();

                GoToRun();
            }
        }
        else
        {
            RewardManager.Instance.SetupBattleResult();

            GoToRun();
        }

        totalBattles++;
    }

    public void BattleLost()
    {
        teamManager.heroes.FullRestoreTeam();

        FirstDeath();

        GoToTown();

        RewardManager.Instance.SetupBattleResult();
    }
    
    private void FirstDeath()
    {
        if (firstDeath)
            return;

        firstDeath = true;

        firstDeathRewards.Reward(currencyHandler);
    }

    public void BattleFled()
    {
        GoToRun();

        dungeonManager.locationInformationObject.SetActive(true);

        dungeonManager.player.GetComponent<PlayerController>().movementLocked = true;
    }

    public void GoToTown()
    {
        gameState = GameState.TOWN;

        //teamManager.Regroup(true);

        currencyHandler.SetState(false);
        currencyHandler.UpdateCurrencies();

        cameraScript.GoToCamera(townManager.cameraObject, false);
        if (townManager.isTutorial) townManager.TutorialUI(true);
        else townManager.EnableUI(true);

        mapButton.UpdateButton(gameState);

        townManager.ShopButtonsState(true);
    }

    public void GoToRun()
    {
        gameState = GameState.RUN;

        //teamManager.Regroup(false);

        currencyHandler.SetState(true);
        currencyHandler.UpdateCurrencies();

        cameraScript.GoToCamera(dungeonManager.cameraObject, true);
        dungeonManager.EnableUI(true);

        mapButton.UpdateButton(gameState);

        townManager.ShopButtonsState(false);
    }

    public void TryAgain()
    {
        cameraScript.GoToCamera(startCamera, false);
        EnableUI(startUI);
        
        titleElements.gameObject.SetActive(true);
        newGameElements.gameObject.SetActive(false);

        ResetGame();
    }

    public void ResetGame()
    {
        teamManager.heroes.ResetTeam();
        teamManager.enemies.ResetTeam();

        totalBattles = 0;
        monstersDefeated = 0;

        // Reset Inventory
        // Reset Everything
    }
}
