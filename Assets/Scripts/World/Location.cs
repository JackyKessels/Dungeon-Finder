using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum LocationType
{
    Start,
    Empty,
    Battle,
    Elite,
    Boss,
    Treasure,
    Campfire,
    Spirit,
    Mystery,
}

public class Location : MonoBehaviour, IDescribable
{
    private GameManager gameManager;
    private DungeonManager dungeonManager;
    private EventManager eventManager;
    private TooltipHandler tooltipHandler;
    private Pathfinder pathfinder;
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite spriteEmpty;
    public Sprite spriteBattle;
    public Sprite spriteElite;
    public Sprite spriteBoss;
    public Sprite spriteTreasure;
    public Sprite spriteCampfire;
    public Sprite spriteSpirit;
    public Sprite spriteMystery;

    [Header("[ Location Specific ]")]
    public new string name;
    public Sprite locationImage;
    public List<Sprite> locationBackgrounds;
    [TextArea(3, 3)] public string description;
    public LocationType locationType = LocationType.Empty;
    public bool locked = false;
    public List<Location> connectedLocations;
    public List<EnemyObject> enemyUnits = new List<EnemyObject>();

    // Pathfinder Data
    [HideInInspector] public float gCost;
    [HideInInspector] public float hCost;
    [HideInInspector] public float fCost;
    [HideInInspector] public Location cameFromLocation;

    [HideInInspector] public int x;
    [HideInInspector] public int y;

