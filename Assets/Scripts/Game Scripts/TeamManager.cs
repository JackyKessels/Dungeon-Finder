using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    #region Singleton
    public static TeamManager Instance { get; private set; }

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
    private InventoryManager inventoryManager;
    private TownManager townManager;

    [Header("[ Game Objects ]")]
    public GameObject heroesContainer;
    public GameObject heroesBench;
    public GameObject enemiesContainer;
    public GameObject enemiesDefeated;
    public GameObject heroPositionsContainer;
    public GameObject enemyPositionsContainer;

    [Header("[ Heroes Only ]")]
    public List<HeroObject> heroObjects;
    public ExperienceManager experienceManager;

    [Header("[ Teams ]")]
    //[HideInInspector] 
    public Team heroes;
    //[HideInInspector] 
    public Team enemies;

    public Transform[] heroPositions = new Transform[3];
    public Transform[] enemyPositions = new Transform[3];

    private void Start()
    {
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
        townManager = TownManager.Instance;

        heroes = new Team(3);
        enemies = new Team(3);

        experienceManager = new ExperienceManager();
    }

    public void SaveTeam()
    {
        SaveSystem.SaveTeam(heroes);

        Debug.Log("Game Saved!");
    }

    public void LoadTeam()
    {
        TeamData data = SaveSystem.LoadTeam();

        if (data.heroIndex_0 != -1)
        {
            Hero hero = CreateHero(data.heroIndex_0, 0, data.equipment_0);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_0, data.abilityLevels_0);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_0);
            hero.spellbook.LearnPassives(data.learnedPassives_0);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_0);
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_0);

            hero.statsManager.currentHealth = data.currentHealth_0;
        }
                                       
        if (data.heroIndex_1 != -1)
        {
            Hero hero = CreateHero(data.heroIndex_1, 1, data.equipment_1);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_1, data.abilityLevels_1);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_1);
            hero.spellbook.LearnPassives(data.learnedPassives_1);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_1);
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_1);

            hero.statsManager.currentHealth = data.currentHealth_1;
        }
            
        if (data.heroIndex_2 != -1)
        {
            Hero hero = CreateHero(data.heroIndex_2, 2, data.equipment_2);
            hero.newHero = false;

            hero.spellbook.AddAbilitiesToCollection(data.abilityCollection_2, data.abilityLevels_2);
            hero.spellbook.ChangeIDsToActive(data.activeAbilities_2);
            hero.spellbook.LearnPassives(data.learnedPassives_2);

            hero.heroPathManager.LoadPathLevels(data.pathLevels_2);     
            hero.heroPathManager.LoadPathPassives(data.unlockedPassives_2);

            hero.statsManager.currentHealth = data.currentHealth_2;
        }

        experienceManager.GainExperience(data.teamExperience);

        // Add path points
        if (data.heroIndex_0 != -1)
        {
            Hero hero = Instance.heroes.Members[0] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_0);
        }
        if (data.heroIndex_1 != -1)
        {
            Hero hero = Instance.heroes.Members[1] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_1);
        }
        if (data.heroIndex_2 != -1)
        {
            Hero hero = Instance.heroes.Members[2] as Hero;

            hero.heroPathManager.LoadPoints(data.pathPoints_2);
        }

        inventoryManager.inventoryObject.AddItemsToInventory(data.inventory);

        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Gold, data.currency_0));
        gameManager.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, data.currency_1));

        if (data.isTutorial)
        {
            townManager.StartTutorial();
        }
        else
        {
            townManager.SetupStratford();



            townManager.blacksmith.LoadShop(data.itemShop, data.itemShopCosts, data.itemShopRotation);
            townManager.trophyHunter.LoadShop(data.chapterShop);

            townManager.UpdateRotationDisplay();
        }

        //gameManager.interfaceBar.SetCorrectButton((GameState)data.gameState);
    }

    public void SetupBattle(List<EnemyObject> enemyObjects)
    {
        // Setup hero team
        heroes.Setup(heroPositions, false);

        // Create enemy objects
        for (int i = 0; i < enemyObjects.Count; i++)
        {
            if (enemyObjects[i] != null)
                CreateEnemy(enemyObjects[i], i);
        }

        // Setup enemy team
        enemies.Setup(enemyPositions, true);

        heroes.TriggerStartBattle();
        enemies.TriggerStartBattle();
    }

    public void TriggerRoundStartPassives()
    {
        heroes.TriggerRoundStart();
        enemies.TriggerRoundStart();
    }

    public void ApplyPreBattleEffects()
    {
        heroes.ApplyPreBattleEffects();
    }

    public Hero CreateHero(int heroIndex, int index, int[] loadItemIDs = null)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.heroPrefab, heroesContainer);
        obj.name = heroObjects[heroIndex].name;

        Hero hero = obj.GetComponent<Hero>();
        hero.UpdateUnit(heroIndex, index, loadItemIDs);

        heroes.AddUnit(hero, index);

        return hero;
    }

    public Enemy CreateEnemy(EnemyObject enemyObject, int position)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.enemyPrefab, enemiesContainer);
        obj.name = enemyObject.name;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.UpdateUnit(enemyObject);

        enemies.AddUnit(enemy, position);

        return enemy;
    }

    public void SpawnEnemy(EnemyObject enemyObject, bool instant, List<ParticleSystem> specialEffects)
    {
        int availablePosition = enemies.GetFirstEmptyPosition();

        // No empty position
        if (availablePosition == -1)
        {
            Debug.Log("No position available for spawn.");
            return;
        }

        Enemy enemy = CreateEnemy(enemyObject, availablePosition);
        GameObject enemyGameObject = enemy.gameObject;

        enemies.InitializeUnit(enemyPositions, true, availablePosition);

        StartCoroutine(enemy.FadeInAndOut(true, true, 1f));

        if (!instant)
        {
            Vector3 positionLocation = enemyGameObject.transform.position;
            Vector3 outOfScreenLocation = new Vector3(enemyGameObject.transform.position.x + 7, enemyGameObject.transform.position.y, enemyGameObject.transform.position.z);

            enemyGameObject.transform.position = outOfScreenLocation;

            StartCoroutine(enemy.MoveIntoBattle(enemyGameObject, positionLocation, 7f));
        }
        else
        {
            EffectManager.CreateSpecialEffect(enemy, specialEffects);
        }

        BattleHUD.Instance.Refresh();

        QueueManager.Instance.Refresh(false);
    }

    public void RewardExperienceToTeam(int amount)
    {
        experienceManager.GainExperience(amount, true);
    }

    public void ResetEnemy()
    {
        enemies.ResetTeam();

        foreach (Transform obj in enemiesContainer.transform)
        {
            Destroy(obj.gameObject);
        }
    }

    public void RemoveMember(Unit u)
    {
        u.statsManager.isDead = true;
        u.effectManager.ExpireAll();
        u.GetComponent<CapsuleCollider2D>().enabled = false;

        if (u.isEnemy)
        {
            // Remove shadow
            enemyPositions[u.battleNumber].GetChild(0).gameObject.SetActive(false);

            // Set team slot to null and add unit to some other list so rewards
            // can be extracted.
            enemies.KillEnemy(u);

            gameManager.monstersDefeated++;
        }
        else
        {      
            // Remove shadow
            heroPositions[u.battleNumber].GetChild(0).gameObject.SetActive(false);
        }
    }
}
