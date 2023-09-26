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

    [Header("[ Manager Variables ]")]
    public GameObject campaignInterface;
    public GameObject endlessInterface;
    public Camera cameraObject;

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
            gameManager.gameMode == GameMode.Campaign &&
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

    public void SetupTown(Town town)
    {
        DamageMeterManager.Instance.InitializeDamageMeters();

        gameManager.audioSourceAmbient.FadeIn(stratfordAmbient, 0.5f);

        gameManager.gameState = GameState.TOWN;

        EnableUI(true);

        ProgressionManager.Instance.SetPathButtonState();

        GlyphManager.Instance.UnlockStarterGlyphs();

        townName.text = town.name;
        townBackground.sprite = town.background;


        currencyHandler.UpdateCurrencies();

        gameManager.mapButton.UpdateButton(GameState.TOWN);
    }

    public void StartDungeon(Dungeon dungeon)
    {
        if (dungeon == null)
        {
            Debug.Log("Dungeon is null.");
            return;
        }

        if (dungeon.floors.Count == 0)
        {
            Debug.Log("Dungeon has no floors.");
            return;
        }

        gameManager.audioSourceAmbient.FadeOut(0.5f);

        TooltipHandler.Instance.HideTooltip();

        Floor floor = null;

        switch (gameManager.gameMode)
        {
            case GameMode.Campaign:
                {
                    floor = dungeon.floors[0];

                    mapObject.SetActive(false);
                }
                break;
            case GameMode.Endless:
                {
                    floor = ProgressionManager.Instance.endlessManager.NextFloor();
                }
                break;
        }

        if (floor == null)
        {
            Debug.Log("Floor not generated properly.");
            return;
        }

        if (floor.backgroundSound != null)
        {
            gameManager.audioSourceAmbient.clip = dungeon.floors[0].backgroundSound;
            gameManager.audioSourceAmbient.Play();
        }
        else
        {
            gameManager.audioSourceAmbient.Stop();
        }

        dungeonManager.BuildDungeon(dungeon, floor);

        gameManager.GoToRun();

        dungeonManager.EnableUI(true);
    }

    public void SetupMap()
    {
        mapObject.SetActive(true);

        ProgressionManager.Instance.campaignManager.AddDungeonsToMap(mapContainer);
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
        switch (gameManager.gameMode)
        {
            case GameMode.Campaign:
                campaignInterface.SetActive(show);
                break;
            case GameMode.Endless:
                endlessInterface.SetActive(show);
                break;
            default:
                break;
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
