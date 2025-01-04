using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : IHasTooltip
{
    public string name;
    public int id = -1;
    public ItemObject itemObject;
    public int level;

    public Item()
    {
        name = "";
        id = -1;
        itemObject = null;
        level = 1;
    }

    private Item(ItemObject _itemObject, int _level)
    {
        itemObject = _itemObject;

        name = itemObject.name;
        id = itemObject.item.id;
        level = _level;
    }

    public static Item CreateItem(ItemObject _itemObject, int _level)
    {
        if (_itemObject is EquipmentObject equipmentObject)
        {
            return new Equipment(equipmentObject, _level);
        }
        else if (_itemObject.item.id != -1)
        {
            return new Item(_itemObject, _level);
        }
        else
        {
            return new Item();
        }
    }

    public string GetName()
    {
        return $"<smallcaps><b><color={ColorDatabase.QualityColor(itemObject.quality)}>{name}</color></b></smallcaps>";
    }

    public virtual string GetCompleteTooltip(TooltipObject tooltipInfo)
    {
        if (itemObject is Consumable consumable)
        {
            return itemObject.GetName() +
                   consumable.GetTooltip(tooltipInfo) +
                   itemObject.GetDescription() +
                   consumable.HowToUseText();
        }

        return itemObject.GetName();
    }

    public virtual void LeftClick(InteractableItem interactableItem)
    {

    }

    public virtual void RightClick(InteractableItem interactableItem)
    {

    }
}
