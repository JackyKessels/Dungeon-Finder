using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticInterface : InventoryInterface
{
    public GameObject[] slots;

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < inventoryObject.GetSlots.Length; i++)
        {
            var obj = slots[i];

            obj.GetComponent<TooltipObject>().state = CurrentState.Values;
            obj.GetComponent<InteractableItem>().Setup(inventoryObject.GetSlots[i], true);

            if (!hasBeenCreated)
            {
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });           
            }

            inventoryObject.GetSlots[i].slotDisplay = obj;

            slotsOnInterface.Add(obj, inventoryObject.GetSlots[i]);
        }
        hasBeenCreated = true;
    }
}
