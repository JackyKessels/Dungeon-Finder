using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Singleton
    public static SaveManager Instance { get; private set; }

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
    private ProgressionManager progressionManager;
    private InventoryManager inventoryManager;

    public string currentId;

    [SerializeField] GameObject saveSlotsWindow;
    [SerializeField] SaveSlot[] saveSlots = new SaveSlot[3];

    private void Start()
    {
        gameManager = GameManager.Instance;
        teamManager = TeamManager.Instance;
        progressionManager = ProgressionManager.Instance;
        inventoryManager = InventoryManager.Instance;
    }

    public void SaveGame()
    {
        SaveSystem.SaveTeamData(teamManager.heroes, currentId);

        //ShortMessage.SendMessage(Input.mousePosition, "Saved!", 24, Color.green);

        Debug.Log("Game Saved! - " + currentId + " - " + System.DateTime.Now);
    }

    public void LoadTeam(string id, TeamData data)
    {
        gameManager.SetupTown();

        //gameManager.optionsManager.LoadAmbientVolume();
        //gameManager.optionsManager.LoadSFXVolume();

        if (data == null)
            return;

        currentId = id;

        if (data.heroIndex_0 != -1)
        {
            Hero hero = teamManager.CreateHero(data.heroIndex_0, 0, data.equipment_0);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_0, data.abilityLevels_0);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_0);
            hero.spellbook.LearnPassives(data.learnedPassives_0);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_0);
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_0);

            hero.RestoreHealth();
        }

        if (data.heroIndex_1 != -1)
        {
            Hero hero = teamManager.CreateHero(data.heroIndex_1, 1, data.equipment_1);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_1, data.abilityLevels_1);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_1);
            hero.spellbook.LearnPassives(data.learnedPassives_1);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_1);
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_1);

            hero.RestoreHealth();
        }

        if (data.heroIndex_2 != -1)
        {
            Hero hero = teamManager.CreateHero(data.heroIndex_2, 2, data.equipment_2);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_2, data.abilityLevels_2);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_2);
            hero.spellbook.LearnPassives(data.learnedPassives_2);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_2);
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_2);

            hero.RestoreHealth();
        }

        teamManager.experienceManager.GainExperience(data.teamExperience);

        inventoryManager.inventoryObject.AddItemsToInventory(data.inventory);
        inventoryManager.consumablesObject.AddItemsToInventory(data.consumables);

        // Add path points
        if (data.heroIndex_0 != -1)
        {
            Hero hero = teamManager.heroes.Members[0] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_0);
        }
        if (data.heroIndex_1 != -1)
        {
            Hero hero = teamManager.heroes.Members[1] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_1);
        }
        if (data.heroIndex_2 != -1)
        {
            Hero hero = teamManager.heroes.Members[2] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_2);
        }

        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Gold, data.currency_0));
        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, data.currency_1));

        progressionManager.firstDeath = data.firstDeath;
        progressionManager.unlockedPaths = data.unlockedPaths;
        progressionManager.unlockedFourthAbility = data.unlockedFourthAbility;
        progressionManager.unlockedEnchanterUpgrade = data.unlockedEnchanterUpgrade;

        TownManager.Instance.SetupStratford();

        //if (data.isTutorial)
        //{
        //    townManager.StartTutorial();
        //}
        //else
        //{




        //    //townManager.blacksmith.LoadShop(data.itemShop, data.itemShopCosts, data.itemShopRotation);
        //    //townManager.trophyHunter.LoadShop(data.chapterShop);

        //    //townManager.UpdateRotationDisplay();
        //}



        //gameManager.interfaceBar.SetCorrectButton((GameState)data.gameState);
    }

    public void OpenSaveSlots()
    {
        saveSlotsWindow.SetActive(true);

        foreach (SaveSlot saveSlot in saveSlots)
        {
            if (saveSlot != null)
            {
                saveSlot.Setup();
            }
        }
    }
}
