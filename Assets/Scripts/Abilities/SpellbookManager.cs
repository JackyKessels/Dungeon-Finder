using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class SpellbookManager : MonoBehaviour
{
    #region Singleton
    public static SpellbookManager Instance { get; private set; }

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

    [Header("Variables")]
    public GameObject spellbookPanel;           // Background of the Spellbook

    public GameObject activeAbilitiesContainer; // Active abilities container object
    public GameObject[] activeAbilities;        // List of all active abilities

    public GameObject passiveAbilitiesContainer;
    public Button nextPassivesPage;
    public Button previousPassivesPage;
    private int passiveAbilitiesPage = 0;
    private readonly int passiveAbilitiesTotalSlots = 4;

    public GameObject collectionContainer;      // Container object that holds all available abilities

    public GameObject itemAbilitiesContainer;   // Item ability container
    public Button nextItemPage;
    public Button previousItemPage;
    private int itemAbilitiesPage = 0;
    private readonly int itemAbilitiesTotalSlots = 4;

    public GameObject abilityPrefab;            // Prefab that is used to fill the container

    public Hero currentHero;

    private void Start()
    {
        teamManager = TeamManager.Instance;

        activeAbilities = new GameObject[4];

        for (int i = 0; i < activeAbilities.Length; i++)
        {
            activeAbilities[i] = activeAbilitiesContainer.transform.GetChild(i).gameObject;
        }

        foreach (GameObject obj in activeAbilities)
        {
            ObjectUtilities.AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            ObjectUtilities.AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
        }
    }

    public void Setup(Hero hero)
    {
        currentHero = hero;

        GenerateAbilityCollection();

        CheckItemAbilitiesButtons();
        GenerateItemAbilities();

        CheckActiveValidity();

        UpdateActiveAbilities();

        CheckPassiveAbilitiesButtons();
        GeneratePassiveAbilities();
    }

    // Active Abilities //
    // Checks every active ability of the Hero and if it can actually be used
    // Yes: do nothing
    // No: remove ability and set empty
    public void CheckActiveValidity()
    {
        for (int i = 0; i < currentHero.spellbook.activeSpellbook.Length; i++)
        {
            if ((!currentHero.spellbook.abilityCollection.Contains(currentHero.spellbook.activeSpellbook[i])) ||
                (currentHero.spellbook.activeSpellbook[i].activeAbility != null && currentHero.spellbook.HasWeaponRequirement(currentHero.spellbook.activeSpellbook[i]) == false))
            {
                SetActiveSlotInactive(i);
            }
        }
    }

    private void SetActiveSlotInactive(int slot)
    {
        currentHero.spellbook.activeSpellbook[slot] = new Active(null, 0);
        activeAbilities[slot].GetComponent<TooltipObject>().active = new Active(null, 0);
        activeAbilities[slot].GetComponent<Image>().sprite = GameAssets.i.noAbility;
    }

    // Updates all active abilities with the Hero's active abilities
    public void UpdateActiveAbilities()
    {
        for (int i = 0; i < currentHero.spellbook.activeSpellbook.Length; i++)
        {
            Image b = activeAbilities[i].GetComponent<Image>();
            TooltipObject t = activeAbilities[i].GetComponent<TooltipObject>();

            if (currentHero.spellbook.activeSpellbook[i].activeAbility != null)
            {
                t.active = currentHero.spellbook.activeSpellbook[i];
                t.state = CurrentState.HeroInformation;
                b.sprite = currentHero.spellbook.activeSpellbook[i].activeAbility.icon;
            }
            else
            {
                t.active = new Active(null, 0);
                b.sprite = GameAssets.i.noAbility;
            }
        }
    }

    // Prevents abilities from being slotted twice
    public bool ContainsAbility(Active ability)
    {
        bool isInList = false;

        for (int i = 0; i < activeAbilities.Length; i++)
        {
            if (activeAbilities[i].GetComponent<TooltipObject>().active.activeAbility == ability.activeAbility)
            {
                isInList = true;
                Debug.Log("Already contains: " + ability.activeAbility.name);
            }
        }

        return isInList;
    }

    // Active Abilities //

    // Ability Collection //
    // Adds all abilities to the collection
    private void GenerateAbilityCollection()
    {
        GameObject container = collectionContainer;

        ObjectUtilities.ClearContainer(container);

        int totalSize = 8;

        int collectionSize = Mathf.Min(currentHero.spellbook.abilityCollection.Count, totalSize);

        int emptySize = totalSize - collectionSize;

        for (int i = 0; i < collectionSize; i++)
        {
            CreateAbility(currentHero.spellbook.abilityCollection[i], container);
        }

        for (int i = 0; i < emptySize; i++)
        {
            CreateAbility(container);
        }
    }

    private void GenerateItemAbilities()
    {
        GameObject container = itemAbilitiesContainer;

        ObjectUtilities.ClearContainer(container);

        int collectionSize;

        int emptySize;

        if (itemAbilitiesPage == 0)
        {
            collectionSize = Mathf.Min(currentHero.spellbook.itemAbilities.Count, itemAbilitiesTotalSlots);

            emptySize = itemAbilitiesTotalSlots - collectionSize;

            for (int i = 0; i < collectionSize; i++)
            {
                CreateAbility(currentHero.spellbook.itemAbilities[i], container, false);
            }

            for (int i = 0; i < emptySize; i++)
            {
                CreateAbility(container);
            }
        }
        else if (itemAbilitiesPage == 1)
        {
            collectionSize = Mathf.Min(currentHero.spellbook.itemAbilities.Count - itemAbilitiesTotalSlots, itemAbilitiesTotalSlots);

            emptySize = itemAbilitiesTotalSlots - collectionSize;

            for (int i = 0; i < collectionSize; i++)
            {
                CreateAbility(currentHero.spellbook.itemAbilities[i + itemAbilitiesTotalSlots], container, false);
            }

            for (int i = 0; i < emptySize; i++)
            {
                CreateAbility(container);
            }
        }
    }

    private void GeneratePassiveAbilities()
    {
        GameObject container = passiveAbilitiesContainer;

        ObjectUtilities.ClearContainer(container);

        int filledSlots = Mathf.Min(currentHero.spellbook.passives.Count - passiveAbilitiesPage * passiveAbilitiesTotalSlots, passiveAbilitiesTotalSlots);

        int emptySlots = passiveAbilitiesTotalSlots - filledSlots;

        for (int i = 0; i < filledSlots; i++)
        {
            CreateAbility(currentHero.spellbook.passives[i + passiveAbilitiesPage * passiveAbilitiesTotalSlots]);
        }

        for (int i = 0; i < emptySlots; i++)
        {
            CreateAbility(container);
        }
    }

    //public void GeneratePassiveAbilities()
    //{
    //    GeneralUtilities.ClearContainer(passiveAbilitiesContainer);

    //    foreach (Passive passive in currentHero.spellbook.passives)
    //    {
    //        CreateAbility(passive);
    //    }
    //}

    public void NextItemPage()
    {
        itemAbilitiesPage++;

        if (itemAbilitiesPage > 1)
            itemAbilitiesPage = 0;

        GenerateItemAbilities();
    }

    public void PreviousItemPage()
    {
        itemAbilitiesPage--;

        if (itemAbilitiesPage < 0)
            itemAbilitiesPage = 1;

        GenerateItemAbilities();
    }

    private void CheckItemAbilitiesButtons()
    {
        if (currentHero.spellbook.itemAbilities.Count > itemAbilitiesTotalSlots)
        {
            SetItemButtonState(true);
        }
        else
        {
            itemAbilitiesPage = 0;
            SetItemButtonState(false);
        }
    }

    public void NextPassivesPage()
    {
        passiveAbilitiesPage++;

        if (passiveAbilitiesPage > GetCorrectPageCount() - 1)
            passiveAbilitiesPage = 0;

        GeneratePassiveAbilities();
    }

    public void PreviousPassivesPage()
    {
        passiveAbilitiesPage--;
    
        if (passiveAbilitiesPage < 0)
            passiveAbilitiesPage = GetCorrectPageCount() - 1;

        GeneratePassiveAbilities();
    }

    private void CheckPassiveAbilitiesButtons()
    {
        if (currentHero.spellbook.passives.Count > passiveAbilitiesTotalSlots)
        {
            passiveAbilitiesPage = GetCorrectPageCount() - 1;
            SetPassiveButtonState(true);
        }
        else
        {
            passiveAbilitiesPage = 0;
            SetPassiveButtonState(false);
        }
    }

    private void SetItemButtonState(bool active)
    {
        nextItemPage.interactable = active;
        previousItemPage.interactable = active;
    }

    private void SetPassiveButtonState(bool active)
    {
        nextPassivesPage.interactable = active;
        previousPassivesPage.interactable = active;
    }

    private int GetCorrectPageCount()
    {
        return (currentHero.spellbook.passives.Count + passiveAbilitiesTotalSlots - 1) / passiveAbilitiesTotalSlots;
    }

    // Abilitiy Collection //


    // Creates the draggable ability that gets added to the collection
    public void CreateAbility(Active ability, GameObject container, bool draggable = true)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityPrefab, container);
            
        TooltipObject info = obj.GetComponent<TooltipObject>();
        info.active = ability;
        info.state = CurrentState.HeroInformation;
        Image image = obj.GetComponent<Image>();
        image.sprite = ability.activeAbility.icon;

        if (draggable)
        {
            QuickSlot qs = obj.AddComponent<QuickSlot>();
            qs.Ability = ability;

            ObjectUtilities.AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            ObjectUtilities.AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            ObjectUtilities.AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            ObjectUtilities.AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            ObjectUtilities.AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
        }
    }

    // Create empty ability
    private void CreateAbility(GameObject container)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityPrefab, container);

        Image image = obj.GetComponent<Image>();
        image.sprite = GameAssets.i.emptySlot;
    }

    // Passive Ability Only
    private void CreateAbility(Passive passive)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityPrefab, passiveAbilitiesContainer);

        TooltipObject info = obj.GetComponent<TooltipObject>();
        info.passive = passive;
        info.state = CurrentState.HeroInformation;

        Image image = obj.GetComponent<Image>();
        image.sprite = passive.passiveAbility.icon;
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }

    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        MouseData.tempObjectBeingDragged = CreateTempAbility(obj);
    }

    public GameObject CreateTempAbility(GameObject obj)
    {
        GameObject tempAbility = null;

        TooltipObject info = obj.GetComponent<TooltipObject>();

        if (info.active.activeAbility != null)
        {
            tempAbility = new GameObject();

            var rt = tempAbility.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(36, 36);
            tempAbility.transform.SetParent(HeroManager.Instance.heroInformationObject.transform);
            rt.localScale = Vector3.one;

            var img = tempAbility.AddComponent<Image>();
            img.sprite = info.active.activeAbility.icon;
            img.raycastTarget = false;
            tempAbility.name = info.active.activeAbility.name;
        }

        return tempAbility;
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempObjectBeingDragged);

        if (MouseData.slotHoveredOver && MouseData.slotHoveredOver.GetComponent<TooltipObject>())
        {
            TooltipObject draggedAbility = obj.GetComponent<TooltipObject>();



            // If dropped in a Sell Slot
            if (MouseData.slotHoveredOver.GetComponent<SellSlot>() != null)
            {
                if (MouseData.slotHoveredOver.GetComponent<SellSlot>().sellSystem.sellType == SellType.Abilities)
                {
                    TooltipObject targetAbility = MouseData.slotHoveredOver.GetComponent<TooltipObject>();
                    targetAbility.active = draggedAbility.active;

                    SellSlot sellSlot = MouseData.slotHoveredOver.GetComponent<SellSlot>();
                    sellSlot.UpdateSlot(targetAbility.active);
                }
            }
            // If dropped in a Active Ability slot
            else if (MouseData.slotHoveredOver.GetComponent<SlottableAbility>() &&
                    !ContainsAbility(draggedAbility.active) &&
                    currentHero.spellbook.HasWeaponRequirement(draggedAbility.active))
            {
                TooltipObject targetAbility = MouseData.slotHoveredOver.GetComponent<TooltipObject>();

                if (currentHero.spellbook.ValidActiveAbility(draggedAbility.active, targetAbility.active))
                {
                    targetAbility.active = draggedAbility.active;

                    SlottableAbility slottableAbility = MouseData.slotHoveredOver.GetComponent<SlottableAbility>();

                    currentHero.spellbook.activeSpellbook[slottableAbility.slot] = targetAbility.active;
                    currentHero.spellbook.activeSpellbook[slottableAbility.slot].Initialize();

                    UpdateActiveAbilities();

                    TooltipHandler.Instance.ShowTooltip(targetAbility.active.activeAbility, targetAbility, targetAbility.transform.position);
                }
                else
                {
                    ShortMessage.SendMessage(Input.mousePosition, "Already slotted a mystical ability.", 24, Color.red);
                }
            }            
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempObjectBeingDragged != null)
        {
            MouseData.tempObjectBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}
