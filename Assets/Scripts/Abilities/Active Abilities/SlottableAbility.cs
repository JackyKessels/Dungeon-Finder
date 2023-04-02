using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlottableAbility : MonoBehaviour, IPointerClickHandler
{
    public int slot;

    private void Start()
    {
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnter(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExit(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.BeginDrag, delegate { OnDragStart(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.EndDrag, delegate { OnDragEnd(gameObject); });
        ObjectUtilities.AddEvent(gameObject, EventTriggerType.Drag, delegate { OnDrag(gameObject); });
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
        }

        return tempAbility;
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempObjectBeingDragged != null)
        {
            MouseData.tempObjectBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempObjectBeingDragged);

        if (MouseData.slotHoveredOver && MouseData.slotHoveredOver.GetComponent<SlottableAbility>() != null)
        {
            int targetSlot = MouseData.slotHoveredOver.GetComponent<SlottableAbility>().slot;

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