using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexItem : MonoBehaviour
{
    public TooltipIcon tooltipIcon;

    public void Setup(bool discovered, ItemObject itemObject, int level)
    {
        if (discovered)
        {
            tooltipIcon.Setup(itemObject, level);
        }
        else
        {
            tooltipIcon.Setup(GameAssets.i.unknownIcon, "This item has not been discovered yet.");
        }
    }
}
