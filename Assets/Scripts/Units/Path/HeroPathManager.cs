using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroPathManager
{
    public Hero hero;

    public int points;

    public List<HeroPath> paths;

    public bool unlocked = false;

    public HeroPathManager(Hero h)
    {
        hero = h;

        points = 0;

        paths = new List<HeroPath>();

        foreach (HeroPathObject pathObject in hero.heroObject.paths)
        {
            paths.Add(new HeroPath(pathObject, 0));
        }
    }

    public void IncreaseHeroStats(HeroPath path)
    {
        foreach (Attribute attribute in path.path.attributes)
        {
            hero.statsManager.ModifyAttribute(attribute.attributeType, AttributeValue.bonusValue, attribute.baseValue);
        }
    }

    public void DecreaseHeroStats(HeroPath path)
    {
        foreach (Attribute attribute in path.path.attributes)
        {
            hero.statsManager.ModifyAttribute(attribute.attributeType, AttributeValue.bonusValue, attribute.baseValue);
        }
    }

    public List<AbilityObject> GetAllPathAbilities()
    {
        List<AbilityObject> allAbilities = new List<AbilityObject>();

        for (int i = 0; i < paths.Count; i++)
        {
            foreach (AbilityObject abilityObject in paths[i].path.activeAbilities)
            {
                if (abilityObject != null)
                    allAbilities.Add(abilityObject);
            }
        }

        return allAbilities;
    }

    public ActiveAbility GetRandomAbilityFromOtherPaths(HeroPathObject heroPathObject)
    {
        List<HeroPathObject> otherPaths = new List<HeroPathObject>();

        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].path != heroPathObject)
                otherPaths.Add(paths[i].path);
        }

        int randomPath = Random.Range(0, otherPaths.Count);

        int abilityCount = otherPaths[randomPath].activeAbilities.Count;

        int randomAbility = Random.Range(0, abilityCount);

        return otherPaths[randomPath].activeAbilities[randomAbility];
    }

    public void SelectStarterPath(int index)
    {
        foreach (Equipment equipment in paths[index].path.baseWeapons)
        {
            hero.ForceEquipItem(equipment);
        }

        //hero.spellbook.LearnAbility(new Active(paths[index].path.primaryAbility, 1));
        hero.spellbook.LearnAbility(new Active(paths[index].path.secondaryAbility, 1));

        if (paths[index].path.passiveAbility != null)
        {
            Passive passive = new Passive(paths[index].path.passiveAbility, 1);
            passive.ActivatePassive(hero);
        }

        hero.RestoreHealth();
    }

    // Save / Load
    public List<int> SavePathLevels()
    {
        List<int> levelList = new List<int>();

        foreach (HeroPath path in paths)
        {
            levelList.Add(path.level);
        }

        return levelList;
    }

    public List<bool[]> SavePathPassives()
    {
        int totalPaths = paths.Count;

        List<bool[]> allPathPassives = new List<bool[]>();

        for (int i = 0; i < totalPaths; i++)
        {
            allPathPassives.Add(paths[i].unlockedPassives);
        }

        return allPathPassives;
    }

    public void LoadPathLevels(List<int> pathLevels)
    {
        for (int i = 0; i < pathLevels.Count; i++)
        {
            paths[i].level = pathLevels[i];
            LoadPathStats(paths[i]);
        }
    }

    private void LoadPathStats(HeroPath path)
    {
        for (int i = 0; i < path.level; i++)
        {
            IncreaseHeroStats(path);
        }
    }

    public void LoadPathPassives(List<bool[]> pathPassives)
    {
        for (int i = 0; i < pathPassives.Count; i++)
        {
            SetPathPassives(pathPassives[i], paths[i].unlockedPassives);
            //ActivatePathPassives(paths[i]);
        }


    }

    private void SetPathPassives(bool[] loadedPassives, bool[] targetPassives)
    {
        int lengthCap = Mathf.Min(loadedPassives.Length, targetPassives.Length);

        for (int i = 0; i < lengthCap; i++)
        {
            targetPassives[i] = loadedPassives[i];
        }
    }

    // CHANGE THIS 
    //private void ActivatePathPassives(HeroPath path)
    //{
    //    for (int i = 0; i < path.path.passiveAbilities.Count; i++)
    //    {
    //        if (path.unlockedPassives[i])
    //            path.path.passiveAbilities[i]    .ActivatePassive(hero);
    //    }
    //}

    public int GetPoints()
    {
        return points;
    }

    public void LoadPoints(int amount)
    {
        points = amount;
    }
}

[System.Serializable]
public class HeroPath
{
    public readonly int[] checkpoints = new int[6] { 0, 2, 6, 12, 20, 30 };

    public HeroPathObject path;
    public int level;

    public int checkpointsReached;

    public bool[] unlockedPassives;

    public HeroPath(HeroPathObject pathObject, int pathLevel)
    {
        path = pathObject;
        level = pathLevel;

        checkpointsReached = 0;

        int numberOfPassives = pathObject.passiveAbilities.Count;

        unlockedPassives = new bool[numberOfPassives];

        for (int i = 0; i < unlockedPassives.Length; i++)
        {
            unlockedPassives[i] = false;
        }
    }

    public int HighestCheckpoint()
    {
        int highestCheckpoint = 0;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (level >= checkpoints[i])
                highestCheckpoint = i;
        }

        return highestCheckpoint;
    }

    public int UnlockedCheckpoints()
    {
        int unlockedCheckpoints = 0;

        for (int i = 0; i < unlockedPassives.Length; i++)
        {
            if (unlockedPassives[i])
                unlockedCheckpoints++;
        }

        return unlockedCheckpoints;
    }

    public bool CanUnlockPassive()
    {
        return UnlockedCheckpoints() < HighestCheckpoint();
    }
}

