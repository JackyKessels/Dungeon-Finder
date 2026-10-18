using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class BattlePhaseController : MonoBehaviour
{
    private Enemy enemy;
    private List<BattlePhase> phases;
    private int currentPhase;

    public void Initialize(Unit unit)
    {
        enemy = unit as Enemy;

        if (enemy == null)
        {
            Debug.LogError("BattlePhaseController used on non-enemy unit!");
            return;
        }

        phases = enemy.enemyObject.phases;

        if (!HasValidPhases())
        {
            return;
        }

        phases = phases.OrderByDescending(p => p.healthTransition).ToList();

        currentPhase = 0;

        EnterPhase(phases[currentPhase]);
    }

    public bool UseRandomAbilities()
    {
        if (phases == null || phases.Count == 0)
        {
            return enemy.enemyObject.randomAbility;
        }

        return phases[currentPhase].randomAbility;
    }

    public List<AbilityBehavior> GetCurrentActiveAbilities()
    {
        if (phases == null || phases.Count == 0)
        {
            return enemy.enemyObject.abilities;
        }

        return phases[currentPhase].abilityBehaviors;
    }

    public List<PassiveAbility> GetCurrentPassiveAbilities()
    {
        if (phases == null || phases.Count == 0)
        {
            return enemy.enemyObject.passiveAbilities;
        }

        return phases[currentPhase].passiveAbilities;
    }

    public void OnHealthChanged()
    {
        if (phases == null || phases.Count == 0)
        {
            return;
        }

        var health = enemy.statsManager.GetNormalizedHealth();

        while (currentPhase < phases.Count - 1 && health <= phases[currentPhase + 1].healthTransition)
        {
            currentPhase++;
            EnterPhase(phases[currentPhase]);
        }
    }

    private bool HasValidPhases()
    {
        if (phases == null || phases.Count == 0)
        {
            return false;
        }

        var startingPhases = phases.Where(p => Mathf.Approximately(p.healthTransition, 1f)).ToList();

        if (startingPhases.Count == 0)
        {
            Debug.LogError($"{enemy.enemyObject.name} has no starting phase (HP = 1).");
            return false;
        }

        if (startingPhases.Count > 1)
        {
            Debug.LogWarning($"{enemy.enemyObject.name} has multiple starting phases. Using first.");
        }

        return true;
    }

    private void EnterPhase(BattlePhase phase)
    {
        ClearAbilities();
        enemy.spellbook.SetPhaseAbilities(phase);
        enemy.SetStartCooldowns();

        if (currentPhase == 0)
        {
            return;
        }

        enemy.TriggerStartBattleEvent();
        PlayPhaseTransition(phase);
    }

    private void ClearAbilities()
    {
        enemy.spellbook.abilityCollection.Clear();

        for (int i = enemy.spellbook.passives.Count - 1; i >= 0; i--)
        {
            enemy.spellbook.UnlearnPassive(enemy.spellbook.passives[i]);
        }
    }

    private void PlayPhaseTransition(BattlePhase phase)
    {
        if (phase.newSprite != null)
        {
            ObjectUtilities.ChangeSprite(enemy, phase.newSprite);
        }

        ObjectUtilities.CreateSpecialEffects(phase.transitionEffects, enemy, true);

        if (phase.transitionSound != null)
        {
            GameManager.Instance.audioSourceSFX.PlayOneShot(phase.transitionSound);
        }
    }
}