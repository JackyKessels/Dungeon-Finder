using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveAbilitySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Button button;
    public TooltipObject tooltip;

    public int slot;
    public bool locked = false;

    private void Start()
    {
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnter(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExit(); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.BeginDrag, delegate { OnDragStart(); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.EndDrag, delegate { OnDragEnd(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.Drag, delegate { OnDrag(); });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UnslotAbility();
        }
    }

    private void UnslotAbility()
    {
        SpellbookManager.Instance.SetActiveSlotInactive(slot);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }

    public void OnExit()
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnDragStart()
    {
        MouseData.tempObjectBeingDragged = CreateTempAbility();
    }

    public GameObject CreateTempAbility()
    {
        GameObject tempAbility = null;

        if (tooltip.active.activeAbility != null)
        {
            tempAbility = new GameObject();
            var rt = tempAbility.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(36, 36);
            tempAbility.transform.SetParent(SpellbookManager.Instance.draggableContainer.transform);
            rt.localScale = Vector3.one;
            var img = tempAbility.AddComponent<Image>();
            img.sprite = tooltip.active.activeAbility.icon;
            img.raycastTarget = false;
        }

        return tempAbility;
    }

    public void OnDrag()
    {
        if (MouseData.tempObjectBeingDragged != null)
        {
            MouseData.tempObjectBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempObjectBeingDragged);

        if (MouseData.slotHoveredOver && 
            MouseData.slotHoveredOver.GetComponent<ActiveAbilitySlot>() != null &&
            !MouseData.slotHoveredOver.GetComponent<ActiveAbilitySlot>().locked)
        {
            int targetSlot = MouseData.slotHoveredOver.GetComponent<ActiveAbilitySlot>().slot;

            TooltipObject currentObject = obj.GetComponent<TooltipObject>();
            TooltipObject targetObject = MouseData.slotHoveredOver.GetComponent<TooltipObject>();

            Active currentActive = currentObject.active;
            Active targetActive = targetObject.active;

            Unit unit = GeneralUtilities.GetCorrectUnit(currentObject);

            unit.spellbook.activeSpellbook[slot] = targetActive;
            unit.spellbook.activeSpellbook[targetSlot] = currentActive;

            if (targetActive != null && targetActive.activeAbility != null)
                unit.spellbook.activeSpellbook[slot].Initialize();

            if (currentActive != null && currentActive.activeAbility != null)
                unit.spellbook.activeSpellbook[targetSlot].Initialize();

            SpellbookManager.Instance.UpdateActiveAbilities();

            if (currentActive != null && currentActive.activeAbility != null)
                TooltipHandler.Instance.ShowTooltip(targetObject.active.activeAbility, targetObject, targetObject.transform.position);
            else
                TooltipHandler.Instance.HideTooltip();
        }
    }


}