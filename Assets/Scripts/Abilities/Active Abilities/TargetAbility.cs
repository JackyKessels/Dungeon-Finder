using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TargetTargets
{
    Single,
    Adjacent
}

[CreateAssetMenu(fileName = "New Target Ability", menuName = "Unit/Ability Object/Target Ability")]
public class TargetAbility : ActiveAbility
{
    [Header("Targeting")]
    public bool targetsAllies = false;
    public bool targetsEnemies = true;

    public TargetTargets targetTargets = TargetTargets.Single;
    [Range(0, 1)]
    public float adjacentReduction = 0;

    private TargetTriggerable targetTrigger;

    private Unit target;

    public override void Initialize(GameObject obj, Active active)
    {
        Debug.Log("INIT " + name);
        targetTrigger = obj.GetComponent<TargetTriggerable>();

        targetTrigger.abilityTarget = targetTargets;
        targetTrigger.adjacentModifier = adjacentReduction;
        targetTrigger.active = active;
    }

    public override void TriggerAbility(int level)
    {
        Debug.Log("Trigger " + name);
        targetTrigger.Trigger(target, level);
    }

    public void CastAbility(Unit u, int level)
    {
        Debug.Log("Cast " + name);
        target = u;
        TriggerAbility(level);
    }
}
