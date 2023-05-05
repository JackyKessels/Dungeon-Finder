using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlot : MonoBehaviour, IPointerClickHandler
{
    public Active Ability { get; set; }
    public GameObject draggableContainer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SlotAbility();
            ObjectUtilities.ClearContainer(draggableContainer);
        }
    }

    private void SlotAbility()
    {
        if (!SpellbookManager.Instance.ContainsAbility(Ability))
        {
            Hero hero = SpellbookManager.Instance.currentHero;

            hero.spellbook.AddLearnedToActive(Ability, -1);
            SpellbookManager.Instance.Setup(hero);
        }
    }
}

