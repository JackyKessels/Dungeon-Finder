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

    public static List<(EnemyObject, int)> SetupUnitObjects(List<Encounter> encounters, int minimumLevel, int maximumLevel)
    {
        Encounter encounter = WeightedEncounter(encounters);

        List<(EnemyObject, int)> enemyObjects = new List<(EnemyObject, int)>();

        if (encounter.randomOrder)
        {
            encounter.enemyObjects.Shuffle();
        }

        for (int i = 0; i < encounter.enemyObjects.Count; i++)
        {
            int encounterLevel = Random.Range(minimumLevel, maximumLevel + 1);
            enemyObjects.Add((encounter.enemyObjects[i], encounterLevel));
        }

        return enemyObjects;
    }

    public static List<(EnemyObject, int)> SetupUnitObjects(Encounter encounter, int minimumLevel, int maximumLevel)
    {
        List<(EnemyObject, int)> enemyObjects = new List<(EnemyObject, int)>();

        if (encounter.randomOrder)
        {
            encounter.enemyObjects.Shuffle();
        }

        for (int i = 0; i < encounter.enemyObjects.Count; i++)
        {
            int encounterLevel = Random.Range(minimumLevel, maximumLevel + 1);
            enemyObjects.Add((encounter.enemyObjects[i], encounterLevel));
        }

        return enemyObjects;
    }
}

[System.Serializable]
public class BossEncounter 
{
    public List<EnemyObject> enemyObjects;
    public int level;
}
