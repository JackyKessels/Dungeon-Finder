using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUD : MonoBehaviour
{
    public Unit unit;

    [Header("Inspector Variables")]
    public new TextMeshProUGUI name;
    public Image image;
    public TextMeshProUGUI level;
    public Slider healthBar;
    public Gradient healthGradient;
    public Image healthBarFill;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI armorValue;
    public TextMeshProUGUI resistanceValue;
    public TextMeshProUGUI vitalityValue;
    public GameObject effectsTracker;
    public GameObject effectPrefab;

    public void Setup(Unit u)
    {
        unit = u;

        name.text = u.isEnemy ? (u as Enemy).enemyObject.name : (u as Hero).heroObject.name;
        image.sprite = unit.icon;

        string currentLevel = u.isEnemy ? (u as Enemy).Level.ToString() : TeamManager.Instance.experienceManager.currentLevel.ToString();

        level.text = currentLevel;

        int maxHealth = unit.statsManager.GetAttributeValue(AttributeType.Health);

        healthBar.maxValue = maxHealth;
        healthBar.value = unit.statsManager.currentHealth;
        healthBarText.text = unit.statsManager.currentHealth + "/" + maxHealth;

        float armorReduction = GeneralUtilities.DefensiveReductionValue_League_Tweaked(unit.statsManager.GetAttributeValue(AttributeType.Armor));
        float resistanceReduction = GeneralUtilities.DefensiveReductionValue_League_Tweaked(unit.statsManager.GetAttributeValue(AttributeType.Resistance));

        armorValue.text = GeneralUtilities.RoundFloat(100 * (1 - armorReduction), 0).ToString() + "%";
        resistanceValue.text = GeneralUtilities.RoundFloat(100 * (1 - resistanceReduction), 0).ToString() + "%";
        vitalityValue.text = unit.statsManager.GetAttributeValue(AttributeType.Vitality).ToString() + "%";

        UpdateHealthBarColor(u);
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void UpdateBuffs()
    {
        ObjectUtilities.ClearContainer(effectsTracker);

        unit.effectManager.SortEffectDurations(unit.isEnemy);

        for (int i = 0; i < unit.effectManager.effectsList.Count; i++)
        {
            if (unit.effectManager.effectsList[i].duration > 0 && !unit.effectManager.effectsList[i].effectObject.hidden)
                CreateBuff(i);
        }
    }

    public void CreateBuff(int index)
    {
        Effect effect = unit.effectManager.effectsList[index];

        GameObject obj = ObjectUtilities.CreateSimplePrefab(effectPrefab, effectsTracker);

        EffectGameObject effectGameObject = obj.GetComponent<EffectGameObject>();
        effectGameObject.Setup(effect);
    }

    private void UpdateHealthBarColor(Unit unit)
    {
        float currentHealth = unit.statsManager.currentHealth;
        float maxHealth = unit.statsManager.GetAttributeValue(AttributeType.Health);

        float normalizedHealth = currentHealth / maxHealth;

        healthBarFill.color = healthGradient.Evaluate(normalizedHealth);
    }
}
