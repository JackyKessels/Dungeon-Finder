using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    public List<EnemyObject> enemyObjects;
    public bool randomOrder = true;
    public int weight = 1;
    public Conversation conversation;

    private static Encounter WeightedEncounter(List<Encounter> encounters)
    {
        List<int> weights = new List<int>();

        foreach (Encounter encounter in encounters)
        {
            weights.Add(encounter.weight);
        }

        return encounters[GeneralUtilities.RandomWeighted(weights)];
    }

    public static void SetupUnitObjects(Location location, List<Encounter> encounters, int minimumLevel, int maximumLevel)
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

        location.enemyUnits = enemyObjects;
        location.encounter = encounter;
    }

    public static void SetupUnitObjects(Location location, Encounter encounter, int minimumLevel, int maximumLevel)
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

        location.enemyUnits = enemyObjects;
        location.encounter = encounter;
    }
}

[System.Serializable]
public class BossEncounter 
{
    public List<EnemyObject> enemyObjects;
    public int level;
}
