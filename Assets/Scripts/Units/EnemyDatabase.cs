using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public UnitObject targetDummy;
    public List<UnitObject> weakEnemies;
    public List<UnitObject> strongEnemies;
    public List<UnitObject> bosses;

    public UnitObject GenerateTargetDummy()
    {
        return targetDummy;
    }

    public List<UnitObject> GenerateWeakEnemy()
    {
        int i = Random.Range(0, weakEnemies.Count);

        List<UnitObject> weakEnemy = new List<UnitObject>
        {
            weakEnemies[i]
        };

        return weakEnemy;
    }

    public UnitObject GenerateStrongEnemy()
    {
        int i = Random.Range(0, strongEnemies.Count);

        return strongEnemies[i];
    }

    public UnitObject GenerateBoss()
    {
        int i = Random.Range(0, bosses.Count);

        return bosses[i];
    }
}
