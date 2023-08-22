using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CurrentState
{
    Battle,
    HeroInformation,
    Reward,
    Town,
    Default,
    Values
}

public class TooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameManager gameManager;
    private BattleManager battleManager;
    private TooltipHandler tooltipHandler;

    [Header("[ Generic Tooltip ]")]
    public bool useGenericTooltip = false;
    [TextArea(3,3)]
    public string genericTooltip;

    [Header("[ Specific Tooltips ]")]
    public Active active = null;
    public Passive passive = null;
    public Effect effect = null;
    public Item item = null;
    public Attribute attribute = null;
    public Currency currency = null;
    public CurrentState state = CurrentState.Default;
    public bool experience = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        battleManager = BattleManager.Instance;
        tooltipHandler = TooltipHandler.Instance;
    }

    public void SetTooltip(TooltipObject tooltipObject)
    {
        useGenericTooltip = tooltipObject.useGenericTooltip;
        genericTooltip = tooltipObject.genericTooltip;
        active = tooltipObject.active;  
        effect = tooltipObject.effect;           
        item = tooltipObject.item;
        attribute = tooltipObject.attribute;
        currency = tooltipObject.currency;
        state = tooltipObject.state;
        experience = tooltipObject.experience;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useGenericTooltip)
        {
            tooltipHandler.ShowTooltip(genericTooltip, transform.position);
        }
        else if (item != null && item.itemObject != null)
        {
            tooltipHandler.ShowTooltip(item, this, transform.position);
        }
        else if (active != null && active.activeAbility != null)
        {
            tooltipHandler.ShowTooltip(active.activeAbility, this, transform.position);
        }
        else if (passive != null && passive.passiveAbility != null)
        {
            tooltipHandler.ShowTooltip(passive.passiveAbility, this, transform.position);
        }
        else if (effect != null && effect.effectObject != null)
        {
            tooltipHandler.ShowTooltip(effect.effectObject, this, transform.position);
        }
        //else if (GetComponent<Location>() != null)
        //{
        //    Debug.Log("HELLO");
        //    tooltipHandler.ShowTooltip(GetComponent<Location>(), this, transform.position);
        //}
        else if (GetComponent<CharacterAttribute>() != null)
        {
            tooltipHandler.ShowTooltip(GetComponent<CharacterAttribute>(), this, transform.position);
        }
        else if (GetComponent<CurrencyObject>() != null | currency.totalAmount > 0)
        {
            tooltipHandler.ShowTooltip(currency, this, transform.position);
        }
        else if (GetComponent<DungeonObject>() != null)
        {
            tooltipHandler.ShowTooltip(GetComponent<DungeonObject>(), this, transform.position);
        }
        else if (GetComponent<HeroPathGameObject>() != null)
        {
            tooltipHandler.ShowTooltip(GetComponent<HeroPathGameObject>(), this, new Vector2(transform.position.x, transform.position.y + 6));
        }
        else if (experience)
        {
            tooltipHandler.ShowTooltip("Experience points are required to level up your team.", transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipHandler.Instance.HideTooltip();
    }

    public void OnDestroy()
    {
        TooltipHandler.Instance.HideTooltip();
    }

    private void OnDisable()
    {
        TooltipHandler.Instance.HideTooltip();
    }
}
