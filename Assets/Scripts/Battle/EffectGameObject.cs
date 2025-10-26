using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectGameObject : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI stacks;
    public TextMeshProUGUI duration;
    public TooltipObject tooltipObject;

    public void Setup(Effect effect)
    {
        icon.sprite = effect.IconOverride == null ? effect.effectObject.icon : effect.IconOverride;
        stacks.text = effect.stacks == 1 ? "" : effect.stacks.ToString();

        tooltipObject.effect = effect;
        tooltipObject.state = CurrentState.Battle;

        if (effect.effectObject.isBuff)
        {
            duration.color = ColorDatabase.ConvertStringToColor(ColorDatabase.Green);
        }
        else
        {
            duration.color = ColorDatabase.ConvertStringToColor(ColorDatabase.Red);
        }

        if (effect.effectObject.permanent)
        {
            duration.text = "";
        }
        else
        {
            duration.text = effect.duration.ToString();
        }
    }
}
