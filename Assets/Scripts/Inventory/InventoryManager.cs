using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public delegate void ItemEvent(ItemObject itemObject);

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance { get; private set; }

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

    private TeamManager teamManager;
    private CurrencyHandler currencyHandler;

    [Header("Equipment")]
    public InventoryObject[] equipmentObjects = new InventoryObject[3];
    public StaticInterface equipment;

    [Header("Attributes")]
    public GameObject attributeContainer;
    public GameObject attributePrefab;
    public Button attributeToggle;
    public int attributeMenuIndex;

    [Header("Inventory")]
    public GameObject inventoryParent;
    public GameObject inventoryPrefab;
    public InventoryObject inventoryObject;
    public InventoryObject consumablesObject;
    public DynamicInterface inventory;
    public DynamicInterface consumables;

    public ItemEvent OnItemObtained;

    private void Start()
    {
        ResetInventories();

        teamManager = TeamManager.Instance;
        currencyHandler = GameManager.Instance.currencyHandler;

        OnItemObtained += TownManager.Instance.codex.DiscoverItem;

        // Initialize the Inventory, the Consumable slots and the Equipment slots
        inventory.Setup(inventoryObject);
        consumables.Setup(consumablesObject);
        for (int i = 0; i < 3; i++)
        {
            equipment.Setup(equipmentObjects[i]);
            equipmentObjects[i].ResetSlotRestrictions();
        }
    }

    public void Setup(Hero hero)
    {
        attributeMenuIndex = 0;

        UpdateCharacterAttributes(hero, attributeMenuIndex);

        equipment.Setup(equipmentObjects[hero.teamNumber]);
        equipment.UpdateItems();

        currencyHandler.UpdateCurrencies();
    }

    public void UpdateCharacterAttributes(Hero hero, int menuIndex)
    {
        ObjectUtilities.ClearContainer(attributeContainer);

        // Show the current menu
        if (menuIndex == -1)
        {
            menuIndex = attributeMenuIndex;
        }
        // Show general attributes
        if (menuIndex == 0)
        {
            for (int i = 0; i < 9; i++)
            {
                CreateCharacterAttribute(hero.statsManager.GetAttributes()[i], hero);
            }
        }
        // Show school multipliers
        else if (menuIndex == 1)
        {
            for (int i = 9; i < 19; i++)
            {
                CreateCharacterAttribute(hero.statsManager.GetAttributes()[i], hero);
            }
        }
    }

    public void CreateCharacterAttribute(Attribute attribute, Hero hero)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(attributePrefab, attributeContainer);

        CharacterAttribute characterAttribute = obj.GetComponent<CharacterAttribute>();
        characterAttribute.Setup(attribute, hero);
    }

    public void IncreaseAttributeIndex()
    {
        attributeMenuIndex++;

        // Currently only have 2 menus (0, 1)
        if (attributeMenuIndex > 1)
            attributeMenuIndex = 0;

        UpdateCharacterAttributes(HeroManager.Instance.CurrentHero(), attributeMenuIndex);
    }

    public bool AddItemToInventory(Item item, int amount = 1)
    {
        if (item == null)
        {
            return false;
        }

        bool added = false;

        if (item.itemObject != null && item.itemObject.unique && 
            (inventoryObject.ContainsItemObject(item.itemObject) || 
            consumablesObject.ContainsItemObject(item.itemObject)))
        {
            Debug.Log($"{item.itemObject.name} is unique and already in your inventory.");
            return false;
        }

        for (int i = 0; i < amount; i++)
        {
            if (item.itemObject is Food)
            {
                added = consumablesObject.AddItem(item, 1);
            }
            else
            {
                added = inventoryObject.AddItem(item, 1);
            }

            if (added)
            {
                OnItemObtained?.Invoke(item.itemObject);
            }
        }

        return added;
    }

    public void RemoveItemFromInventory(ItemObject itemObject, int amount = 1)
    {
        if (itemObject == null)
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (itemObject is Food)
            {
                try
                {
                    consumablesObject.RemoveItem(itemObject, 1);
                }
                catch
                {
                    Debug.Log($"Could not remove {itemObject} from consumables.");
                }
            }
            else
            {
                try
                {
                    inventoryObject.RemoveItem(itemObject, 1);
                }
                catch
                {
                    Debug.Log($"Could not remove {itemObject} from inventory.");
                }              
            }
        }
    }

    public bool HasItemInInventory(ItemObject itemObject)
    {
        bool checkInventory = inventoryObject.ContainsItemObject(itemObject);
        bool checkConsumables = consumablesObject.ContainsItemObject(itemObject);
        bool checkHero1 = equipmentObjects[0] != null ? equipmentObjects[0].ContainsItemObject(itemObject) : false;
        bool checkHero2 = equipmentObjects[1] != null ? equipmentObjects[1].ContainsItemObject(itemObject) : false;
        bool checkHero3 = equipmentObjects[2] != null ? equipmentObjects[2].ContainsItemObject(itemObject) : false;

        return checkInventory || checkConsumables || checkHero1 || checkHero2 || checkHero3;
    }

    private void ResetInventories()
    {
        foreach (InventoryObject inventory in equipmentObjects)
        {
            inventory.container.Clear();
        }

        inventoryObject.container.Clear();
        consumablesObject.container.Clear();

        Debug.Log("Equipments and Inventory has been reset.");
    }
}
