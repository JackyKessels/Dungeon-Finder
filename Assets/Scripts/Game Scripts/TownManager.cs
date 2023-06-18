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
    public GameObject mapObject;
    public GameObject mapContainer;

    [Header("[ Shop Variables ]")]
    // Ability Shop
    public AbilityShop enchanter;
    // Glyph Shop
    public GlyphShop glyphweaver;


    public GameObject shopItemPrefab;

    [SerializeField] private Dungeon lastFinishedChapter;

    private void Start()
    {
        gameManager = GameManager.Instance;
        dungeonManager = DungeonManager.Instance;
        teamManager = TeamManager.Instance;
        currencyHandler = gameManager.currencyHandler;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W) && gameManager.gameState == GameState.TOWN)
        //{
        //    OpenMap();
        //}

        if (KeyboardHandler.OpenAbilityShop() && 
            gameManager.gameState == GameState.TOWN &&
            enchanter.ActiveButton())
        {
            OpenAbilityShop();
        }

        if (KeyboardHandler.OpenGlyphShop() && 
            gameManager.gameState == GameState.TOWN &&
            glyphweaver.ActiveButton())
        {
            OpenGlyphShop();
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

        gameManager.audioSourceAmbient.FadeIn(stratfordAmbient, 0.5f);

        gameManager.gameState = GameState.TOWN;

        isTutorial = false;

        EnableUI(true);

        ProgressionManager.Instance.SetPathButtonState();

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

        gameManager.audioSourceAmbient.FadeOut(0.5f);

        if (dungeon.floors[0].backgroundSound != null)
        {
            gameManager.audioSourceAmbient.clip = dungeon.floors[0].backgroundSound;
            gameManager.audioSourceAmbient.Play();
        }
        else
        {
            gameManager.audioSourceAmbient.Stop();
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

        ProgressionManager.Instance.dungeonList.AddDungeonsToMap(mapContainer);
    }

    public void OpenAbilityShop()
    {
        OpenShopWindow(enchanter);
    }
    
    public void OpenGlyphShop()
    {
        OpenShopWindow(glyphweaver);
    }

    private void OpenShopWindow(Shop shop)
    {
        if (!shop.unlocked)
            return;

        if (!shop.ActiveSelf())
        {
            gameManager.audioSourceSFX.PlayOneShot(GameAssets.i.coins);

            if (shop is AbilityShop)
                gameManager.tutorialManager.TutorialLearningAbilities();

            if (shop is GlyphShop)
            {
                if (HeroManager.Instance.heroInformationObject.activeSelf)
                    HeroManager.Instance.heroInformationObject.SetActive(false);
            }

            CloseAllWindows();

            shop.SetActive(true);

            shop.SetupShop();
        }
        else
        {
            gameManager.audioSourceSFX.PlayOneShot(GameAssets.i.click);

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

            gameManager.audioSourceSFX.PlayOneShot(openMap, 0.75f);
        }
        else
        {
            mapObject.SetActive(false);

            gameManager.audioSourceSFX.PlayOneShot(closeMap, 0.75f);
        }
    }

    public void ShopButtonsState(bool state)
    {
        enchanter.SetButton(state);
        glyphweaver.SetButton(state);
    }

    private void CloseAllWindows()
    {
        mapObject.SetActive(false);

        enchanter.SetActive(false);
        glyphweaver.SetActive(false);

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
