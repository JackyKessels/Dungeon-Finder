using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instant Ability", menuName = "Unit/Ability Object/Instant Ability")]
public class InstantAbility : ActiveAbility
{
    [Header("Targeting")]
    public AbilityTargets instantTargets = AbilityTargets.SelfOnly;

    private InstantTriggerable instantTrigger;

    public override void Initialize(GameObject obj, Active active)
    {
        instantTrigger = obj.GetComponent<InstantTriggerable>();

        instantTrigger.abilityTarget = instantTargets;

        instantTrigger.active = active;
    }

    public override void TriggerAbility(int level)
    {
        instantTrigger.Trigger(null, level);
    }
}
