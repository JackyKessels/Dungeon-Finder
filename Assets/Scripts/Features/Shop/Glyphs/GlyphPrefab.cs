using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlyphPrefab : MonoBehaviour
{
    [SerializeField] Button _glyphButton;
    [SerializeField] TextMeshProUGUI _glyphName;
    [SerializeField] Image _glyphIcon;
    [SerializeField] GameObject _activeOverlay;

    [Space]

    [SerializeField] TextMeshProUGUI _currencyValue;
    [SerializeField] Image _currencyIcon;

    [Space]

    [SerializeField] GameObject _effectPrefab;
    [SerializeField] GameObject _heroesContainer;
    [SerializeField] GameObject _enemiesContainer;

    [Space]

    private GlyphObject glyphObject;
    
    public void Setup(GlyphObject glyphObject)
    {
        this.glyphObject = glyphObject;

        _activeOverlay.SetActive(false);

        _glyphName.SetText(glyphObject.name);
        _glyphIcon.sprite = glyphObject.icon;

        _currencyValue.SetText(glyphObject.cost.ToString());
        _currencyIcon.sprite = Currency.GetCurrencyIcon(glyphObject.currency, true);

        AddEffects();
    }

    private void AddEffects()
    {
        ObjectUtilities.ClearContainer(_heroesContainer);
        ObjectUtilities.ClearContainer(_enemiesContainer);

        foreach (EffectObject effectObject in glyphObject.heroEffects)
        {
            CreateEffect(effectObject, _heroesContainer);
        }

        foreach (EffectObject effectObject in glyphObject.enemyEffects)
        {
            CreateEffect(effectObject, _enemiesContainer);
        }
    }

    private void CreateEffect(EffectObject effectObject, GameObject container)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(_effectPrefab, container);

        TooltipIcon tooltipIcon = obj.GetComponent<TooltipIcon>();
        if (tooltipIcon != null)
        {
            SetupGlyphEffect(effectObject, tooltipIcon);
        }
    }

    private void SetupGlyphEffect(EffectObject effectObject, TooltipIcon tooltipIcon)
    {
        string tooltip = "";

        tooltip += effectObject.GetDescription(tooltipIcon.tooltipObject);
        tooltip += "\nDuration: " + EffectObject.DurationText(effectObject);

        tooltipIcon.tooltipObject.state = CurrentState.Values;
        tooltipIcon.Setup(effectObject.icon, tooltip);
    }

    public void SelectGlyph()
    {
        CurrencyHandler currencyHandler = GameManager.Instance.currencyHandler;

        Currency cost = new Currency(glyphObject.currency, glyphObject.cost);

        if (currencyHandler.CanBuy(cost))
        {
            currencyHandler.Buy(cost);

            SuccessfulPurchase();

            ShortMessage.SendMessage(Input.mousePosition, "Successful purchase!", 24, Color.green);
        }
        else
        {
            ShortMessage.SendMessage(Input.mousePosition, "Not enough currency.", 24, Color.red);
        }
    }

    private void SuccessfulPurchase()
    {
        GlyphManager.Instance.ApplyGlyph(glyphObject);

        //_activeOverlay.SetActive(true);
    }
}
