using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownManager : MonoBehaviour, IUserInterface
{
    #region Singleton
    public static TownManager Instance { get; private set; }

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

    private GameManager gameManager;
    private DungeonManager dungeonManager;
    private TeamManager teamManager;
    private CurrencyHandler currencyHandler;

    public AudioSource audioSource;

    [Header("[ Sound ]")]
    public AudioClip stratfordAmbient;
    public AudioClip openMap;
    public AudioClip closeMap;

    public Town tutorialTown;
    public Town stratford;

    [Header("[ Manager Variables ]")]
    public Canvas userInterface;
    public Camera cameraObject;

    [Header("[ Tutorial ]")]
    public Canvas tutorialInterface;
    public TextMeshProUGUI tutorialTitle;
    public Button tutorialRun;
    public Dungeon tutorialDungeon;
    public bool isTutorial;

    [Header("[ Town Variables ]")]
    public TextMeshProUGUI townName;
    public SpriteRenderer townBackground;

    [Header("[ Path Unlocking ]")]
    public Button pathsButton;
    public Dungeon pathUnlockDungeon;
    public int pathUnlockFloor;

    [Header("[ Map Variables ]")]
    public DungeonList dungeonList;
    public GameObject mapObject;
    public GameObject mapContainer;

    [Header("[ Button Variables ]")]
    public Button abilityShopButton;
    public Button itemShopButton;

    [Header("[ Shop Variables ]")]
    // Town Hall
    public TownHall townHall;
    // Item Shop
    public ItemShop blacksmith;
    // Ability Shop
    public AbilityShop enchanter;
    // Chapter Shop
    public ChapterShop trophyHunter;
    // General Goods Shop
    public ItemShop generalGoods;

    public GameObject shopItemPrefab;

    [SerializeField] private Dungeon lastFinishedChapter;

    private void Start()
    {
        gameManager = GameManager.Instance;
        dungeonManager = DungeonManager.Instance;
        teamManager = TeamManager.Instance;
        currencyHandler = gameManager.currencyHandler;

        audioSource = gameManager.audioSource;


    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W) && gameManager.gameState == GameState.TOWN)
        //{
        //    OpenMap();
        //}

        if (KeyboardHandler.OpenAbilityShop() && 
            gameManager.gameState == GameState.TOWN &&
            abilityShopButton.interactable)
        {
            OpenAbilityShop();
        }

        if (KeyboardHandler.OpenItemShop() && 
            gameManager.gameState == GameState.TOWN &&
            itemShopButton.interactable)
        {
            OpenItemShop();
        }

        if (KeyboardHandler.Escape())
        {
            CloseAllWindows();
        }
    }

    public void StartTutorial()
    {
        gameManager.gameState = GameState.TOWN;

        isTutorial = true;

        TutorialUI(true);

        tutorialTitle.text = tutorialTown.name;
        townBackground.sprite = tutorialTown.background;

        tutorialRun.onClick.AddListener(delegate { StartRun(tutorialDungeon); });
    }

    // Only runs once per game, after doing tutorial or on load.
    public void SetupStratford()
    {
        DamageMeterManager.Instance.InitializeDamageMeters();

        audioSource.FadeIn(stratfordAmbient, 0.5f);

        gameManager.gameState = GameState.TOWN;

        isTutorial = false;

        EnableUI(true);

        PathsState(gameManager.unlockedPaths);

        townName.text = stratford.name;
        townBackground.sprite = stratford.background;

        //townHall.Initialize();

        //BuildShops();

        currencyHandler.UpdateCurrencies();

        gameManager.mapButton.UpdateButton(GameState.TOWN);
    }

    public void StartRun(Dungeon dungeon)
    {
        if (dungeon.floors.Count == 0)
        {
            Debug.Log("Dungeon has no floors.");
            return;
        }

        audioSource.FadeOut(0.5f);

        if (dungeon.floors[0].backgroundSound != null)
        {
            audioSource.clip = dungeon.floors[0].backgroundSound;
            audioSource.Play();
        }

        TooltipHandler.Instance.HideTooltip();

        mapObject.SetActive(false);

        dungeonManager.StartDungeon(dungeon);

        gameManager.GoToRun();

        dungeonManager.EnableUI(true);
        this.EnableUI(false);

        if (isTutorial) TutorialUI(false);
    }

    public void SetupMap()
    {
        mapObject.gameObject.SetActive(true);

        dungeonList.AddDungeonsToMap(mapContainer);
    }

    public void BuildShops()
    {
        //UpdateRotationDisplay();

        //blacksmith.BuildShop(teamManager.experienceManager.currentLevel);
        //enchanter.BuildShop();
        //trophyHunter.BuildShop(lastFinishedChapter.dungeonAbilities);
        generalGoods.BuildShop(teamManager.experienceManager.currentLevel);
    }

    public void RotateShopDisplay()
    {
        if (isTutorial)
            return;

        blacksmith.RotateShopDisplay(teamManager.experienceManager.currentLevel);
    }

    public void UpdateRotationDisplay()
    {
        blacksmith.UpdateRotationDisplay();
    }

    public void OpenTownHall()
    {
        OpenShopWindow(townHall);
    }

    public void OpenItemShop()
    {
        OpenShopWindow(blacksmith);
    }

    public void OpenAbilityShop()
    {
        OpenShopWindow(enchanter);
    }

    public void OpenChapterShop()
    {
        OpenShopWindow(trophyHunter);
    }

    public void OpenGeneralGoodsShop()
    {
        OpenShopWindow(generalGoods);
    }

    private void OpenShopWindow(Shop shop)
    {
        if (!shop.unlocked)
            return;

        if (!shop.ActiveSelf())
        {
            gameManager.audioSource.PlayOneShot(GameAssets.i.coins);

            gameManager.tutorialManager.ShowLearningAbilities();

            CloseAllWindows();

            shop.SetActive(true);

            shop.SetupShop();
        }
        else
        {
            gameManager.audioSource.PlayOneShot(GameAssets.i.click);

            shop.SetActive(false);
        }
    }

    public void OpenMap()
    {
        if (!mapObject.activeSelf)
        {
            CloseAllWindows();

            mapObject.SetActive(true);

            if (HeroManager.Instance.heroInformationObject.activeSelf)
                HeroManager.Instance.heroInformationObject.SetActive(false);

            SetupMap();

            audioSource.PlayOneShot(openMap, 0.75f);
        }
        else
        {
            mapObject.SetActive(false);

            audioSource.PlayOneShot(closeMap, 0.75f);
        }
    }

    public void ShopButtonsState(bool state)
    {
        abilityShopButton.interactable = state;
        itemShopButton.interactable = state;
    }



    private void CloseAllWindows()
    {
        mapObject.SetActive(false);
        townHall.SetActive(false);
        blacksmith.SetActive(false);
        enchanter.SetActive(false);
        trophyHunter.SetActive(false);
        generalGoods.SetActive(false);

        TooltipHandler.Instance.HideTooltip();
    }

    public void EnableUI(bool show)
    {
        userInterface.gameObject.SetActive(show);
    }

    public void TutorialUI(bool show)
    {
        tutorialInterface.gameObject.SetActive(show);
    }

    public void SKIP_TO_TOWN()
    {
        SetupStratford();

        TutorialUI(false);

        gameManager.mapButton.UpdateButton(GameState.TOWN);

        //HeroManager.Instance.SetupStarterPath(teamManager.heroes.members[0] as Hero);
    }

    public void UnlockPaths(Dungeon dungeon, int floor)
    {
        if (pathUnlockDungeon == dungeon && pathUnlockFloor == floor)
        {
            PathsState(true);
        }
    }

    public void PathsState(bool unlocked)
    {
        TooltipObject tooltip = pathsButton.GetComponent<TooltipObject>();

        if (unlocked)
        {
            gameManager.unlockedPaths = true;

            pathsButton.interactable = true;

            Destroy(tooltip);

            Debug.Log("Unlocked Paths");
        }
        else
        {
            pathsButton.interactable = false;

            string color = ColorDatabase.GeneralInformation();

            tooltip.useGenericTooltip = true;
            tooltip.genericTooltip = string.Format("Unlocks when <color={0}>Si'keth Bloodclaw</color> has been defeated.\n\nHe can be found in the <color={0}>Crimson Burrows</color>.", color);
        }
    }




    // Save & Load

    public List<int> ConvertItemShopToIDs(List<CurrentShopItem> currentShopItems)
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < currentShopItems.Count; i++)
        {
            newList.Add(currentShopItems[i].GetRewardID());
        }

        return newList;
    }

    public List<int> ConvertItemShopToCosts(List<CurrentShopItem> currentShopItems)
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < currentShopItems.Count; i++)
        {
            newList.Add(currentShopItems[i].cost.totalAmount);
        }

        return newList;
    }
}
