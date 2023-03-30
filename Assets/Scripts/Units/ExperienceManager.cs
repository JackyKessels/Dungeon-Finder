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
            NotificationObject.SendNotification("Level Up! Maximum Health of all party members increased by " + StatsManager.levelUpHealth + ".");

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
