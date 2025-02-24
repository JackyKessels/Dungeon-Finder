
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryPanel : MonoBehaviour
{
    public Image enemyIcon;
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI enemyLevel;

    public GameObject activeAbilityContainer;
    public GameObject passiveAbilityContainer;

    public GameObject bestiaryAbilityPrefab;
    public GameObject tooltipIconPrefab;

    public void Setup(Enemy enemy)
    {
        enemyIcon.sprite = enemy.enemyObject.icon;
        enemyName.text = enemy.enemyObject.name;
        enemyLevel.text = enemy.Level.ToString();

        ObjectUtilities.ClearContainer(activeAbilityContainer);

        CreateActiveAbilities(enemy);

        ObjectUtilities.ClearContainer(passiveAbilityContainer);

        CreatePassiveAbilities(enemy);
    }

    private void CreateActiveAbilities(Enemy enemy)
    {
        foreach (var abilityBehavior in enemy.enemyObject.abilities)
        {
            var obj = ObjectUtilities.CreateSimplePrefab(bestiaryAbilityPrefab, activeAbilityContainer);
            var bestiaryAbility = obj.GetComponent<BestiaryAbility>();
            bestiaryAbility.Setup(abilityBehavior, enemy);
        }
    }

    private void CreatePassiveAbilities(Enemy enemy)
    {
        foreach (var passive in enemy.enemyObject.passiveAbilities)
        {
            var tooltip = ObjectUtilities.CreateTooltipIcon(passiveAbilityContainer);
            tooltip.Setup(passive, enemy.Level);
            tooltip.tooltipObject.state = CurrentState.Battle;
            tooltip.tooltipObject.source = enemy;
        }
    }
}
