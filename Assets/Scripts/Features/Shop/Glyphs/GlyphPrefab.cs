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
    
    public void Setup(GlyphObject glyphObject)
    {
        _activeOverlay.SetActive(false);

        _glyphName.SetText(glyphObject.name);
        _glyphIcon.sprite = glyphObject.icon;

        _currencyValue.SetText(glyphObject.cost.ToString());
        _currencyIcon.sprite = Currency.GetCurrencyIcon(glyphObject.currency, true);

        AddEffects(glyphObject);

    }

    private void AddEffects(GlyphObject glyphObject)
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
            tooltipIcon.Setup(effectObject);
        }
    }

    public void Buy()
    {
        Debug.Log("Glyph bought");
    }
}
