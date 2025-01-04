using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableItem : MonoBehaviour, IPointerClickHandler
{
    private ItemObject itemObject;
    public InventorySlot inventorySlot;

    public bool equipped;

    public void Setup(InventorySlot _inventorySlot, bool _equipped)
    {
        inventorySlot = _inventorySlot;

        equipped = _equipped;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        itemObject = inventorySlot.item.itemObject;

        if (itemObject != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventorySlot.item.LeftClick(this);
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventorySlot.item.RightClick(this);

                RightClick();
            }
        }
    }

    public void RightClick()
    {
        if (itemObject is Consumable c)
        {
            if (c.consumptionType == ConsumptionType.Party)
            {
                List<ContextMenuElement> elements = new List<ContextMenuElement>();

                elements.Add(new ContextMenuElement(itemObject.name, false, ColorDatabase.QualityColor(itemObject.quality)));

                Action<int, GameObject> consume = new Action<int, GameObject>(Consume);
                elements.Add(new ContextMenuElement("Use", 0, consume));
          
                elements.Add(new ContextMenuElement("Close"));

                ContextMenuHandler.Instance.ShowContextMenu(elements, transform.position);

                TooltipHandler.Instance.HideTooltip();
            }
            else if (c.consumptionType == ConsumptionType.Single)
            {
                List<ContextMenuElement> elements = new List<ContextMenuElement>();

                elements.Add(new ContextMenuElement(itemObject.name, false, ColorDatabase.QualityColor(itemObject.quality)));

                List<Unit> targetGroup = c.usableOnDead ? TeamManager.Instance.heroes.Members : TeamManager.Instance.heroes.LivingMembers;

                for (int i = 0; i < targetGroup.Count; i++)
                {
                    Action<int, GameObject> consume = new Action<int, GameObject>(Consume);
                    elements.Add(new ContextMenuElement(targetGroup[i] as Hero, i, consume));
                }

                elements.Add(new ContextMenuElement("Close"));

                ContextMenuHandler.Instance.ShowContextMenu(elements, transform.position);

                TooltipHandler.Instance.HideTooltip();
            }
        }
    }

    public void Consume(int i, GameObject menu)
    {
        if (itemObject is Consumable c)
        {
            int amount = c.consumptionType == ConsumptionType.Party ? 0 : i;
            if (c.Consume(amount))
            {
                inventorySlot.ReduceAmount(1);
            }
            else
            {
                ShortMessage.SendMessage(Input.mousePosition, "Cannot consume item.", 24, Color.red);
            }
        }

        ContextMenuHandler.Instance.HideContextMenu();
    }
}
