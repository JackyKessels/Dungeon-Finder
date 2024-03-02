using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExperienceManager
{
    private readonly int maxLevel = 50;
    public static readonly int pathPointLevelRequirement = 1;

    private static readonly float baseExperienceRequirement = 66.6666f;
    private static readonly float experienceCoefficient = 1.5f;

    private static readonly Dictionary<int, float> levelExperienceGains = new Dictionary<int, float>()
    {
        { 1, 1.00f },
        { 2, 1.00f },
        { 3, 1.00f },
        { 4, 0.75f },
        { 5, 0.50f },
        { 6, 0.25f },
    };

    public int currentLevel;
    public int currentExperience;
    public int totalExperienceGained;

    public ExperienceManager()
    {
        currentLevel = 1;
        currentExperience = 0;
        totalExperienceGained = 0;
    }

    public static int TotalExperienceToLevel(int level)
    {
        float totalExperience = 100f * ((1f - Mathf.Pow(experienceCoefficient, level - 1)) / -0.5f);
        Debug.Log(totalExperience);
        return (int)Mathf.Floor(totalExperience);
    }

    public static int ExperienceToNextLevel(int level)
    {
        return GeneralUtilities.RoundFloat(baseExperienceRequirement * (Mathf.Pow(experienceCoefficient, level)), 0);
    }

    public static int ExperienceToNextLevel()
    {
        int level = TeamManager.Instance.experienceManager.currentLevel;

        return ExperienceToNextLevel(level);
    }

    public void SetCurrentLevel(int level, bool showNotification = false)
    {
        int levelsToAdd = level - currentLevel;

        for (int i = 0; i < levelsToAdd; i++)
        {
            LevelUpTeam(showNotification);
        }
    }

    public static int EnemyExperience(int teamLevel, Enemy enemy, bool defeated)
    {
        int experience = enemy.enemyObject.experienceReward;
        int levelDifference = Mathf.Abs(teamLevel - enemy.Level);

        int totalExperience = 0;

        if (levelExperienceGains.TryGetValue(levelDifference, out float experienceFactor))
        {
            totalExperience = (int)(experience * experienceFactor);
        }

        if (defeated)
        {
            return totalExperience;
        }
        else
        {
            if (DungeonManager.Instance.IsCurrentLocationBoss())
            {
                float healthPercentage = 1f - enemy.statsManager.GetHealthPercentage();
                totalExperience = GeneralUtilities.RoundFloat(totalExperience * healthPercentage, 0);
                Debug.Log(totalExperience);
                return totalExperience;
            }
        }

        return 0;
    }

    public void GainExperience(int value, bool showNotification = false)
    {
        currentExperience += value;
        totalExperienceGained += value;

        while (currentExperience >= ExperienceToNextLevel(currentLevel))
        {
            int remainderExperience = currentExperience - ExperienceToNextLevel(currentLevel);
            currentExperience = remainderExperience;

            LevelUpTeam(showNotification);
        }
    }

    public void LevelUpTeam(bool showNotification)
    {
        if (showNotification)
            NotificationObject.CreateNotification("Level Up! Maximum Health of all party members increased by " + StatsManager.levelUpHealth + ".", 500, 300);

        if (currentLevel < maxLevel)
        {
            currentLevel++;

            //TownManager.Instance.BuildShops();

            foreach (Hero hero in TeamManager.Instance.heroes.Members)
            {
                LevelUp(hero);
            }
        }
        else
        {
            Debug.Log("Max level.");
        }
    }

    public void LoseExperience(int value)
    {
        if (currentExperience - value < 0)
        {
            totalExperienceGained -= currentExperience;
            currentExperience = 0;
        }
        else
        {
            currentExperience -= value;
            totalExperienceGained -= value;
        }
    }

    public void LevelUp(Hero hero)
    {
        hero.statsManager.LevelUp();

        if (currentLevel >= pathPointLevelRequirement)
        {
            hero.heroPathManager.points++;

            if (!hero.heroPathManager.unlocked)
                hero.heroPathManager.unlocked = true;
        }
    }
}