    private readonly float campfireHeal = 0.5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        dungeonManager = DungeonManager.Instance;
        eventManager = EventManager.Instance;
        tooltipHandler = TooltipHandler.Instance;
        pathfinder = new Pathfinder();
}

    public void SetLocationType(LocationType type)
    {
        name = type.ToString();
        locationType = type;

        switch (type)
        {
            case LocationType.Empty:
                {
                    spriteRenderer.sprite = spriteEmpty; 
                    break;
                }
            case LocationType.Battle:
                {
                    spriteRenderer.sprite = spriteBattle;
                    break;
                }
            case LocationType.Elite:
                {
                    spriteRenderer.sprite = spriteElite;
                    break;
                }
            case LocationType.Boss:
                {
                    spriteRenderer.sprite = spriteBoss;
                    break;
                }
            case LocationType.Treasure:
                {
                    spriteRenderer.sprite = spriteTreasure;
                    break;
                }
            case LocationType.Campfire:
                {
                    spriteRenderer.sprite = spriteCampfire;
                    break;
                }
            case LocationType.Spirit:
                {
                    spriteRenderer.sprite = spriteSpirit;
                    break;
                }
            case LocationType.Mystery:
                {
                    spriteRenderer.sprite = spriteMystery;
                    break;
                }
            default:
                break;
        }
    }

    public void StartEvent(Dungeon dungeon, int floor)
    {
        dungeonManager.locationInformationObject.SetActive(false);

        switch (locationType)
        {
            case LocationType.Empty:
                break;
            case LocationType.Battle:
                {
                    eventManager.SetBackground(RandomBackground(), true);
                    BattleManager.Instance.StartBattle(enemyUnits);
                    break;
                }
            case LocationType.Elite:
                {
                    eventManager.SetBackground(RandomBackground(), true);
                    BattleManager.Instance.StartBattle(enemyUnits);
                    break;
                }
            case LocationType.Boss:
                {
                    eventManager.SetBackground(RandomBackground(), true);
                    BattleManager.Instance.StartBattle(enemyUnits);
                    break;
                }
            case LocationType.Treasure:
                {
                    RewardManager.Instance.GenerateLootTable(false, dungeon.floors[floor].itemPool, 3, 3);
                    break;
                }
            case LocationType.Campfire:
                {
                    TeamManager.Instance.heroes.ReviveDeadMembers(false);
                    TeamManager.Instance.heroes.HealTeam(campfireHeal, true);

                    //string percentage = (campfireHeal * 100).ToString();

                    //NotificationObject.SendNotification("Your team has restored " + percentage + "% of their missing Health, and all dead members are revived");
                    break;
                }
            case LocationType.Spirit:
                {
                    foreach (Unit unit in TeamManager.Instance.heroes.Members)
                    {
                        ActiveAbility abilityReward = RewardManager.Instance.GenerateRandomAbility(unit, RewardManager.Instance.database.abilityRewards);
                        NotificationObject.SendNotification(unit.name + " has learned " + abilityReward.name + ".", new List<Reward>() { new Reward(abilityReward) });
                    }
                    break;
                }
            case LocationType.Mystery:
                {
                    {
                        DungeonManager.Instance.OpenEventWindow(dungeon.floors[floor].mysteryEvents.GetRandomEvent());
                    }
                    break;
                }
            default:
                break;
        }

        dungeonManager.player.movementLocked = false;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlayerController player = dungeonManager.player.GetComponent<PlayerController>();

            if (connectedLocations.Contains(player.currentLocation) && !locked && !player.movementLocked)
            {
                if (!player.isMoving)
                {
                    player.MoveToLocation(this);
                }
            }
        }
    }

    public void OnMouseEnter()
    {
        Vector3 cameraCoords = dungeonManager.cameraObject.WorldToScreenPoint(transform.position);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            tooltipHandler.ShowTooltip(GetDescription(null), cameraCoords);
        }
    }

    public void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            tooltipHandler.HideTooltip();
        }
    }

    // Tooltip
    public string GetDescription(TooltipObject tooltipInfo)
    {
        return LocationName() + "\n" + TypeDescription() + PreviewEnemies();
    }

    private string PreviewEnemies()
    {
        string enemy1 = "";
        string enemy2 = "";
        string enemy3 = "";

        if (enemyUnits.Count > 0)
            enemy1 = "\n" + enemyUnits[0].name;
        if (enemyUnits.Count > 1)
            enemy2 = "\n" + enemyUnits[1].name;
        if (enemyUnits.Count > 2)
            enemy3 = "\n" + enemyUnits[2].name;

        return string.Format("\n{0}{1}{2}", enemy1, enemy2, enemy3);
    }

    private string LockedString()
    {
        if (locked)
            return "<color=#FF0000>Locked</color>";
        else
            return "<color=#00FF00>Open</color>";
    }

    private string LocationName()
    {
        string colorName = "";

        switch (locationType)
        {
            case LocationType.Start:
                colorName = "#80FF33";
                break;
            case LocationType.Empty:
                break;
            case LocationType.Battle:
                colorName = "#FF2300";
                break;
            case LocationType.Elite:
                colorName = "#FF2300";
                break;
            case LocationType.Boss:
                colorName = "#FF2300";
                break;
            case LocationType.Treasure:
                colorName = "#80FF33";
                break;
            case LocationType.Campfire:
                colorName = "#80FF33";
                break;
            case LocationType.Spirit:
                colorName = "#80FF33";
                break;
            case LocationType.Mystery:
                colorName = "#FF8C00";
                break;
            default:
                colorName = "#FFFFFF";
                break;
        }

        return string.Format("<b><color={0}>{1}</color></b>", colorName, locationType);
    }

    public string TypeDescription()
    {
        string colorDescription = "#FFFFFF";

        string text;

        switch (locationType)
        {
            case LocationType.Start:
                text = "This is the starting location.";
                break;
            case LocationType.Empty:
                text = "This is an empty location.";
                break;
            case LocationType.Battle:
                text = "Engage in combat with the following enemies:";
                break;
            case LocationType.Elite:
                text = "Engage in combat with the following enemies:";
                break;
            case LocationType.Boss:
                text = "Defeat the following boss to complete this chapter:";
                break;
            case LocationType.Treasure:
                text = "Your party stumbles upon items of value." +
                       "\n\n- Choose from a random array of items.";
                break;
            case LocationType.Campfire:
                string percentage = (campfireHeal * 100).ToString();
                text = "Your party has a moment of respite." +
                       "\n\n- Dead members are revived and everyone restores " + percentage + "% of their missing Health.";
                break;
            case LocationType.Spirit:
                text = "Each member gets a random ability.";
                break;
            case LocationType.Mystery:
                text = "Something of note happens to your party." +
                       "\n\n- A random event occurs, it could be positive or negative.";
                break;
            default:
                text = "";
                break;
        }

        return string.Format("<color={0}>{1}</color>", colorDescription, text);
    }

    // Pathfinding
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    private Sprite RandomBackground()
    {
        int totalBackgrounds = locationBackgrounds.Count;

        if (totalBackgrounds == 0)
            return null;

        int randomIndex = Random.Range(0, totalBackgrounds);

        return locationBackgrounds[randomIndex];
    }

    public void LockLocation(bool setLocked)
    {
        locked = setLocked;

        if (setLocked)
        {
            spriteRenderer.color = Color.gray;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public string GetLocationString()
    {
        return "Location[" + x + ", " + y + "]";
    }

    public void SetVisited()
    {
        locked = true;

        spriteRenderer.color = Color.green;
    }

    public List<Location> GetLeftConnectedLocations()
    {
        List<Location> leftConnectedLocations = new List<Location>();

        foreach (Location location in connectedLocations)
        {
            if (location.x < x)
            {
                leftConnectedLocations.Add(location);
            }
        }

        return leftConnectedLocations;
    }

    public List<Location> GetRightConnectedLocations()
    {
        List<Location> leftConnectedLocations = new List<Location>();

        foreach (Location location in connectedLocations)
        {
            if (location.x > x)
            {
                leftConnectedLocations.Add(location);
            }
        }

        return leftConnectedLocations;
    }
}
