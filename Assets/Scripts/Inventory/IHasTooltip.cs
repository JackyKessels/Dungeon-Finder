using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasTooltip
{ 
    string GetCompleteTooltip(TooltipObject tooltipInfo);
}
