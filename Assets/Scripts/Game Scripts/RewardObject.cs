using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    Ability,
    Item,
    Attribute,
    Currency,
    Mix
}

public class RewardObject : MonoBehaviour
{
    public Reward reward; 

    private TooltipObject tooltipIcon;

    public void SetData(Reward r)
    {
        reward = r;

        tooltipIcon = GetComponent<TooltipObject>();
        tooltipIcon.state = CurrentState.Values;

        switch (reward.type)
        {
            case RewardType.Ability:
                GetComponent<Image>().sprite = reward.ability.activeAbility.icon;
                tooltipIcon.active = reward.ability;            
                break;
            case RewardType.Item:
                GetComponent<Image>().sprite = reward.itemDrop.itemObject.icon;
                tooltipIcon.itemObject = reward.itemDrop.itemObject;
                break;
            case RewardType.Attribute:
                GetComponent<Image>().sprite = GeneralUtilities.GetAttributeIcon(reward.attribute.attributeType);
                tooltipIcon.attribute = reward.attribute;
                break;
            case RewardType.Currency:
                GetComponent<Image>().sprite = Currency.GetCurrencyIcon(r.currencyType, true);
                tooltipIcon.currency.totalAmount = reward.currency;
                tooltipIcon.currency.currencyType = reward.currencyType;
                break;
        }
    }
}
