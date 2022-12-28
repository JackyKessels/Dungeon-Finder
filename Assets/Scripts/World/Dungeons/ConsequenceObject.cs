using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConsequenceObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image firstImage;
    [SerializeField] private Image secondImage;

    public void Setup(ConsequenceStructure consequenceStructure)
    {
        text.text = consequenceStructure.text;
        firstImage.sprite = consequenceStructure.firstIcon;

        if (consequenceStructure.secondIcon == null)
            secondImage.gameObject.SetActive(false);
        else
            secondImage.sprite = consequenceStructure.secondIcon;

        if (consequenceStructure.tooltip && consequenceStructure.effect != null)
        {
            TooltipObject tooltip = firstImage.gameObject.AddComponent<TooltipObject>();
            Effect effect = new Effect(consequenceStructure.effect, 1, TeamManager.Instance.heroes.LivingMembers[0], TeamManager.Instance.heroes.LivingMembers[0], 1, null);
            tooltip.effect = effect;
            tooltip.state = CurrentState.Values;
        }
    }
}

public class ConsequenceStructure
{
    public string text;
    public Sprite firstIcon;
    public Sprite secondIcon;
    public bool tooltip;
    public EffectObject effect;

    public ConsequenceStructure(string _text, Sprite _firstIcon, Sprite _secondIcon, bool _tooltip = false, EffectObject _effect = null)
    {
        text = _text;
        firstIcon = _firstIcon;
        secondIcon = _secondIcon;
        tooltip = _tooltip;
        effect = _effect;
    }
}
