using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : InventoryInterface
{
    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventoryObject.GetSlots.Length; i++)
        {
            var obj = Instantiate(GameAssets.i.inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<TooltipObject>().itemObject = null;
            obj.GetComponent<TooltipObject>().state = CurrentState.Values;
            obj.GetComponent<InteractableItem>().Setup(inventoryObject.GetSlots[i], false);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            inventoryObject.GetSlots[i].slotDisplay = obj;

            slotsOnInterface.Add(obj, inventoryObject.GetSlots[i]);
        }
    }
}
