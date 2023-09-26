using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour, IUserInterface
{
    #region Singleton
    public static DungeonManager Instance { get; private set; }

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
    [HideInInspector] public GridHandler gridHandler;

    public PlayerController player;

    [Header("Manager Variables")]
    public Canvas userInterface;
    public Camera cameraObject;

    [Header("Run Manager")]
    public GameObject mapObject;
    public SpriteRenderer dungeonBackground; 
    public GameObject playerPrefab;
    public GameObject objects;
    public GameObject locationInformationObject;
    public TextMeshProUGUI dungeonTitle;
    public TextMeshProUGUI floorName;
    public GameObject floorBackground;
    private LocationInformation locationInformation;
    [SerializeField] EventWindow eventWindow;

    public Button leaveDungeonButton;

    public EnemyDatabase enemyDatabase;

    [Header("Information")]
    public GameObject pathsContainer;
    public GameObject nameTagContainer;

    public Dungeon currentDungeon;
    public int currentFloor;

    private Vector2 initialSize;

    private void Start()
    {
        gameManager = GameManager.Instance;
        locationInformation = locationInformationObject.GetComponent<LocationInformation>();

        initialSize = dungeonBackground.size;

        gridHandler = GetComponent<GridHandler>();
    }

    public static int GetDungeonLevel(Floor floor)
    {
        switch (GameManager.Instance.gameMode)
        {
            case GameMode.Campaign:
                return floor.level;
            case GameMode.Endless:
                return ProgressionManager.Instance.endlessLevel;
            default:
                return 1;
        }
    }

    // Main function that triggers all the other functions
    public void BuildDungeon(Dungeon dungeon, Floor floor)
    {
        locationInformationObject.SetActive(false);

        // Remove player, locations and paths
        RemovePlayer();

        // Move camera to the appropriate map location
        Vector3 camPos = new Vector3(mapObject.transform.position.x, mapObject.transform.position.y, -10);
        cameraObject.transform.position = camPos;

        // Create player on the map
        CreatePlayer(gridHandler.startLocation);

        currentDungeon = dungeon;

        SetDungeonNames(floor);

        gridHandler.GenerateFloor(dungeon, floor);

        dungeonBackground.sprite = floor.dungeonBackground;
        dungeonBackground.size = new Vector2(initialSize.x + gridHandler.GetWidthIncrease(), initialSize.y);

        if (floor.battleBackground != null)
            EventManager.Instance.SetBackground(floor.battleBackground, true);
    }

    private void SetDungeonNames(Floor floor)
    {
        dungeonTitle.text = currentDungeon.name;
        dungeonTitle.color = currentDungeon.nameColor;

        floorName.text = $"{floor.name}{GetFloorString()}";
        floorName.color = currentDungeon.nameColor;

        if (floor.name == "")
            floorBackground.SetActive(false);
        else
            floorBackground.SetActive(true);
    }

    private string GetFloorString()
    {
        switch (gameManager.gameMode)
        {
            case GameMode.Campaign:
                return $" ({currentFloor + 1} / {currentDungeon.floors.Count})";
            case GameMode.Endless:
                return "";
            default:
                return "";
        }   
    }

    // Creates the movable player icon on the map
    private void CreatePlayer(Location initialLocation)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        player = playerObject.GetComponent<PlayerController>();
        player.transform.SetParent(objects.transform);
        player.SetCurrentLocation(initialLocation);
    }

    private void RemovePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
    }

    public void SetupLocationInformation(Location currentLocation)
    {
        locationInformationObject.SetActive(true);
        locationInformation.Setup(currentLocation);
        locationInformation.interactButton.onClick.RemoveAllListeners();
        if (!currentLocation.locked)
        {
            locationInformation.interactButton.onClick.AddListener(delegate { StartEvent(currentLocation); });
        }
    }

    // When the button is clicked -> trigger event depending on location
    private void StartEvent(Location location)
    {
        location.StartEvent(currentDungeon, currentFloor);
        ShowLocationInformation(false);
    }

    // Toggle location information
    public void ShowLocationInformation(bool show)
    {
        locationInformationObject.SetActive(show);
    }

    public void AbandonRunButton()
    {
        leaveDungeonButton.interactable = false;

        ConfirmationBoxHandler.Instance.SetupConfirmationBox("Leave Dungeon?", new List<ConfirmationButton>()
                                                                            { new ConfirmationButton("Yes", AbandonRun),
                                                                              new ConfirmationButton("No", ReactiveStartButton) });
    }

    private void ReactiveStartButton()
    {
        leaveDungeonButton.interactable = true;
    }

    private void AbandonRun()
    {
        leaveDungeonButton.interactable = true;

        gameManager.GoToTown();

        EnableUI(false);
    }

    public void NextFloor()
    {
        currentFloor++;

        TeamManager.Instance.heroes.FullRestoreTeam();

        if (currentFloor >= currentDungeon.floors.Count)
            currentFloor = 0;

        Floor floor = currentDungeon.floors[currentFloor];

        BuildDungeon(currentDungeon, floor);
    }

    public bool IsLastFloor()
    {
        if (currentFloor == currentDungeon.floors.Count - 1)
            return true;
        else
            return false;
    }

    public void EnableUI(bool show)
    {
        userInterface.gameObject.SetActive(show);
    }
}
