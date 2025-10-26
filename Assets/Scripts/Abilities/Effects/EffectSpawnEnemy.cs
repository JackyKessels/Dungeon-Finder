using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Effect", menuName = "Unit/Effect Object/Spawn Enemy")]
public class EffectSpawnEnemy : EffectObject
{
    [Header("Spawn Enemy")]
    public List<Encounter> encounters = new List<Encounter>();
    public bool killCaster = false;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }

    public void SpawnEnemies(Unit caster)
    {
        if (killCaster)
        {
            caster.statsManager.InstantDeath();
        }

        var encounter = Encounter.WeightedEncounter(encounters);

        if (encounters == null)
        {
            return;
        }

        for (int i = 0; i < encounter.enemyObjects.Count; i++)
        {
            var spawnLevel = GeneralUtilities.GetUnitLevel(caster);

            TeamManager.Instance.SpawnEnemy(encounter.enemyObjects[i], spawnLevel, true, expirationSpecialEffects);
        }
    }
}
