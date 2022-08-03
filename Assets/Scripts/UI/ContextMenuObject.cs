using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ContextMenuHandler.Instance.overMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ContextMenuHandler.Instance.overMenu = false;
    }
}
