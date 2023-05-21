using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamData
{
    // Tutorial or Stratford
    public bool isTutorial;
    public int gameState;

    public bool firstDeath;
    public bool unlockedPaths;
    public bool unlockedFourthAbility;
    public bool unlockedEnchanterUpgrade;

    // The correct HeroObject
    public int heroIndex_0 = -1;
    public int heroIndex_1 = -1;
    public int heroIndex_2 = -1;

    // Hero experience earned
    public int teamExperience;

    // All ability IDs of the collection
    public List<int> abilityCollection_0;
    public List<int> abilityCollection_1;
    public List<int> abilityCollection_2;

    // The levels of the abilities in the collection
    public List<int> abilityLevels_0;
    public List<int> abilityLevels_1;
    public List<int> abilityLevels_2;

    // All passive IDs
    public List<(int, int)> learnedPassives_0;
    public List<(int, int)> learnedPassives_1;   
    public List<(int, int)> learnedPassives_2;

    // The IDs of the abilities that are active
    public int[] activeAbilities_0;
    public int[] activeAbilities_1;
    public int[] activeAbilities_2;

    // The levels of the Hero paths
    public List<int> pathLevels_0;
    public List<int> pathLevels_1;
    public List<int> pathLevels_2;

    // Total points left for Hero Path
    public int pathPoints_0;
    public int pathPoints_1;
    public int pathPoints_2;

    // Unlocked passives
    public List<bool[]> unlockedPassives_0;
    public List<bool[]> unlockedPassives_1;
    public List<bool[]> unlockedPassives_2;

    // The IDs of the items in the inventory
    public (int, int)[] inventory;

    // The IDs and stacks of the items in the consumables
    public (int, int)[] consumables;

    // The IDs of the items in the hero's equipment
    public (int, int)[] equipment_0;
    public (int, int)[] equipment_1;
    public (int, int)[] equipment_2;

    // Current health of the hero
    public int currentHealth_0;
    public int currentHealth_1;
    public int currentHealth_2;

    // Currencies
    public int currency_0;
    public int currency_1;

    // Stratford Shops
    public List<int> itemShop;
    public List<int> itemShopCosts;
    public int itemShopRotation;

    public List<int> abilityShop_0;
    public List<int> abilityShopCosts_0;
    public List<int> abilityShop_1;
    public List<int> abilityShopCosts_1;
    public List<int> abilityShop_2;
    public List<int> abilityShopCosts_2;
    public int abilityShopRotation;

    public List<int> chapterShop;
    public List<int> chapterShopCosts;
    public int chapterShopRotation;

    public TeamData(Team team)
    {
        GameManager gameManager = GameManager.Instance;
        InventoryManager inventoryManager = InventoryManager.Instance;
        TownManager townManager = TownManager.Instance;
        ProgressionManager progressionManager = ProgressionManager.Instance;

        isTutorial = townManager.isTutorial;
        gameState = (int)gameManager.gameState;

        firstDeath = progressionManager.firstDeath;
        unlockedPaths = progressionManager.unlockedPaths;
        unlockedFourthAbility = progressionManager.unlockedFourthAbility;
        unlockedEnchanterUpgrade = progressionManager.unlockedEnchanterUpgrade;

        Hero hero_0 = team.GetUnit(0) as Hero;
        Hero hero_1 = team.GetUnit(1) as Hero;
        Hero hero_2 = team.GetUnit(2) as Hero;

        if (hero_0 != null)
        {
            heroIndex_0 = hero_0.heroObjectIndex;

            abilityCollection_0 = hero_0.spellbook.ConvertCollectionToIDs();
            abilityLevels_0 = hero_0.spellbook.ConvertCollectionToLevels();
            activeAbilities_0 = hero_0.spellbook.ConvertActiveToIDs();

            pathLevels_0 = hero_0.heroPathManager.SavePathLevels();
            pathPoints_0 = hero_0.heroPathManager.points;
            unlockedPassives_0 = hero_0.heroPathManager.SavePathPassives();
            learnedPassives_0 = hero_0.spellbook.ConvertPassives();

            equipment_0 = inventoryManager.equipmentObjects[0].ConvertInventoryToIDs();

            currentHealth_0 = hero_0.statsManager.currentHealth;
        }

        if (hero_1 != null)
        {
            heroIndex_1 = hero_1.heroObjectIndex;

            abilityCollection_1 = hero_1.spellbook.ConvertCollectionToIDs();
            abilityLevels_1 = hero_1.spellbook.ConvertCollectionToLevels();
            activeAbilities_1 = hero_1.spellbook.ConvertActiveToIDs();

            pathLevels_1 = hero_1.heroPathManager.SavePathLevels();
            pathPoints_1 = hero_1.heroPathManager.points;
            unlockedPassives_1 = hero_1.heroPathManager.SavePathPassives();
            learnedPassives_1 = hero_1.spellbook.ConvertPassives();

            equipment_1 = inventoryManager.equipmentObjects[1].ConvertInventoryToIDs();

            currentHealth_1 = hero_1.statsManager.currentHealth;
        }

        if (hero_2 != null) 
        {
            heroIndex_2 = hero_2.heroObjectIndex;

            abilityCollection_2 = hero_2.spellbook.ConvertCollectionToIDs();
            abilityLevels_2 = hero_2.spellbook.ConvertCollectionToLevels();
            activeAbilities_2 = hero_2.spellbook.ConvertActiveToIDs();

            pathLevels_2 = hero_2.heroPathManager.SavePathLevels();
            pathPoints_2 = hero_2.heroPathManager.points;
            unlockedPassives_2 = hero_2.heroPathManager.SavePathPassives();
            learnedPassives_2 = hero_2.spellbook.ConvertPassives();

            equipment_2 = inventoryManager.equipmentObjects[2].ConvertInventoryToIDs();

            currentHealth_2 = hero_2.statsManager.currentHealth;
        }

        teamExperience = TeamManager.Instance.experienceManager.totalExperienceGained;

        inventory = inventoryManager.inventoryObject.ConvertInventoryToIDs();
        consumables = inventoryManager.consumablesObject.ConvertInventoryToIDs();

        currency_0 = gameManager.currencyHandler.GetAmount(CurrencyType.Gold);
        currency_1 = gameManager.currencyHandler.GetAmount(CurrencyType.Spirit);

        //itemShop = townManager.ConvertItemShopToIDs(townManager.blacksmith.currentShopItems);
        //itemShopCosts = townManager.ConvertItemShopToCosts(townManager.blacksmith.currentShopItems);
        //itemShopRotation = townManager.blacksmith.rotationCounter;

        //chapterShop = townManager.ConvertItemShopToIDs(townManager.trophyHunter.currentChapterAbilities);
        //chapterShopCosts = townManager.ConvertItemShopToCosts(townManager.trophyHunter.currentChapterAbilities);
    }
}
