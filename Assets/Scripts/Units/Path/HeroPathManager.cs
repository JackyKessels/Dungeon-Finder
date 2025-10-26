using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class HeroPathManager
{
    public Hero hero;

    public int points;

    public List<HeroPath> paths;

    public bool unlocked = false;

    private const int MultiPathingThreshold = 2;

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

        if (otherPaths.Count > 0)
        {
            int randomPath = Random.Range(0, otherPaths.Count);

            int abilityCount = otherPaths[randomPath].activeAbilities.Count;

            int randomAbility = Random.Range(0, abilityCount);

            return otherPaths[randomPath].activeAbilities[randomAbility];
        }
        else if (paths.Count == 1)
        {
            return paths[0].path.GetRandomActiveAbilities(1).FirstOrDefault();
        }
        else
        {
            return null;
        }
    }

    public void SelectStarterPath(int index)
    {
        foreach (EquipmentObject equipmentObject in paths[index].path.baseWeapons)
        {
            Equipment equipment = new Equipment(equipmentObject, 1);
            hero.ForceEquipItem(equipment);
        }

        //hero.spellbook.LearnAbility(new Active(paths[index].path.primaryAbility, 1));
        hero.spellbook.LearnAbility(new Active(paths[index].path.secondaryAbility, 1));

        if (paths[index].path.passiveAbility != null)
        {
            Passive passive = new Passive(paths[index].path.passiveAbility, 1);

            hero.spellbook.LearnPassive(passive);
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

    public int GetPoints()
    {
        return points;
    }

    public void LoadPoints(int amount)
    {
        points = amount;
    }

    public void UpdatePathBlockers()
    {
        var totalLevel = paths.Sum(p => p.level);
        var unlockedMultiPathing = paths.Any(p => p.checkpointsReached >= MultiPathingThreshold);
        foreach (var path in paths)
        {
            var setBlocked = false;

            if (!unlockedMultiPathing && totalLevel > 0 && path.level == 0)
            {
                setBlocked = true;
            }

            path.pathGameObject.SetPathBlocker(setBlocked);     
        }
    }
}

[System.Serializable]
public class HeroPath
{
    public readonly int[] checkpoints = new int[6] { 0, 3, 6, 12, 20, 30 };

    public HeroPathObject path;
    public HeroPathGameObject pathGameObject;
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

    public int AvailableCheckpoints()
    {
        int availableCheckpoints = 0;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (level >= checkpoints[i])
            {
                availableCheckpoints = i;
            }
        }

        return availableCheckpoints;
    }

    public int SpentCheckpoints()
    {
        int spentCheckpoints = 0;

        for (int i = 0; i < unlockedPassives.Length; i++)
        {
            if (unlockedPassives[i])
                spentCheckpoints++;
        }

        return spentCheckpoints;
    }

    public bool CanUnlockPassive()
    {
        return SpentCheckpoints() < AvailableCheckpoints();
    }
}

