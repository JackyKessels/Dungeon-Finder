using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Team
{
    private Unit[] members;

    // Only for enemies
    public List<Unit> killedMembers;

    public Team(int size)
    {
        members = new Unit[size];
    }

    public void ResetTeam()
    {
        for (int i = 0; i < members.Length; i++)
        {
            members[i] = null;
        }
    }

    public List<Unit> Members 
    {
        get
        {
            return (from Unit unit in members
                    where unit != null
                    select unit).ToList();
        }
    }

    public List<Unit> LivingMembers
    {
        get
        {
            return (from Unit unit in Members
                    where !unit.statsManager.isDead
                    select unit).ToList();
        }
    }

    public List<Unit> DeadMembers
    {
        get
        {
            return (from Unit unit in Members
                    where unit.statsManager.isDead
                    select unit).ToList();
        }
    }

    public Unit GetUnit(int battleNumber)
    {
        return members[battleNumber];
    }

    public Unit GetUnitAlive(int battleNumber)
    {
        if (members[battleNumber] != null && !members[battleNumber].statsManager.isDead)
            return members[battleNumber];
        else
            return null;
    }

    public void AddUnit(Unit unit, int position)
    {
        members[position] = unit;
    }

    public void Setup(UnitPosition[] positions, bool isEnemy)
    {
        killedMembers = new List<Unit>();

        foreach (UnitPosition position in positions)
        {
            position.shadow.SetActive(false);
        }

        for (int i = 0; (i < LivingMembers.Count); i++)
        {
            InitializeUnit(positions, isEnemy, i);
            positions[i].shadow.SetActive(true);
        }
    }

    public void InitializeUnit(UnitPosition[] positions, bool isEnemy, int i)
    {
        Unit unit = LivingMembers[i];

        // Show the shadow at the location of the unit
        positions[i].shadow.SetActive(true);
        positions[i].currentTurn.SetActive(false);

        // Set the unit at the correct position, offset by his sprite
        unit.transform.position = new Vector3(positions[i].transform.position.x,
                                              positions[i].transform.position.y + (unit.sprite.bounds.size.y * unit.unitRenderer.transform.localScale.y) / 2,
                                              positions[i].transform.position.z);

        unit.SetState(i, isEnemy);
        unit.battleNumber = i;

        if (isEnemy)
        {
            unit.spellbook.SetActiveSpellbook();
            (unit as Enemy).SetStartCooldowns();
        }
        else
        {
            unit.spellbook.SetCooldowns();
        }

        unit.hasTurn = true;
        QueueManager.Instance.AddToSpeedList(unit);
    }

    public void HealTeam(float factor, bool missingHealth, bool showWindow, string titleText = null)
    {
        List<string> messages = new();

        foreach (Unit unit in LivingMembers)
        {
            int restoreValue;

            if (missingHealth)
                restoreValue = (int)((unit.statsManager.GetAttributeValue(AttributeType.Health) - unit.statsManager.currentHealth) * factor);
            else
                restoreValue = (int)(unit.statsManager.GetAttributeValue((int)AttributeType.Health) * factor);

            unit.statsManager.currentHealth += restoreValue;

            if (unit.statsManager.currentHealth > unit.statsManager.GetAttributeValue((int)AttributeType.Health))
            {
                unit.statsManager.currentHealth = unit.statsManager.GetAttributeValue((int)AttributeType.Health);
            }

            messages.Add($"{unit.name} restored <color={ColorDatabase.SchoolColor(AbilitySchool.Healing)}>{restoreValue}</color> Health");
        }

        string fullMessage = GeneralUtilities.JoinString(messages, "\n", "\n");

        TextWindow.CreateTextWindow(titleText, fullMessage, 500, 400);
    }

    public void ReviveDeadMembers(bool fullRestore)
    {
        for (int i = DeadMembers.Count; i-- > 0;)
        {
            DeadMembers[i].ReviveUnit(fullRestore);
        }
    }

    public void FullRestoreTeam()
    {
        for (int i = 0; i < members.Length; i++)
        {
            members[i].ReviveUnit(true);
        }
    }

    public void SetInvulnerable(bool invulnerable)
    {
        foreach (Unit unit in members)
        {
            if (unit != null)
            {
                unit.statsManager.isInvulnerable = invulnerable;
            }
        }
    }

    public void ExpireEffects()
    {
        foreach (Unit unit in Members)
        {
            if (unit != null)
            {
                unit.effectManager.ExpireAll();
            }
        }
    }

    public void KillEnemy(Unit unit)
    {
        killedMembers.Add(unit);
        members[unit.battleNumber] = null;
    }

    public List<int> GetEmptyPositions()
    {
        List<int> emptyPositions = new List<int>();

        for (int i = 0; i < members.Length; i++)
        {
            if (members[i] == null)
                emptyPositions.Add(i);
        }

        return emptyPositions;
    }

    public int GetFirstEmptyPosition()
    {
        List<int> emptyPositions = GetEmptyPositions();

        return emptyPositions.Count == 0 ? -1 : emptyPositions[0];
    }

    public void TriggerStartBattle()
    {
        for (int i = 0; i < LivingMembers.Count; i++)
        {
            LivingMembers[i].TriggerStartBattleEvent();
        }
    }

    public void TriggerRoundStart()
    {
        for (int i = 0; i < LivingMembers.Count; i++)
        {
            LivingMembers[i].TriggerRoundStartEvent();
        }
    }

    public void ApplyPreBattleEffects()
    {
        for (int i = 0; i < LivingMembers.Count; i++)
        {
            LivingMembers[i].effectManager.ApplyPreBattleEffects();
        }
    }

    public Unit GetRandomUnit()
    {
        int randomUnit = Random.Range(0, LivingMembers.Count);

        return LivingMembers[randomUnit];
    }


    //public List<Unit> ActiveMembers 
    //{
    //    get
    //    {
    //        return GetActiveMembers();
    //    }
    //}

    //private List<Unit> GetActiveMembers()
    //{
    //    List<Unit> activeMembers = new List<Unit>();

    //    foreach (Unit unit in members)
    //    {
    //        if (unit != null)
    //        {
    //            activeMembers.Add(unit);
    //        }
    //    }

    //    return activeMembers;
    //}







    //public void AddMember(Unit unit)
    //{
    //    for (int i = 0; i < Members.Count; i++)
    //    {
    //        if (members[i] != null)
    //        {
    //            Debug.Log("Member " + i + " is " + unit.name);
    //        }
    //        else
    //        {
    //            members[i] = unit;
    //            break;
    //        }
    //    }
    //}

    //public int TotalActiveMembers()
    //{
    //    int totalActiveMembers = 0;

    //    foreach (Unit unit in members)
    //    {
    //        if (unit != null)
    //            totalActiveMembers++;
    //    }

    //    return totalActiveMembers;
    //}





    // True restores missing health of the team,
    // False restores a percentage of the health of the team.




    //public int GetFirstEmptyPosition()
    //{
    //    for (int i = 0; i < members.Length; i++)
    //    {
    //        if (members[i] == null)
    //            return i;
    //    }

    //    return -1;
    //}

    //public List<int> GetEmptyPositions()
    //{
    //    List<int> emptyPositions = new List<int>();

    //    for (int i = 0; i < members.Length; i++)
    //    {
    //        if (members[i] == null)
    //            emptyPositions.Add(i);
    //    }

    //    return emptyPositions;
    //}


}
