using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : IDescribable
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

    public virtual string GetDescription(TooltipObject tooltipInfo)
    {
        string color = ColorDatabase.QualityColor(itemObject.quality);

        string itemName = string.Format("<smallcaps><b><color={0}>{1}</color></b></smallcaps>", color, name);

        if (itemObject is Food food)
        {
            return food.GetDescription(tooltipInfo, itemName, GetItemDescription());
        }

        if (itemObject is Tome tome)
        {
            return tome.GetDescription(tooltipInfo, itemName, GetItemDescription());
        }

        if (itemObject is ResourceContainer container)
        {
            return container.GetDescription(tooltipInfo, itemName, GetItemDescription());
        }

        return itemName;
    }

    protected string GetItemDescription()
    {
        string color = "#FFD78B";

        if (itemObject.description != "")
        {
            return string.Format("<color={0}>\n\n\"{1}\"</color>", color, itemObject.description);
        }
        else
        {
            return "";
        }
    }

    public virtual void LeftClick(InteractableItem interactableItem)
    {

    }

    public virtual void RightClick(InteractableItem interactableItem)
    {

    }
}
