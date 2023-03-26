using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Spellbook
{
    public Unit unit;

    public List<Active> abilityCollection;
    public List<Active> itemAbilities;
    public Active[] activeSpellbook;

    public Active flaskAbility;

    public List<Passive> passives;

    public static readonly int mysticalAbilityCap = 1;
    public static readonly int abilityCollectionCap = 8;

    public Spellbook(Unit u)
    {
        unit = u;
        abilityCollection = new List<Active>();
        itemAbilities = new List<Active>();
        activeSpellbook = new Active[4];
        flaskAbility = new Active();
        passives = new List<Passive>();

        for (int i = 0; i < activeSpellbook.Length; i++)
        {
            activeSpellbook[i] = new Active(null, 0);
        }
    }

    public void SetDefaultAbilities(EnemyObject enemyObject)
    {
        foreach (AbilityBehavior a in enemyObject.abilities)
        {
            LearnAbility(new Active(a.ability, 1));
        }

        foreach (PassiveAbility p in enemyObject.passiveAbilities)
        {
            Passive passive = new Passive(p, 1);

            passive.ActivatePassive(unit);
        }
    }

    public void LearnAbility(Active active, bool isItemAbility = false)
    {
        List<Active> targetCollection = isItemAbility ? itemAbilities : abilityCollection;
        int collectionCap = isItemAbility ? abilityCollectionCap : abilityCollectionCap;

        AbilityObject toReplaceAbility = active.activeAbility.replacesAbility;

        if (toReplaceAbility != null)
        {
            for (int i = targetCollection.Count; i-- > 0;)
            {
                if (targetCollection[i].activeAbility == toReplaceAbility)
                {
                    active.SetReplacedAbility(targetCollection[i]);
                    active.level = targetCollection[i].level;
                    UnlearnAbility(targetCollection[i]);
                    SpellbookManager.Instance.CheckActiveValidity();
                }
            }
        }

        bool hasAbility = false;

        // If the spellbook already contains the learned spell then upgrade the level of that spell.
        hasAbility = UpgradeAbility(active, targetCollection, hasAbility);

        // If the spellbook does not contain the learned spell then add it to the collection.
        // Also add it to the active spells if there is room.
        if (hasAbility == false)
        {
            if (targetCollection.Count < collectionCap)
            {
                active.owner = unit;

                targetCollection.Add(active);

                if (unit is Hero)
                {
                    if (isItemAbility)
                    {
                        active.Initialize();
                    }
                    else
                    {
                        AddLearnedToActive(active, -1);
                        HeroManager.Instance.Refresh();
                    }
                }
            }
            else
            {
                Debug.Log("Too many abilities in the collection!");
            }
        }
    }

    private bool UpgradeAbility(Active active, List<Active> targetCollection, bool hasAbility)
    {
        for (int i = 0; i < targetCollection.Count; i++)
        {
            if (targetCollection[i].activeAbility == active.activeAbility)
            {
                targetCollection[i].level++;
                hasAbility = true;
            }
        }

        return hasAbility;
    }

    private bool HasAbility(Active active, List<Active> targetCollection)
    {
        bool hasAbility = false;

        for (int i = 0; i < targetCollection.Count; i++)
        {
            if (targetCollection[i].activeAbility == active.activeAbility)
            {
                hasAbility = true;
            }
        }

        return hasAbility;
    }

    public (bool hasAbility, int level) GetAbility(ActiveAbility activeAbility)
    {
        bool hasAbility = false;
        int abilityLevel = 1;

        for (int i = 0; i < abilityCollection.Count; i++)
        {
            if (abilityCollection[i].activeAbility == activeAbility)
            {
                hasAbility = true;
                abilityLevel = abilityCollection[i].level;
            }
        }

        return (hasAbility, abilityLevel);
    }

    public bool HasFlask()
    {
        if (flaskAbility.activeAbility == null)
            return false;
        else
            return true;
    }

    public void UnlearnAbility(Active active, bool isItemAbility = false)
    {
        List<Active> targetCollection = isItemAbility ? itemAbilities : abilityCollection;

        Active toRelearnAbility = active.GetReplacedAbility();

        if (toRelearnAbility != null)
        {
            toRelearnAbility.level = active.level;
            LearnAbility(toRelearnAbility);
            active.SetReplacedAbility(null);
        }

        targetCollection.Remove(active);

        HeroManager.Instance.Refresh();
    }

    public bool IsCollectionFull()
    {
        if (abilityCollection.Count >= abilityCollectionCap)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanAddToCollection(Active active)
    {
        if (!IsCollectionFull())
        {
            return true;
        }
        else
        {
            if (HasAbility(active, abilityCollection))
            {
                return true;
            }

            return false;
        }
    }

    public int LevelOfAbility(AbilityObject abilityObject)
    {
        int level = 0;

        // If the spellbook already contains the learned spell then upgrade the level of that spell.
        for (int i = 0; i < unit.spellbook.abilityCollection.Count; i++)
        {
            if (unit.spellbook.abilityCollection[i].activeAbility == abilityObject as ActiveAbility)
            {
                level = unit.spellbook.abilityCollection[i].level;
            }
        }

        return level;
    }

    public void AddLearnedToActive(Active active, int index)
    {
        // -1 means add to first empty slot
        if (index == -1)
        {
            for (int i = 0; i < activeSpellbook.Length; i++)
            {
                if (activeSpellbook[i].activeAbility == null && HasWeaponRequirement(active))
                {
                    activeSpellbook[i] = active;
                    activeSpellbook[i].Initialize();
                    return;
                }
            }
        }
        else
        {
            if (HasWeaponRequirement(active))
            {
                activeSpellbook[index] = active;
                activeSpellbook[index].Initialize();
            }
        }
    }

    public bool ItemAbilityLimit()
    {
        if (itemAbilities.Count > 4)
            return true;
        else
            return false;
    }

    public bool HasWeaponRequirement(Active active)
    {
        Hero hero = (unit as Hero);

        if (hero.equippedWeapons.Contains(active.activeAbility.weaponRequirement))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ValidActiveAbility(Active drag, Active target)
    {
        // Drag Uncommon = Good
        // Drag Mystical + Target Empty + Count >= Cap = Bad
        // Drag Mystical + Target Empty + Count < Cap = Good
        // Drag Mystical + Target Mystical = Good
        // Drag Mystical + Target Uncommon + Count >= Cap = Bad
        // Drag Mystical + Target Uncommon + Count < Cap = Good

        if (drag.activeAbility.quality != Quality.Mystical)
        {
            return true;
        }
        else
        {
            if (IsActiveOfQuality(target, Quality.Mystical))
            {
                return true;
            }
            else
            {
                if (GetMysticalAbilitiesCount() >= mysticalAbilityCap)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }          
        }
    }

    private bool IsActiveOfQuality(Active active, Quality quality)
    {
        // Active is empty
        if (active.activeAbility == null)
        {
            return false;
        }
        // Active is not empty
        else
        {
            if (active.activeAbility.quality == quality)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public int GetMysticalAbilitiesCount()
    {
        int total = 0;

        for (int i = 0; i < activeSpellbook.Length; i++)
        {
            if (activeSpellbook[i].activeAbility && activeSpellbook[i].activeAbility.quality == Quality.Mystical)
                total++;
        }

        return total;
    }

    public Active FindItemAbility(ActiveAbility activeAbility)
    {
        Active foundAbility = null;

        for (int i = 0; i < itemAbilities.Count; i++)
        {
            if (itemAbilities[i].activeAbility == activeAbility)
                foundAbility = itemAbilities[i];
        }

        return foundAbility;
    }

    public Active FindCollectionAbility(ActiveAbility activeAbility)
    {
        Active foundAbility = null;

        for (int i = 0; i < abilityCollection.Count; i++)
        {
            if (abilityCollection[i].activeAbility == activeAbility)
                foundAbility = abilityCollection[i];
        }

        return foundAbility;
    }

    public void AddPassive(Passive passive)
    {
        passives.Add(passive);

        if (unit is Hero h)
            SpellbookManager.Instance.Setup(h);
    }

    public void RemovePassive(Passive passive)
    {
        RemovePassiveFromList(passive);

        if (unit is Hero h)
            SpellbookManager.Instance.Setup(h);
    }

    private void RemovePassiveFromList(Passive passive)
    {
        for (int i = passives.Count; i-- > 0;)
        {
            if (passives[i].passiveAbility == passive.passiveAbility && passives[i].level == passive.level)
            {
                passives.RemoveAt(i);
                return;
            }
        }
    }

    public int GetPassiveLevel(PassiveAbility passiveAbility)
    {
        int level = 0;

        for (int i = 0; i < passives.Count; i++)
        {
            if (passives[i].passiveAbility == passiveAbility)
                level = passives[i].level;
        }

        return level;
    }

    public void SetActiveSpellbook()
    {
        for (int i = 0; (i < 4) && (i < abilityCollection.Count); i++)
        {
            activeSpellbook[i] = abilityCollection[i];

            activeSpellbook[i].Initialize();
        }
    }

    public void CooldownAbilities()
    {
        foreach (Active a in activeSpellbook)
        {
            a.CoolDown(1);
        }

        foreach (Active a in itemAbilities)
        {
            a.CoolDown(1);
        }

        if (flaskAbility != null)
        {
            flaskAbility.CoolDown(1);
        }
    }

    public void SetCooldowns()
    {
        foreach (Active ability in activeSpellbook)
        {
            if (ability != null && ability.activeAbility != null)
            {
                ability.currentCooldown = ability.activeAbility.initialCooldown;
            }
            
        }

        foreach (Active ability in itemAbilities)
        {
            if (ability != null && ability.activeAbility != null)
            {
                ability.currentCooldown = ability.activeAbility.initialCooldown;
            }
        }

        if (flaskAbility != null && flaskAbility.activeAbility != null)
        {
            flaskAbility.currentCooldown = flaskAbility.activeAbility.initialCooldown;
        }      
    }

    // Save & Load System

    public List<int> ConvertCollectionToIDs()
    {
        List<int> abilityList = new List<int>();

        foreach (Active active in abilityCollection)
        {
            abilityList.Add(active.activeAbility.id);
        }

        return abilityList;
    }

    public void AddAbilitiesToCollection(List<int> abilityIDs, List<int> abilityLevels)
    {
        DatabaseHandler databaseHandler = DatabaseHandler.Instance;

        abilityCollection = new List<Active>();

        for (int i = 0; i < abilityIDs.Count; i++)
        {
            ActiveAbility active = databaseHandler.abilityDatabase.abilityObjects[abilityIDs[i]] as ActiveAbility;

            LearnAbility(new Active(active, abilityLevels[i]));
        }
    }

    public List<int> ConvertCollectionToLevels()
    {
        List<int> levelList = new List<int>();

        foreach (Active active in abilityCollection)
        {
            levelList.Add(active.level);
        }

        return levelList;
    }

    public int[] ConvertActiveToIDs()
    {
        int[] active = new int[4];

        for (int i = 0; i < activeSpellbook.Length; i++)
        {
            if (activeSpellbook[i].activeAbility != null)
                active[i] = activeSpellbook[i].activeAbility.id;
            else
                active[i] = -1;
        }

        return active;
    }

    public void ChangeIDsToActive(int[] abilityIDs)
    {
        activeSpellbook = new Active[4];

        for (int i = 0; i < activeSpellbook.Length; i++)
        {
            if (abilityIDs[i] != -1)
                AddLearnedToActive(GetActive(abilityIDs[i]), i);
            else
                activeSpellbook[i] = new Active();
        }
    }

    private Active GetActive(int id)
    {
        foreach (Active active in abilityCollection)
        {
            if (id == active.activeAbility.id)
                return active;
        }

        foreach (Active active in itemAbilities)
        {
            if (id == active.activeAbility.id)
                return active;
        }

        return null;
    }

    public List<(int, int)> ConvertPassives()
    {
        List<(int, int)> completeList = new List<(int, int)>();

        foreach (Passive passive in passives)
        {
            completeList.Add((passive.passiveAbility.id, passive.level));
        }

        return completeList;
    }

    public void LearnPassives(List<(int, int)> passivesList)
    {
        for (int i = 0; i < passivesList.Count; i++)
        {
            (int id, int level) = passivesList[i];

            // Skip item passives
            if (id != -1)
            {
                PassiveAbility p = DatabaseHandler.Instance.abilityDatabase.abilityObjects[id] as PassiveAbility;

                Passive passive = new Passive(p, level);
                passive.ActivatePassive(unit);
            }
        }
    }
}
