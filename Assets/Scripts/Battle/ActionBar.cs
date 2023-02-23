using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ActionBar : MonoBehaviour
{
    public GameObject[] abilities = new GameObject[4];
    public GameObject flask;
    public GameObject pass;
    public GameObject itemBar;
    [SerializeField] GameObject itemBarPrefab;

    private ActionBarButton[] heroAbilities;
    private ActionBarButton[] itemAbilities;
    private ActionBarButton flaskButton;
    private ActionBarButton passButton;

    [SerializeField] private Sprite emptyFlaskIcon;

    public void Start()
    {
        AddEvent(abilities[0], EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(0, true); });
        AddEvent(abilities[1], EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(1, true); });
        AddEvent(abilities[2], EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(2, true); });
        AddEvent(abilities[3], EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(3, true); });
        AddEvent(flask, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnFlaskButton(); });
        AddEvent(pass, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnPassButton(); });
    }

    public List<ActionBarButton> GetItemAbilityButtons()
    {
        List<ActionBarButton> itemAbilityButtons = new List<ActionBarButton>();

        foreach (Transform child in itemBar.transform)
        {
            itemAbilityButtons.Add(child.GetComponent<ActionBarButton>());
        }

        return itemAbilityButtons;
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
        heroAbilities = transform.GetComponentsInChildren<ActionBarButton>();
        itemAbilities = itemBar.GetComponentsInChildren<ActionBarButton>();

        flaskButton = flask.GetComponent<ActionBarButton>();
        passButton = pass.GetComponent<ActionBarButton>();

        foreach (ActionBarButton b in heroAbilities)
        {
            b.active = true;
        }

        foreach (ActionBarButton b in itemAbilities)
        {
            b.active = true;
        }

        flaskButton.active = true;
        passButton.active = true;
    }

    public void SetInteractable(bool active)
    {
        heroAbilities = transform.GetComponentsInChildren<ActionBarButton>();
        itemAbilities = itemBar.GetComponentsInChildren<ActionBarButton>();

        flaskButton = flask.GetComponent<ActionBarButton>();

        foreach (ActionBarButton b in heroAbilities)
        {
            b.SetInteractable(active);
        }

        foreach (ActionBarButton b in itemAbilities)
        {
            b.SetInteractable(active);
        }

        flaskButton.interactable = active;
        passButton.interactable = active;
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
                SetupActionBarAbility(abilities[i],
                                      currentHero.spellbook.activeSpellbook[i].activeAbility.icon,
                                      currentHero.spellbook.activeSpellbook[i],
                                      CurrentState.Battle);
            }
            else
            {
                SetupActionBarAbility(abilities[i],
                                      GameAssets.i.noAbility,
                                      null,
                                      CurrentState.Battle);
            }
        }
    }

    private void SetupFlask(Hero currentHero)
    {
        if (currentHero.GetFlask() != null)
        {
            SetupActionBarAbility(flask,
                                  currentHero.spellbook.flaskAbility.activeAbility.icon,
                                  currentHero.spellbook.flaskAbility,
                                  CurrentState.Battle);
        }
        else
        {
            SetupActionBarAbility(flask,
                                  emptyFlaskIcon,
                                  null,
                                  CurrentState.Battle);
        }
    }

    private void SetupItemBar(Hero currentHero)
    {
        ObjectUtilities.ClearContainer(itemBar);

        if (currentHero.spellbook.itemAbilities.Count <= 0)
            return;

        for (int i = 0; i < currentHero.spellbook.itemAbilities.Count; i++)
        {
            Active active = currentHero.spellbook.itemAbilities[i];

            GameObject itemBarAbility = ObjectUtilities.CreateSimplePrefab(itemBarPrefab, itemBar);
            itemBarAbility.name = active.activeAbility.name;

            ActionBarButton actionBarButton = itemBarAbility.GetComponent<ActionBarButton>();

            actionBarButton.SetHotkeyText(i);

            SetupActionBarAbility(itemBarAbility,
                                  active.activeAbility.icon,
                                  active,
                                  CurrentState.Battle);

            int index = i;
            AddEvent(itemBarAbility, EventTriggerType.PointerDown, delegate { BattleManager.Instance.OnAbilityButton(index, false); });
        }
    }

    public int GetTotalItemAbilities()
    {
        return itemBar.transform.childCount;
    }

    private void SetupActionBarAbility(GameObject _buttonObject, Sprite _sprite, Active _active, CurrentState _state)
    {
        _buttonObject.GetComponent<Image>().sprite = _sprite;
        _buttonObject.GetComponent<TooltipObject>().active = _active;
        _buttonObject.GetComponent<TooltipObject>().state = _state;
        _buttonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("");
    }

    public void UpdateCooldowns(Hero currentHero, bool expendedTurn = false)
    {
        SetInteractable(true);

        for (int i = 0; i < currentHero.spellbook.activeSpellbook.Length; i++)
        {
            ActionBarButton b = abilities[i].GetComponent<ActionBarButton>();
            TextMeshProUGUI t = abilities[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (currentHero.spellbook.activeSpellbook[i].IsOnCooldown())
            {
                b.SetInteractable(false);

                if (currentHero.spellbook.activeSpellbook[i].currentCooldown > currentHero.spellbook.activeSpellbook[i].cooldown)
                //if (!currentHero.spellbook.activeSpellbook[i].activeAbility.endTurn && expendedTurn)
                {
                    t.text = "";
                }
                else
                {
                    t.SetText(currentHero.spellbook.activeSpellbook[i].currentCooldown.ToString());
                }
            }
        }

        for (int i = 0; i < currentHero.spellbook.itemAbilities.Count; i++)
        {
            ActionBarButton b = GetItemAbilityButtons()[i].GetComponent<ActionBarButton>();
            TextMeshProUGUI t = GetItemAbilityButtons()[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (currentHero.spellbook.itemAbilities[i].IsOnCooldown())
            {
                b.SetInteractable(false);

                if (currentHero.spellbook.itemAbilities[i].currentCooldown > currentHero.spellbook.itemAbilities[i].cooldown)
                //if (!currentHero.spellbook.itemAbilities[i].activeAbility.endTurn && expendedTurn)
                {
                    t.text = "";
                }
                else
                {
                    t.SetText(currentHero.spellbook.itemAbilities[i].currentCooldown.ToString());
                }
            }
        }

        if (currentHero.GetFlask() != null)
        {
            ActionBarButton b = flask.GetComponent<ActionBarButton>();
            TextMeshProUGUI t = flask.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (currentHero.spellbook.flaskAbility.IsOnCooldown())
            {
                b.SetInteractable(false);

                if (currentHero.spellbook.flaskAbility.currentCooldown > currentHero.spellbook.flaskAbility.cooldown)
                //if (!currentHero.spellbook.flaskAbility.activeAbility.endTurn && expendedTurn)
                {
                    t.text = "";
                }
                else
                {
                    t.SetText(currentHero.spellbook.flaskAbility.currentCooldown.ToString());
                }
            }
        }
    }
}
