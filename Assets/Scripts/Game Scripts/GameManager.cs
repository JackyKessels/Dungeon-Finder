using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public enum GameState
{
    NONE,
    TOWN,
    RUN,
    BATTLE,
    START_SCREEN
}

public enum GameMode
{
    Campaign,
    Endless
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
    private ProgressionManager progressionManager;

    [HideInInspector] public CurrencyHandler currencyHandler;

    public AudioSource audioSourceAmbient;
    public AudioSource audioSourceSFX;

    public GameState gameState;
    public GameMode gameMode;

    [Header("TESTING STUFF")]
    public bool TEST_MODE = false;
    public Canvas TEST;
    public GameObject SKIP_BUTTON;
    public Unit SFX_TARGET;

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

    [Header("[ Select Mode ]")]
    [SerializeField] GameObject selectModeWindow;
    [SerializeField] Town campaignTown;
    [SerializeField] Town endlessTown;

    [Header("[ Story ]")]
    public HeroObject startHero;
    public HeroObject SECOND_TEST_HERO;
    public HeroObject THIRD_TEST_HERO;
    public Town startTown;

    [Header("[ Main Systems ]")]
    public CameraScript cameraScript;
    public EnemyDatabase enemyDatabase;
    public MapButton mapButton;
    public OptionsManager optionsManager;

    [Header("[ Inventory ]")]
    public GameObject inventoryContainer;
    public InventoryObject inventory;

    private Canvas currentUI;

    private Transform titleElements;
    private Transform newGameElements;

    private HeroSelectionObject heroSelectionObject_0;
    private HeroSelectionObject heroSelectionObject_1;
    private HeroSelectionObject heroSelectionObject_2;

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
        progressionManager = ProgressionManager.Instance;

        journeyManager = new JourneyManager(endUI);
        currencyHandler = GetComponent<CurrencyHandler>();

        audioSourceAmbient = GetComponent<AudioSource>();

        titleElements = startUI.transform.GetChild(0);
        newGameElements = startUI.transform.GetChild(1);

