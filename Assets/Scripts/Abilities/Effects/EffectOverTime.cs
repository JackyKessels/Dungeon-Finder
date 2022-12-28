using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Over Time Effect", menuName = "Unit/Effect Object/Over Time Effect")]
public class EffectOverTime : EffectObject
{
    [Header("[ Timed Actions ]")]
    public List<TimedAction> timedActions;

    public AbilitySource GetAbilitySource(TimedActionType actionType, int index)
    {
        List<TimedAction> relevantActions = new List<TimedAction>();

        foreach (TimedAction timedAction in timedActions)
        {
            if (timedAction.actionType == actionType)
            {
                relevantActions.Add(timedAction);
            }
        }

        if (index <= relevantActions.Count - 1)
        {
            return relevantActions[index].abilitySource;
        }
        else
        {
            return null;
        }
    }

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        for (int i = 0; i < tooltipInfo.effect.timedActions.Count; i++)
        {
            AbilitySource source = tooltipInfo.effect.timedActions[i].abilitySource;

            string check = string.Format("<{0}>", i + 1);

            if (temp.Contains(check))
            {
                temp = temp.Replace(check, "<color=" + ColorDatabase.ScalingColor(source.attributeType) + ">{0}</color>");

                temp = string.Format(temp, GeneralUtilities.RoundFloat(tooltipInfo.effect.timedActions[i].storedValue, 0));
            }
        }

        temp = AbilityTooltipHandler.ColorAllSchools(temp);

        temp = AbilityTooltipHandler.InsertRed(temp);

        return temp;
    }
}

public enum TimedActionType
{
    OnApplication,
    OnActive,
    OnExpiration
}

public enum TimedActionTargets
{
    Single,
    Team,
    Adjacent,
    RandomAlly,
    EnemyTeam,
    RandomEnemy
}

[System.Serializable]
public class TimedAction
{
    public TimedActionType actionType;
    public TimedActionTargets actionTargets = TimedActionTargets.Single;
    public bool triggersPassives = true;
    public AbilitySource abilitySource;
    public List<ParticleSystem> specialEffects = new List<ParticleSystem>();

    [HideInInspector] public float storedValue;

    public TimedAction(TimedAction copyTimedAction)
    {
        actionType = copyTimedAction.actionType;
        actionTargets = copyTimedAction.actionTargets;
        triggersPassives = copyTimedAction.triggersPassives;
        abilitySource = copyTimedAction.abilitySource;
        specialEffects = copyTimedAction.specialEffects;
    }

    public static TimedAction StackTimedActions(TimedAction sameTimedAction, TimedAction applyTimedAction)
    {
        // Missing a timed action
        if (sameTimedAction == null || applyTimedAction == null)
            return null;

        TimedAction newTimedAction = new TimedAction(sameTimedAction);
        newTimedAction.storedValue = sameTimedAction.storedValue + applyTimedAction.storedValue;

        return newTimedAction;
    }
}