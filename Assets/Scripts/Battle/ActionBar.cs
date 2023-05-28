using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ActionBar : MonoBehaviour
{
    public ActionBarButton[] heroAbilities = new ActionBarButton[4];
    public List<ActionBarButton> itemAbilities;
    public ActionBarButton flask;
    public ActionBarButton pass;

    public GameObject itemBar;
    [SerializeField] GameObject itemBarPrefab;

    [SerializeField] private Sprite emptyFlaskIcon;

    public void Start()
    {
        AddEvent(heroAbilities[0].gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(0, true); });
        AddEvent(heroAbilities[1].gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(1, true); });
        AddEvent(heroAbilities[2].gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(2, true); });
        AddEvent(heroAbilities[3].gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(3, true); });
        AddEvent(flask.gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnFlaskButton(); });
        AddEvent(pass.gameObject, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnPassButton(); });
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void Initialize()
    {
        foreach (ActionBarButton b in heroAbilities)
        {
            b.isActive = true;
            b.CrossButton(false);
        }

        // itemAbilities has not been created at this point.
        //foreach (ActionBarButton b in itemAbilities)
        //{
        //    b.isActive = true;
        //    b.CrossButton(false);
        //}

        flask.isActive = true;
        flask.CrossButton(false);

        pass.isActive = true;
        pass.CrossButton(false);
    }

    public void SetInteractable(bool active)
    {
        foreach (ActionBarButton b in heroAbilities)
        {
            b.SetInteractable(active);
        }

        foreach (ActionBarButton b in itemAbilities)
        {
            b.SetInteractable(active);
        }

        flask.SetInteractable(active);
        pass.SetInteractable(active);
    }

    public void SetupActionBar(Hero currentHero)
    {
        SetupActiveBar(currentHero);

        SetupFlask(currentHero);

        SetupItemBar(currentHero);
    }

    private void SetupActiveBar(Hero currentHero)
    {
        for (int i = 0; i < currentHero.spellbook.activeSpellbook.Length; i++)
        {
            if (currentHero.spellbook.activeSpellbook[i].activeAbility != null)
            {
                heroAbilities[i].SetupActionBarAbility(
                    currentHero.spellbook.activeSpellbook[i].activeAbility.icon,
                    currentHero.spellbook.activeSpellbook[i],
                    CurrentState.Battle);                     
            }
            else
            {
                heroAbilities[i].SetupEmptyActionBarAbility(SpellbookManager.Instance.activeAbilities[i].locked);
            }
        }
    }

    private void SetupFlask(Hero currentHero)
    {
        if (currentHero.GetFlask() != null)
        {
            flask.SetupActionBarAbility(
                currentHero.spellbook.flaskAbility.activeAbility.icon,
                currentHero.spellbook.flaskAbility,
                CurrentState.Battle);
        }
        else
        {
            flask.SetupActionBarAbility(
                emptyFlaskIcon,
                null,
                CurrentState.Battle);
        }
    }

    private void SetupItemBar(Hero currentHero)
    {
        ObjectUtilities.ClearContainer(itemBar);
        itemAbilities.Clear();

        if (currentHero.spellbook.itemAbilities.Count <= 0)
            return;

        for (int i = 0; i < currentHero.spellbook.itemAbilities.Count; i++)
        {
            Active active = currentHero.spellbook.itemAbilities[i];

            GameObject itemBarAbility = ObjectUtilities.CreateSimplePrefab(itemBarPrefab, itemBar);
            itemBarAbility.name = active.activeAbility.name;
            RectTransform rt = itemBarAbility.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(80, 80);

            ActionBarButton itemAbility = itemBarAbility.GetComponent<ActionBarButton>();

            itemAbilities.Add(itemAbility);

            itemAbility.SetHotkeyText(i);

            itemAbility.SetupActionBarAbility(
                active.activeAbility.icon,
                active,
                CurrentState.Battle);

            int index = i;
            AddEvent(itemBarAbility, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(index, false); });
        }
    }

    public int GetTotalItemAbilities()
    {
        return itemAbilities.Count;
    }

    public void UpdateCooldowns(Hero currentHero)
    {
        SetInteractable(true);

        for (int i = 0; i < currentHero.spellbook.activeSpellbook.Length; i++)
        {
            heroAbilities[i].SetButtonCooldown(currentHero.spellbook.activeSpellbook[i]);
        }

        for (int i = 0; i < currentHero.spellbook.itemAbilities.Count; i++)
        {
            itemAbilities[i].SetButtonCooldown(currentHero.spellbook.itemAbilities[i]);
        }

        if (currentHero.GetFlask() != null)
        {
            flask.SetButtonCooldown(currentHero.spellbook.flaskAbility);
        }
    }
}
