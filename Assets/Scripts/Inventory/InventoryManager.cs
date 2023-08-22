using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private void Start()
    {
        ResetInventories();

        teamManager = TeamManager.Instance;
        currencyHandler = GameManager.Instance.currencyHandler;

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
            for (int i = 9; i < 18; i++)
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
        bool added = false;

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
        }

        return added;
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
