using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityUtilities
{
    public static List<Unit> GetInstantTargets(InstantTargets targetType, Unit caster)
    {
        TeamManager teamManager = TeamManager.Instance;

        List<Unit> targets = new List<Unit>();

        switch (targetType)
        {
            case InstantTargets.SelfOnly:
                {
                    targets.Add(caster);
                }
                break;
            case InstantTargets.Allies:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                }
                break;
            case InstantTargets.Enemies:
                {
                    Team team = caster.isEnemy ? teamManager.heroes : teamManager.enemies;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        targets.Add(unit);
                    }
                }
                break;
            case InstantTargets.RandomAlly:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    if (team.LivingMembers.Count > 0)   
                        targets.Add(GetRandomUnit(team));
                }
                break;
            case InstantTargets.RandomEnemy:
                {
                    Team team = caster.isEnemy ? teamManager.heroes : teamManager.enemies;

                    if (team.LivingMembers.Count > 0)
                        targets.Add(GetRandomUnit(team));
                }
                break;
            case InstantTargets.All:
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
            case InstantTargets.AlliesNotSelf:
                {
                    Team team = caster.isEnemy ? teamManager.enemies : teamManager.heroes;

                    foreach (Unit unit in team.LivingMembers)
                    {
                        if (unit != caster)
                            targets.Add(unit);
                    }
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
}
