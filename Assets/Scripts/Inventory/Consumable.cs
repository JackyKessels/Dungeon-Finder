using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ConsumptionType
{
    Single,
    Party, 
    None
}

public abstract class Consumable : ItemObject
{
    public ConsumptionType consumptionType = ConsumptionType.Single;
    public bool usableOnDead = false;

    public abstract bool Consume(int i);

    public abstract string GetTooltip(TooltipObject tooltipInfo);

    public string HowToUseText()
    {
        return string.Format("<color={0}>\n\nRight-click to use item.</color>", ColorDatabase.Gray());
    }
}
