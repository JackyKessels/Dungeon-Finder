using TMPro;
using UnityEngine;

public class BestiaryAbility : MonoBehaviour
{
    public TooltipIcon tooltipIcon;
    public TextMeshProUGUI targetingDescription;

    public void Setup(AbilityBehavior abilityBehavior, Enemy enemy)
    {
        tooltipIcon.Setup(abilityBehavior.ability, enemy.Level);
        tooltipIcon.tooltipObject.source = enemy;
        tooltipIcon.tooltipObject.state = CurrentState.Battle;
        targetingDescription.text = GetAbilityTargetingDescription(abilityBehavior);
    }

    private string GetAbilityTargetingDescription(AbilityBehavior abilityBehavior)
    {
        if (abilityBehavior.ability is InstantAbility)
        {
            return GetInstantAbilityDescription(abilityBehavior);
        }

        if (abilityBehavior.ability is TargetAbility)
        {
            return GetTargetAbilityDescription(abilityBehavior);
        }

        return "";
    }

    private string GetInstantAbilityDescription(AbilityBehavior abilityBehavior)
    {
        var instantAbility = abilityBehavior.ability as InstantAbility;

        switch (instantAbility.abilityTargets)
        {
            case AbilityTargets.SelfOnly:
                return "Targets self.";
            case AbilityTargets.Allies:
                return "Targets all allies.";
            case AbilityTargets.Enemies:
                return "Targets all enemies.";
            case AbilityTargets.RandomAlly:
                return "Targets a random ally.";
            case AbilityTargets.RandomEnemy:
                return "Targets a random enemy.";
            case AbilityTargets.All:
                return "Targets all enemies and allies.";
            case AbilityTargets.AlliesNotSelf:
                return "Targets all allies except self.";
            case AbilityTargets.TwoRandomAllies:
                return "Targets two random allies.";
            case AbilityTargets.TwoRandomEnemies:
                return "Targets two random enemies.";
        }

        return "";
    }

    private string GetTargetAbilityDescription(AbilityBehavior abilityBehavior)
    {
        string attributeString = "";
        switch (abilityBehavior.targetAttribute)
        {
            case TargetAttribute.CurrentHealth:
                attributeString = "current Health";
                break;
            case TargetAttribute.MaximumHealth:
                attributeString = "maximum Health";
                break;
            case TargetAttribute.Power:
                attributeString = "Power";
                break;
            case TargetAttribute.Wisdom:
                attributeString = "Wisdom";
                break;
            case TargetAttribute.Armor:
                attributeString = "Armor";
                break;
            case TargetAttribute.Resistance:
                attributeString = "Resistance";
                break;
            case TargetAttribute.Speed:
                attributeString = "Speed";
                break;
            case TargetAttribute.Vitality:
                attributeString = "Vitality";
                break;
        }

        string thresholdString = "";
        switch (abilityBehavior.condition)
        {
            case CastCondition.Nothing:
                thresholdString = "Targets";
                break;
            case CastCondition.BelowHealthThreshold:
                thresholdString = $"While below {abilityBehavior.healthThreshold}% Health, targets";
                break;
            case CastCondition.AboveHealthThreshold:
                thresholdString = $"While above {abilityBehavior.healthThreshold}% Health, targets";
                break;
        }

        switch (abilityBehavior.target)
        {
            case TargetCondition.Random:
                return $"{thresholdString} a random enemy.";
            case TargetCondition.LowestAttribute:
                return $"{thresholdString} the enemy with the lowest {attributeString}.";
            case TargetCondition.HighestAttribute:
                return $"{thresholdString} the enemy with the highest {attributeString}.";
        }

        return "";
    }
}
