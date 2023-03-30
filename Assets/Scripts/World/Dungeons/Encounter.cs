using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    public List<EnemyObject> enemyObjects;
    public bool randomOrder = true;
    public int weight = 1;

    private static Encounter WeightedEncounter(List<Encounter> encounters)
    {
        List<int> weights = new List<int>();

        foreach (Encounter encounter in encounters)
        {
            weights.Add(encounter.weight);
        }

        return encounters[GeneralUtilities.RandomWeighted(weights)];
    }

    public static List<EnemyObject> SetupUnitObjects(List<Encounter> encounters)
    {
        Encounter encounter = WeightedEncounter(encounters);

        if (encounter.randomOrder)
        {
            List<EnemyObject> mixedUnits = new List<EnemyObject>(encounter.enemyObjects);

            for (int i = 0; i < mixedUnits.Count; i++)
            {
                EnemyObject temp = mixedUnits[i];
                int randomIndex = Random.Range(i, mixedUnits.Count);
                mixedUnits[i] = mixedUnits[randomIndex];
                mixedUnits[randomIndex] = temp;
            }

            return mixedUnits;
        }

        return encounter.enemyObjects;
    }
}

[System.Serializable]
public class BossEncounter 
{
    public List<EnemyObject> enemyObjects;
}
