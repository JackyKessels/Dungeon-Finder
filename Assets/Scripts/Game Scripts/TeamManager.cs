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

    public UnitPosition[] heroPositions = new UnitPosition[3];
    public UnitPosition[] enemyPositions = new UnitPosition[3];

    private void Start()
    {
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
        townManager = TownManager.Instance;

        heroes = new Team(3);
        enemies = new Team(3);

        experienceManager = new ExperienceManager();
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

    public Hero CreateHero(int heroIndex, int index, (int, int)[] loadItemIDs = null)
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
            enemyPositions[u.battleNumber].shadow.SetActive(false);

            // Set team slot to null and add unit to some other list so rewards
            // can be extracted.
            enemies.KillEnemy(u);

            gameManager.monstersDefeated++;
        }
        else
        {      
            // Remove shadow
            heroPositions[u.battleNumber].shadow.SetActive(false);
        }
    }

    public void SetCurrentTurnVisual(Unit u, bool active)
    {
        if (u.isEnemy)
        {
            enemyPositions[u.battleNumber].currentTurn.SetActive(active);
        }
        else
        {
            heroPositions[u.battleNumber].currentTurn.SetActive(active);
        }
    }
}
