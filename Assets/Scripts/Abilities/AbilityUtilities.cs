﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AbilityTargets
{
    SelfOnly,
    Allies,
    Enemies,
    RandomAlly,
    RandomEnemy,
    All,
    AlliesNotSelf,
    Target,
    TwoRandomAllies,
    TwoRandomEnemies
}

public static class AbilityUtilities
{
    public static List<Unit> GetAbilityTargets(AbilityTargets targetType, Unit caster, Unit target = null)
    {
        TeamManager teamManager = TeamManager.Instance;

        List<Unit> targets = new List<Unit>();

        switch (targetType)
        {
            case AbilityTargets.SelfOnly:
                {
                    targets.Add(caster);
                }
                break;
            case AbilityTargets.Allies:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                }
                break;
            case AbilityTargets.Enemies:
                {
                    Team team = caster.isEnemy ? teamManager.heroes : teamManager.enemies;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                }
                break;
            case AbilityTargets.RandomAlly:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    if (team.LivingMembers.Count > 0)
                        targets.Add(GetRandomUnit(team));
                }
                break;
            case AbilityTargets.RandomEnemy:
                {
                    Team team = caster.isEnemy ? teamManager.heroes : teamManager.enemies;

                    if (team.LivingMembers.Count > 0)
                        targets.Add(GetRandomUnit(team));
                }
                break;
            case AbilityTargets.All:
                {
                    foreach (Unit unit in teamManager.heroes.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                    foreach (Unit unit in teamManager.enemies.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                }
                break;
            case AbilityTargets.AlliesNotSelf:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        if (unit != caster)
                            targets.Add(unit);
                    }
                }
                break;
            case AbilityTargets.Target:
                {
                    if (target != null)
                    {
                        targets.Add(target);
                    }
                    else
                    {
                        Debug.Log("[AbilityTargets.Target] selected but target is null.");
                    }
                }
                break;
            case AbilityTargets.TwoRandomAllies:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    targets.AddRange(GetRandomUnits(team, 2));
                }
                break;
            case AbilityTargets.TwoRandomEnemies:
                {
                    Team team = caster.isEnemy ? teamManager.heroes : teamManager.enemies;

                    targets.AddRange(GetRandomUnits(team, 2));
                }
                break;
        }

        return targets;
    }

    public static List<Unit> GetAdjacentUnits(Unit target)
    {
        TeamManager teamManager = TeamManager.Instance;

        List<Unit> adjacentUnits = new List<Unit>();

        Team team = target.isEnemy ? teamManager.enemies : teamManager.heroes;

        if (target.battleNumber == 0) // Hit member 0, 1 and 2
        {          
            if (team.GetUnitAlive(1) != null)           
                adjacentUnits.Add(team.GetUnitAlive(1));
            
            if (team.GetUnitAlive(2) != null)          
                adjacentUnits.Add(team.GetUnitAlive(2));           
        }
        else if (target.battleNumber == 1) // Hit member 0 and 1
        {
            if (team.GetUnitAlive(0) != null)          
                adjacentUnits.Add(team.GetUnitAlive(0));     
        }
        else if (target.battleNumber == 2) // Hit member 0 and 2
        {        
            if (team.GetUnitAlive(0) != null)
                adjacentUnits.Add(team.GetUnitAlive(0));         
        }

        return adjacentUnits;
    }

    public static Unit GetRandomUnit(Team team)
    {
        int randomTarget = Random.Range(0, team.LivingMembers.Count);

        return team.LivingMembers[randomTarget];
    }

    public static List<Unit> GetRandomUnits(Team team, int amount)
    {
        if (team.LivingMembers.Count > amount)
        {
            List<Unit> randomUnits = new List<Unit>();

            List<int> targetNumbers = Enumerable.Range(0, team.LivingMembers.Count).ToList();
            targetNumbers.Shuffle();

            for (int i = 0; i < amount; i++)
            {
                randomUnits.Add(team.LivingMembers[targetNumbers[i]]);
            }

            return randomUnits;
        }
        else
        {
            return team.LivingMembers;
        }
    }
}