        //if (SaveSystem.LoadTeam() == null)
        //    loadButton.interactable = false;
    }

    public void LoadParticleSystems()
    {
        var guids = AssetDatabase.FindAssets("", new string[] { "Assets/Prefabs/Particle Effects" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            ParticleSystem specialEffect = AssetDatabase.LoadAssetAtPath<ParticleSystem>(path);
            if (specialEffect != null)
            {
                ObjectUtilities.CreateSpecialEffects(new() { specialEffect }, SFX_TARGET);
                Debug.Log($"Loaded: {specialEffect.name}");
            }
        }
    }

    public void EnableUI(Canvas ui)
    {
        currentUI.gameObject.SetActive(false);

        ui.gameObject.SetActive(true);
        currentUI = ui;
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

    public void StartNewGame()
    {
        Introduction1();
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

    public void ChooseTeam()
    {
        ObjectUtilities.BlackTransition(true);

        titleElements.gameObject.SetActive(false);

        teamNameWindow.SetActive(true);

        SetupTeamSelection();
        heroManager.SetTeamName(teamName);
    }

    public void SelectMode()
    {
        ObjectUtilities.BlackTransition(true);

        teamNameWindow.SetActive(false);

        selectModeWindow.SetActive(true);
    }

    // Start Story
    public void StartCampaign()
    {
        LoadParticleSystems();

        ObjectUtilities.BlackTransition(true);
        
        gameState = GameState.TOWN;

        gameMode = GameMode.Campaign;

        selectModeWindow.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        CreateTeam();

        progressionManager.ResetProgression();

        townManager.SetupTown(campaignTown);

        StartCoroutine(StartIntroductionDialogue());

        SaveManager.Instance.SaveGame();
    }

    public void StartEndless()
    {
        LoadParticleSystems();

        ObjectUtilities.BlackTransition(true);

        gameState = GameState.TOWN;

        gameMode = GameMode.Endless;

        selectModeWindow.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        CreateTeam();

        progressionManager.ResetProgression();

        townManager.SetupTown(endlessTown);

        SaveManager.Instance.SaveGame();
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

    public void SetupTown()
    {
        gameState = GameState.TOWN;

        titleElements.gameObject.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);
    }

    public void TestGame()
    {
        SaveManager.Instance.currentId = "3";

        gameState = GameState.TOWN;

        TEST_MODE = true;

        TEST.gameObject.SetActive(true);
        SKIP_BUTTON.SetActive(true);

        progressionManager.campaignManager.SetDungeonList(true);

        progressionManager.firstDeath = true;
        progressionManager.unlockedPaths = true;
        progressionManager.SetPathButtonState();
        progressionManager.UnlockFourthAbility();
        progressionManager.UnlockEnchanterUpgrade();

        tutorialManager.SkipTutorials = true;

        titleElements.gameObject.SetActive(false);

        cameraScript.GoToCamera(townManager.cameraObject, false);

        heroManager.EnableUI(true);

        SetupTeamSelection();
        CreateTeam();     

        townManager.SetupTown(campaignTown);
    }

    public void BackToTitle()
    {
        titleElements.gameObject.SetActive(true);
        newGameElements.gameObject.SetActive(false);

        startCamera.GetComponentInChildren<SpriteRenderer>().sprite = GameAssets.i.startBackground;
    }

    public void BattleWon()
    {
        progressionManager.totalVictories++;

        // Won last location -> Map is finished so return back to town
        if (dungeonManager.player.currentLocation.x == dungeonManager.gridHandler.columns - 1)
        {
            switch (gameMode)
            {
                case GameMode.Campaign:
                    {
                        progressionManager.UnlockPaths(dungeonManager.currentDungeon, dungeonManager.currentFloor);

                        if (dungeonManager.IsLastFloor())
                        {
                            progressionManager.totalBossesDefeated++;

                            progressionManager.campaignManager.UnlockDungeon(dungeonManager.currentDungeon);
                            progressionManager.UnlockFourthAbility();
                            progressionManager.UnlockEnchanterUpgrade();

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
                    break;
                case GameMode.Endless:
                    {
                        GoToTown();

                        dungeonManager.EnableUI(false);

                        RewardManager.Instance.SetupBattleResult();

                        teamManager.heroes.FullRestoreTeam();

                        progressionManager.endlessManager.EndlessLevel++;
                    }
                    break;
            }
        }
        else
        {
            RewardManager.Instance.SetupBattleResult();

            GoToRun();
        }
    }

    public Town GetTown(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Campaign:
                return campaignTown;
            case GameMode.Endless:
                return endlessTown;
            default:
                return null;
        }
    }

    public void BattleLost()
    {
        progressionManager.totalDefeats++;

        progressionManager.FirstDeath();

        GoToTown();

        RewardManager.Instance.SetupBattleResult();
    }


    public void BattleFled()
    {
        GoToRun();

        dungeonManager.locationInformationObject.SetActive(true);

        dungeonManager.player.GetComponent<PlayerController>().movementLocked = true;
    }

    public void GoToTown()
    {
        teamManager.heroes.FullRestoreTeam();

        gameState = GameState.TOWN;

        currencyHandler.SetState(false);
        currencyHandler.UpdateCurrencies();

        cameraScript.GoToCamera(townManager.cameraObject, false);

        townManager.EnableUI(true);

        mapButton.UpdateButton(gameState);

        townManager.ShopButtonsState(true);

        SaveManager.Instance.SaveGame();
    }

    public void GoToRun()
    {
        gameState = GameState.RUN;

        cameraScript.GoToCamera(dungeonManager.cameraObject, true);
        dungeonManager.EnableUI(true);

        switch (gameMode)
        {
            case GameMode.Campaign:
                {
                    currencyHandler.SetState(true);
                    currencyHandler.UpdateCurrencies();

                    mapButton.UpdateButton(gameState);

                    townManager.ShopButtonsState(false);
                }
                break;
            case GameMode.Endless:
                {

                }
                break;
        }

        SaveManager.Instance.SaveGame();
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

        progressionManager.ResetProgression();
    }
}
