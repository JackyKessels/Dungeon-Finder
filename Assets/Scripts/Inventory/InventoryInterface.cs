using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public abstract class InventoryInterface : MonoBehaviour
{
    protected GameManager gameManager;
    protected BattleManager battleManager;
    protected TeamManager teamManager;
    protected InventoryManager inventoryManager;

    [HideInInspector] public bool hasBeenCreated = false;

    [HideInInspector] public InventoryObject inventoryObject;

    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        battleManager = BattleManager.Instance;
        teamManager = TeamManager.Instance;
        inventoryManager = InventoryManager.Instance;

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    public void Setup(InventoryObject currentInventory)
    {
        inventoryObject = currentInventory;

        for (int i = 0; i < inventoryObject.GetSlots.Length; i++)
        {
            inventoryObject.GetSlots[i].parent = this;
            inventoryObject.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }

        CreateSlots();
    }

    public void UpdateItems()
    {
        for (int i = 0; i < inventoryObject.GetSlots.Length; i++)
        {
            OnSlotUpdate(inventoryObject.GetSlots[i]);
        }
    }

    protected void OnSlotUpdate(InventorySlot _slot)
    {
        if (_slot.item.id >= 0)
        {
            _slot.slotDisplay.GetComponent<Image>().sprite = _slot.ItemObject.icon;
            _slot.slotDisplay.GetComponent<TooltipObject>().itemObject = _slot.ItemObject;

            if (_slot.slotDisplay.GetComponent<EquipmentSlotIcon>() == null)
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
        }
        else
        {
            if (_slot.slotDisplay.GetComponent<EquipmentSlotIcon>() != null)
            {
                if (_slot.restricted)
                {
                    _slot.slotDisplay.GetComponent<Image>().sprite = GameAssets.i.restrictedSlot;
                    _slot.slotDisplay.GetComponent<TooltipObject>().itemObject = null;
                }
                else
                {
                    _slot.slotDisplay.GetComponent<Image>().sprite = _slot.slotDisplay.GetComponent<EquipmentSlotIcon>().icon;
                    _slot.slotDisplay.GetComponent<TooltipObject>().itemObject = null;
                }
            }
            else
            {
                _slot.slotDisplay.GetComponent<Image>().sprite = GameAssets.i.emptySlot;
                _slot.slotDisplay.GetComponent<TooltipObject>().itemObject = null;
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
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
        MouseData.tempObjectBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;

        if (slotsOnInterface[obj].item.id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(32, 32);
            tempItem.transform.SetParent(HeroManager.Instance.heroInformationObject.transform);
            rt.localScale = Vector3.one;
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.icon;
            img.raycastTarget = false;
        }
        return tempItem;
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempObjectBeingDragged);

        if (MouseData.slotHoveredOver)
        {
            // If dragged into a SellSlot
            if (MouseData.slotHoveredOver.GetComponent<SellSlot>() != null)
            {
                if (MouseData.slotHoveredOver.GetComponent<SellSlot>().sellSystem.sellType == SellType.Items)
                {
                    SellSlot sellSlot = MouseData.slotHoveredOver.GetComponent<SellSlot>();
                    sellSlot.UpdateSlot(slotsOnInterface[obj]);
                }
            }
            else
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
                inventoryObject.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);

                if (mouseHoverSlotData.item.itemObject != null)
                    TooltipHandler.Instance.ShowTooltip(mouseHoverSlotData.item.itemObject, mouseHoverSlotData.slotDisplay.GetComponent<TooltipObject>(), MouseData.slotHoveredOver.transform.position);
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

    protected void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<InventoryInterface>();
    }

    protected void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }
}

public static class MouseData
{
    public static InventoryInterface interfaceMouseIsOver;
    public static GameObject tempObjectBeingDragged;
    public static GameObject slotHoveredOver;
}
