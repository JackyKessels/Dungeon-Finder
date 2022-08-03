using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class ItemObject : ScriptableObject, IDescribable
{
    [Header("General")]
    new public string name = "New Item";
    public Sprite icon = null;
    public int value = 0;
    public bool stackable = false;
    public int maxStacks = 1;
    public bool sellable = true;
    public Item item = new Item();

    [TextArea(5, 5)] public string description;

    public Quality quality = Quality.Common;

    public virtual string GetDescription(TooltipObject tooltipInfo)
    {
        string color = ColorDatabase.QualityColor(quality);

        return string.Format("<smallcaps><b><color={0}>{1}</color></b></smallcaps>", color, name);// + string.Format("\nType: {0}", type);
    }

    protected string GetItemDescription()
    {
        string color = "#FFD78B";

        if (description != "")
        {
            return string.Format("<color={0}>\n\n\"{1}\"</color>", color, description);
        }
        else
        {
            return "";
        }
    }

    public abstract void LeftClick(InteractableItem interactableItem);

    public abstract void RightClick(InteractableItem interactableItem);
}

[System.Serializable]
public class Item
{
    public string name;
    public int id = -1;
    public ItemObject itemObject;
    public List<Attribute> attributes;

    public Item()
    {
        name = "";
        id = -1;
        itemObject = null;
    }

    public Item(ItemObject data)
    {
        name = data.name;
        id = data.item.id;
        itemObject = data;

        if (data is Equipment e)
        {
            attributes = new List<Attribute>();

            foreach (AttributeType type in (AttributeType[])System.Enum.GetValues(typeof(AttributeType)))
            {
                attributes.Add(new Attribute(type));
            }

            foreach (Attribute a in attributes)
            {
                switch (a.attributeType)
                {
                    // General Attributes
                    case AttributeType.Health:
                        a.baseValue = e.health;
                        break;
                    case AttributeType.Power:
                        a.baseValue = e.power;
                        break;
                    case AttributeType.Wisdom:
                        a.baseValue = e.wisdom;
                        break;
                    case AttributeType.Armor:
                        a.baseValue = e.armor;
                        break;
                    case AttributeType.Resistance:
                        a.baseValue = e.resistance;
                        break;
                    case AttributeType.Vitality:
                        a.baseValue = e.vitality;
                        break;
                    case AttributeType.Speed:
                        a.baseValue = e.speed;
                        break;
                    case AttributeType.Accuracy:
                        a.baseValue = e.accuracy;
                        break;
                    case AttributeType.Crit:
                        a.baseValue = e.crit;
                        break;

                    // School Multipliers
                    case AttributeType.HealingMultiplier:
                        a.baseValue = e.healingMultiplier;
                        break;
                    case AttributeType.PhysicalMultiplier:
                        a.baseValue = e.physicalMultiplier;
                        break;
                    case AttributeType.FireMultiplier:
                        a.baseValue = e.fireMultiplier;
                        break;
                    case AttributeType.IceMultiplier:
                        a.baseValue = e.iceMultiplier;
                        break;
                    case AttributeType.NatureMultiplier:
                        a.baseValue = e.natureMultiplier;
                        break;
                    case AttributeType.ArcaneMultiplier:
                        a.baseValue = e.arcaneMultiplier;
                        break;
                    case AttributeType.HolyMultiplier:
                        a.baseValue = e.holyMultiplier;
                        break;
                    case AttributeType.ShadowMultiplier:
                        a.baseValue = e.shadowMultiplier;
                        break;
                    case AttributeType.CritMultiplier:
                        a.baseValue = e.critMultiplier;
                        break;
                }
            }
        }
    }
}
