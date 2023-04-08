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

        if (consequenceStructure.firstIcon == null)
        {
            firstImage.gameObject.SetActive(false);
        }
        else
        {
            firstImage.sprite = consequenceStructure.firstIcon;
        }

        if (consequenceStructure.secondIcon == null)
        {
            secondImage.gameObject.SetActive(false);
        }
        else
        { 
            secondImage.sprite = consequenceStructure.secondIcon; 
        }

        if (consequenceStructure.tooltip)
        {
            TooltipObject tooltip = firstImage.GetComponent<TooltipObject>();
            tooltip.state = CurrentState.Values;

            if (consequenceStructure.effect != null)
            {
                Effect effect = new Effect(consequenceStructure.effect, 1, TeamManager.Instance.heroes.LivingMembers[0], TeamManager.Instance.heroes.LivingMembers[0], 1, null);
                tooltip.effect = effect;
            }
            else if (consequenceStructure.itemObject != null)
            {
                tooltip.itemObject = consequenceStructure.itemObject;
            }
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
    public ItemObject itemObject;

    public ConsequenceStructure(string _text, Sprite _firstIcon, Sprite _secondIcon, bool _tooltip = false, EffectObject _effect = null, ItemObject _itemObject = null)
    {
        text = _text;
        firstIcon = _firstIcon;
        secondIcon = _secondIcon;
        tooltip = _tooltip;
        effect = _effect;
        itemObject = _itemObject;
    }
}
