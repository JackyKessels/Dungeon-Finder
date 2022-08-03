using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventHandler
{
    public static List<EnemyObject> GenerateEnemies(List<EnemyObject> enemyPool, int difficulty)
    {
        int currentDifficulty = 0;
        int totalEnemies = 0;

        List<EnemyObject> potentialEnemies = new List<EnemyObject>();

        int lowestDifficultyValue = enemyPool.Select(x => x.difficulty).Min();

        // Generate random enemies
        while (currentDifficulty < difficulty && totalEnemies < 3)
        {
            EnemyObject randomEnemy = enemyPool[Random.Range(0, enemyPool.Count)];
            if (currentDifficulty + randomEnemy.difficulty <= difficulty)
            {
                potentialEnemies.Add(randomEnemy);
                currentDifficulty += randomEnemy.difficulty;
                totalEnemies++;
            }

            // Break if there is no unit with a low enough value to fill
            if (lowestDifficultyValue > difficulty - currentDifficulty)
            {
                break;
            }
        }

        return potentialEnemies;
    }
}
