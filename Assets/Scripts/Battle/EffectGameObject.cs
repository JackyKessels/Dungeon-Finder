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
            duration.color = new Color(0f, 1f, 0f);
        }
        else
        {
            duration.color = new Color(1f, 0f, 0f);
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
