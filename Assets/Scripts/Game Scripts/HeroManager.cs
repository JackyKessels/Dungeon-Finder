using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroManager : MonoBehaviour, IUserInterface
{
    #region Singleton
    public static HeroManager Instance { get; private set; }

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
    private TeamManager teamManager;
    private InventoryManager inventoryManager;
    private SpellbookManager spellbookManager;

    [Header("[ Manager Specific ]")]
    public Canvas userInterface;
    public Camera cameraObject;

    [Header("[ Hero Information ]")]
    public TextMeshProUGUI teamName;
    public GameObject heroInformationObject;
    public TextMeshProUGUI currentHeroName;
    public Image heroIcon;

    [Header("[ Experience ]")]
    public Slider experienceBar;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI experienceText;

    [Header("[ Paths ]")]
    public GameObject starterPathObject;
    public GameObject starterPathContainer;
    public GameObject starterPathPrefab;
    public Button pathButton;
    public PathSystem pathSystem;

    private int nextHero = 0;

    private void Start()
    {
        gameManager = GameManager.Instance;
        teamManager = TeamManager.Instance;
        inventoryManager = InventoryManager.Instance;
        spellbookManager = SpellbookManager.Instance;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Q)) && gameManager.gameState != GameState.NONE)
        {
            OpenHeroInformation();
        }

        if (Input.GetKeyDown(KeyCode.A) && gameManager.gameState != GameState.NONE && heroInformationObject.activeSelf)
        {
            PreviousHero();
        }

        if (Input.GetKeyDown(KeyCode.D) && gameManager.gameState != GameState.NONE && heroInformationObject.activeSelf)
        {
            NextHero();
        }
    }

    public void SetTeamName(string name)
    {
        teamName.text = name;
    }

    public void Setup()
    {
        Hero hero = CurrentHero();

        // Set name
        currentHeroName.text = GeneralUtilities.GetFullUnitName(hero.heroObject);

        // Set icon
        heroIcon.sprite = hero.heroObject.icon;
        if (hero.statsManager.isDead)
            heroIcon.color = GeneralUtilities.ConvertString2Color("#505050");
        else
            heroIcon.color = Color.white;

        // Set experience bar
        UpdateCharacterExperience();

        // Set spellbook
        spellbookManager.Setup(hero);

        // Set equipment and inventory
        inventoryManager.Setup(hero);

        // Path System
        PathButtonStatus();
        pathSystem.ClosePathSystem();
        pathSystem.Setup(hero);
    }

    public void Refresh()
    {
        if (heroInformationObject.activeSelf)
            Setup();
    }

    // Updates the experience bar
    public void UpdateCharacterExperience()
    {
        int currentLevel = teamManager.experienceManager.currentLevel;

        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();

        float value = (float)teamManager.experienceManager.currentExperience / (float)ExperienceManager.ExperienceToNextLevel();

        experienceBar.value = value;

        experienceText.text = (float)teamManager.experienceManager.currentExperience + " / " + (float)ExperienceManager.ExperienceToNextLevel();
    }

    // Opens the interface
    public void OpenHeroInformation()
    {
        if (gameManager.gameState != GameState.BATTLE)
        {
            gameManager.audioSource.PlayOneShot(GameAssets.i.click);

            // If not open
            if (!heroInformationObject.activeSelf)
            {
                heroInformationObject.SetActive(true);

                Setup();
            }
            // If open
            else
            {
                heroInformationObject.SetActive(false);
                pathSystem.ClosePathSystem();
                TooltipHandler.Instance.HideTooltip();
            }
        }
    }

    // Shows the information of the next Hero
    public void NextHero()
    {
        if (nextHero < teamManager.heroes.Members.Count - 1)
        {
            nextHero++;
        }
        else
        {
            nextHero = 0;
        }

        Setup();
    }

    public void PreviousHero()
    {
        if (nextHero > 0)
        {
            nextHero--;
        }
        else
        {
            nextHero = teamManager.heroes.Members.Count - 1;
        }

        Setup();
    }

    public Hero CurrentHero()
    {
        return teamManager.heroes.Members[nextHero].GetComponent<Hero>();
    }

    public void SetCurrentHero(Unit hero)
    {
        for (int i = 0; i < teamManager.heroes.Members.Count; i++)
        {
            if (teamManager.heroes.Members[i] == hero)
            {
                nextHero = i;
            }
        }
    }

    private void PathButtonStatus()
    {
        if (CurrentHero().heroPathManager.unlocked)
        {
            pathButton.interactable = true;
            pathButton.GetComponentInChildren<TextMeshProUGUI>().text = "Paths";
        }
        else
        {
            pathButton.interactable = false;
            pathButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlocked at level " + ExperienceManager.pathPointLevelRequirement;
        }
    }

    public void EnableUI(bool show)
    {
        userInterface.gameObject.SetActive(show);
    }

    public void PickStarterPath(HeroPathObject heroPathObject, Hero hero)
    {
        foreach (Equipment equipment in heroPathObject.baseWeapons)
        {
            hero.ForceEquipItem(equipment);
        }

        //foreach (AbilityObject abilityObject in heroPathObject.baseAbilities)
        //{
        //    hero.spellbook.LearnAbility(new Active(abilityObject as ActiveAbility, 1));
        //}

        starterPathObject.SetActive(false);
    }

    public void SetupStarterPath(Hero hero)
    {
        starterPathObject.SetActive(true);

        ObjectUtilities.ClearContainer(starterPathContainer);

        foreach (HeroPath heroPath in hero.heroPathManager.paths)
        {
            AddStarterPath(heroPath.path, hero);
        }
    }

    private void AddStarterPath(HeroPathObject heroPathObject, Hero hero)
    {

        GameObject obj = ObjectUtilities.CreateSimplePrefab(starterPathPrefab, starterPathContainer);

        StarterPathGameObject starterPathGameObject = obj.GetComponent<StarterPathGameObject>();
        starterPathGameObject.Setup(heroPathObject, hero);
    }


}
