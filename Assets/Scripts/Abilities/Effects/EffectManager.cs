using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private TeamManager teamManager;

    // In-battle effect list
    public List<Effect> effectsList = new List<Effect>();

    public IEnumerable<Effect> PositiveEffects => effectsList.Where(e => e.effectObject.isBuff);
    public IEnumerable<Effect> NegativeEffects => effectsList.Where(e => !e.effectObject.isBuff);

    // Effects that will be applied at the start of the next battle
    public List<EffectObject> preBattleEffects = new List<EffectObject>();

    /// <summary>
    /// The unit that this effect manager belongs to.
    /// </summary>
    private Unit unit;

    private void Start()
    {
        teamManager = TeamManager.Instance;
        unit = GetComponent<Unit>();
    }

    public static void ApplyEffects(List<EffectObject> effectsList, Unit caster, Unit target, int level, AbilityObject sourceAbility)
    {
        if (effectsList.Count <= 0)
            return;

        foreach (EffectObject effectObject in effectsList)
        {
            if (!target.statsManager.isDead)
            {
                ApplyEffect(effectObject, caster, target, level, sourceAbility);
            }
        }
    }

    public static void ApplyEffect(EffectObject effectObject, Unit caster, Unit target, int level, AbilityObject sourceAbility)
    {
        for (int i = 0; i < effectObject.applyStacks; i++)
        {
            ApplyAndStackEffect(effectObject, caster, target, level, sourceAbility);
        }
    }

    private static void ApplyAndStackEffect(EffectObject effectObject, Unit caster, Unit target, int level, AbilityObject sourceAbility)
    {
        Effect sameEffect = target.effectManager.GetSameEffect(effectObject);
        Effect applyEffect = new Effect(effectObject, 1, caster, target, level, sourceAbility);

        if (applyEffect.effectObject.permanent)
            applyEffect.duration = 999;

        // Target has the same effect active already and the effect to be applied is stackable
        if (sameEffect != null && effectObject.stackable)
        {
            Effect stackedEffect = Effect.ApplyStacks(sameEffect, applyEffect);

            int index = target.effectManager.effectsList.IndexOf(sameEffect);
            if (index != -1)
            {
                target.effectManager.OnExpiration(sameEffect, removeEffectFromList: false, triggerEffect: sameEffect.effectObject is not EffectOverTime);

                target.effectManager.effectsList[index] = stackedEffect;

                target.effectManager.OnApplication(stackedEffect, false);

                TriggerStackEffect(stackedEffect);
            }
        }
        else
        {
            target.effectManager.OnApplication(applyEffect);
        }
    }

    private static void TriggerStackEffect(Effect effect)
    {
        var stackEffects = effect.effectObject.stackEffects;
        if (stackEffects.Count == 0)
        {
            return;
        }

        var consume = false;
        foreach (var stackEffect in stackEffects)
        {
            if (effect.stacks == stackEffect.stacksToTrigger)
            {
                if (stackEffect.effectObject != null)
                {
                    ApplyEffect(stackEffect.effectObject, effect.caster, effect.target, effect.level, effect.sourceAbility);
                }

                if (stackEffect.castActiveAbility.activeAbility != null)
                {
                    stackEffect.castActiveAbility.CastAbility(effect.caster, effect.target);
                }

                if (stackEffect.consumeEffect)
                {
                    consume = true;
                }
            }
        }

        if (consume)
        {
            effect.target.effectManager.OnExpiration(effect);
        }
    }

    public void DispelEffect(Effect effect)
    {
        OnExpiration(effect, triggerEffect: effect.effectObject is not EffectOverTime);
    }

    public void ApplyPreBattleEffects()
    {
        ApplyEffects(preBattleEffects, unit, unit, 1, null);

        preBattleEffects.Clear();
    }

    public void PreparePreBattleEffect(EffectObject effectObject)
    {
        preBattleEffects.Add(effectObject);
    }

    public static void RemoveAuras()
    {
        foreach (Unit unit in TeamManager.Instance.heroes.LivingMembers)
        {
            unit.effectManager.RemoveAura();
        }

        foreach (Unit unit in TeamManager.Instance.enemies.LivingMembers)
        {
            unit.effectManager.RemoveAura();
        }
    }

    private void RemoveAura()
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].effectObject.aura && effectsList[i].caster.statsManager.isDead)
            {
                OnExpiration(effectsList[i]);
            }
        }
    }

    public static void RemoveApplications(Effect effect)
    {
        foreach (Unit unit in TeamManager.Instance.heroes.LivingMembers)
        {
            if (unit != effect.target)
            {
                unit.effectManager.RemoveEffect(effect.effectObject);
            }
        }

        foreach (Unit unit in TeamManager.Instance.enemies.LivingMembers)
        {
            if (unit != effect.target)
            {
                unit.effectManager.RemoveEffect(effect.effectObject);
            }
        }
    }

    private bool NoEffects()
    {
        return effectsList.Count == 0;
    }

    private void TriggerConditionalEffect(Effect effect)
    {
        EffectConditionalTrigger trigger = effect.effectObject as EffectConditionalTrigger;

        if (trigger.conditionalEffectType == ConditionalEffectType.None)
            return;
        else if (trigger.conditionalEffectType == ConditionalEffectType.CrowdControl)
        {
            Unit target = GetConditionalTarget(effect, trigger.checkTarget);

            bool meetCondition = false;

            foreach (CrowdControlType ccType in trigger.ccTypes)
            {
                if (target.effectManager.IsCrowdControlled(ccType))
                    meetCondition = true;        
            }

            if (meetCondition)
            {
                ApplyEffects(trigger.appliedEffects, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level, effect.sourceAbility);

                if (trigger.consumeEffect)
                {
                    foreach (CrowdControlType ccType in trigger.ccTypes)
                    {
                        target.effectManager.BreakCrowdControl(ccType);
                    }
                }
            }
        }
        else if (trigger.conditionalEffectType == ConditionalEffectType.SpecificEffect)
        {
            Unit target = GetConditionalTarget(effect, trigger.checkTarget);

            bool meetCondition;

            if (trigger.hasAllEffects)
            {
                meetCondition = true;

                foreach (EffectObject specificEffect in trigger.specificEffects)
                {
                    if (!target.effectManager.HasEffect(specificEffect))
                    {
                        meetCondition = false;
                        break;
                    }
                }
            }
            else
            {
                meetCondition = false;

                foreach (EffectObject specificEffect in trigger.specificEffects)
                {
                    if (target.effectManager.HasEffect(specificEffect))
                        meetCondition = true;
                }
            }

            if (meetCondition)
            {
                ApplyEffects(trigger.appliedEffects, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level, effect.sourceAbility);

                if (trigger.consumeEffect)
                {
                    foreach (EffectObject effectObject in trigger.specificEffects)
                    {
                        target.effectManager.RemoveEffect(effectObject);
                    }
                }
            }
        }
    }

    private Unit GetConditionalTarget(Effect effect, ConditionalEffectTarget target)
    {
        if (target == ConditionalEffectTarget.Caster)
            return effect.caster;
        else if (target == ConditionalEffectTarget.Target)
            return effect.target;
        else
            return effect.target;
    }

    // Crowd Control Immunity
    private bool HasCrownControlImmunity(CrowdControlType ccType)
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            if ((effectsList[i].effectObject is EffectCrowdControlImmunity immunity) && (immunity.HasImmunityType(ccType)))
                return true;
        }

        return false;
    }

    public bool HasDamageImmunity()
    {
        return effectsList.Any(e => e.effectObject is EffectImmunity);
    }

    public Effect GetSameEffect(EffectObject effectObject)
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].effectObject == effectObject)
            {
                return effectsList[i];
            }
        }

        return null;
    }

    // Used from outside the EffectManager
    public void RemoveEffect(EffectObject removeEffect)
    {
        for (int i = effectsList.Count; i-- > 0;)         
        {
            if (effectsList[i].effectObject == removeEffect)
            {
                OnExpiration(effectsList[i]);
            }
        }
    }

    public void RemoveCharging()
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].effectObject is EffectChargeTarget)
            {
                OnExpiration(effectsList[i]);
            }
        }
    }

    // Collect all ability multipliers and expire those that need to be
    public float ApplyMultipliers(ActiveAbility activeAbility, Unit target)
    {
        List<Effect> modifiers = GetAbilityModifiers();

        if (modifiers.Count == 0)
            return 0;

        float bonus = 0;

        for (int i = modifiers.Count; i-- > 0;)
        {
            EffectAbilityModifier modifier = modifiers[i].effectObject as EffectAbilityModifier;

            if (modifier.affectedAbility == AffectedAbility.AnyAbility ||
               (modifier.affectedAbility == AffectedAbility.TypedAbility && activeAbility.abilityType == modifier.abilityType) ||
               (modifier.affectedAbility == AffectedAbility.SpecificAbility && modifier.IsValidAbility(activeAbility)))
            {
                bonus += modifiers[i].storedModValue;

                if (modifier.getConsumed)
                    OnExpiration(modifiers[i]);

                ObjectUtilities.CreateSpecialEffects(modifier.applicationSpecialEffects, target);
            } 
        }

        return bonus;
    }

    private List<Effect> GetAbilityModifiers()
    {
        return effectsList.Where(e => e.effectObject is EffectAbilityModifier).ToList();
    }

    public static Effect GetTransferToTarget(Unit caster, TransferType transferType)
    {
        List<Effect> effects = new List<Effect>();

        foreach (var hero in TeamManager.Instance.heroes.LivingMembers)
        {
            if (hero == caster)
            {
                continue;
            }

            var damageTransfer = hero.effectManager.GetTransfers(TransferDirection.CasterToTarget, transferType)
                .Where(t => t.caster == caster)
                .OrderByDescending(e => (e.effectObject as EffectDamageTransfer).percentage)
                .FirstOrDefault();

            if (damageTransfer != null)
            {
                effects.Add(damageTransfer);
            }
        }

        foreach (var enemy in TeamManager.Instance.enemies.LivingMembers)
        {
            if (enemy == caster)
            {
                continue;
            }

            var damageTransfer = enemy.effectManager.GetTransfers(TransferDirection.CasterToTarget, transferType)
                .Where(t => t.caster == caster)
                .OrderByDescending(e => (e.effectObject as EffectDamageTransfer).percentage)
                .FirstOrDefault();

            if (damageTransfer != null)
            {
                effects.Add(damageTransfer);
            }
        }

        if (effects.Count > 1)
        {
            Debug.Log("Not yet implemented for more than 1 effect.");
        }

        return effects.FirstOrDefault();
    }

    public Effect GetHighestTransfer(TransferDirection damageTransferDirection, TransferType transferType)
    {
        if (NoEffects())
            return null;

        Effect highestEffect = GetTransfers(damageTransferDirection, transferType)
                               .OrderByDescending(e => (e.effectObject as EffectDamageTransfer).percentage)
                               .FirstOrDefault();

        return highestEffect;
    }

    private List<Effect> GetTransfers(TransferDirection damageTransferDirection, TransferType transferType)
    {
        return effectsList.Where(e => e.effectObject is EffectDamageTransfer damageTransfer && 
                                      damageTransfer.direction == damageTransferDirection && 
                                      damageTransfer.transferType == transferType &&
                                      !e.caster.statsManager.isDead).ToList();
    }

    public Unit TauntedBy()
    {
        Effect taunt = GetCrowdControlEffectsOfType(CrowdControlType.Taunt).FindLast(IsTaunt);

        if (taunt == null)
        {
            return null;
        }
        else
        {
            if (taunt.caster.statsManager.isDead)
                return null;
            else
                return taunt.caster;
        }
    }

    public bool IsCrowdControlled(CrowdControlType ccType)
    {
        if (GetCrowdControlEffectsOfType(ccType).Count <= 0)
            return false;
        else
            return true;
    }

    public void BreakCrowdControl(CrowdControlType ccType)
    {
        List<Effect> crowdControlEffects = GetCrowdControlEffectsOfType(ccType);

        for (int i = crowdControlEffects.Count; i-- > 0;)
        {
            OnExpiration(crowdControlEffects[i]);
        }
    }

    private List<Effect> GetCrowdControlEffectsOfType(CrowdControlType ccType)
    {
        List<Effect> list = new List<Effect>();

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].effectObject is EffectCrowdControl cc && cc.type == ccType)
            {
                list.Add(effectsList[i]);
            }
        }

        return list;
    }

    private bool IsTaunt(Effect effect)
    {
        EffectCrowdControl cc = effect.effectObject as EffectCrowdControl;
  
        return cc.type == CrowdControlType.Taunt ? true : false;
    }

    public void TriggerSource(TimedAction timedAction, Effect e)
    {
        if (timedAction.actionTargets == TimedActionTargets.Single)
        {
            timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, e.target, timedAction.storedValue, timedAction.triggersPassives);

            ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, e.target, true);
        }
        else if (timedAction.actionTargets == TimedActionTargets.Team)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, unit, true);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.Adjacent)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in AbilityUtilities.GetAdjacentUnits(e.target))
            {
                timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, unit, true);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomAlly)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, target, true);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.EnemyTeam)
        {
            Team team = e.target.isEnemy ? teamManager.heroes: teamManager.enemies;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, unit, true);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomEnemy)
        {
            Team team = e.target.isEnemy ? teamManager.heroes : teamManager.enemies;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.effectObject, e.sourceAbility, e.level, false, true, e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                ObjectUtilities.CreateSpecialEffects(timedAction.specialEffects, target, true);
            }
        }
    }

    public void OnApplication(Effect e, bool addEffectToList = true)
    {
        // Check if effect has any effects that it removes and do so
        for (int i = 0; i < e.effectObject.removeEffects.Count; i++)
        {
            RemoveEffect(e.effectObject.removeEffects[i]);
        }

        ObjectUtilities.CreateSpecialEffects(e.effectObject.applicationSpecialEffects, e.target, true);

        switch (e.effectObject)
        {
            case EffectCrowdControl crowdControl:
                {
                    if (HasCrownControlImmunity(crowdControl.type))
                    {
                        string text = "Immune";

                        FCTData fctData = new FCTData(false, unit, text, Color.cyan);
                        unit.fctHandler.AddToFCTQueue(fctData);

                        return;
                    }

                    break;
                }
            case EffectCrowdControlImmunity crowdControlImmunity:
                {
                    for (int i = 0; i < crowdControlImmunity.immuneTypes.Count; i++)
                    {
                        BreakCrowdControl(crowdControlImmunity.immuneTypes[i]);
                    }

                    break;
                }
            case EffectOverTime overTime:
                {
                    foreach (TimedAction timedAction in e.timedActions)
                    {
                        if (timedAction.actionType == TimedActionType.OnApplication)
                        {
                            TriggerSource(timedAction, e);
                        }
                    }

                    break;
                }
            case EffectAttributeModifier modifier:
                {
                    ObjectUtilities.CreateSpecialEffects(modifier.applicationSpecialEffects, e.target, true);

                    ModifyAttribute(e.target, modifier.attributeModified, e.storedModValue, modifier.isIncrease, true, modifier.modifierType);

                    break;
                }
            case EffectSpawnEnemy spawn:
                {
                    var spawnLevel = spawn.level == 0 ? GeneralUtilities.GetUnitLevel(e.caster) : spawn.level;

                    TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawnLevel, spawn.instant, spawn.applicationSpecialEffects);

                    if (spawn.two)
                    {
                        TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawnLevel, spawn.instant, spawn.applicationSpecialEffects);
                    }

                    return;
                }
            case EffectActivatePassive passive:
                {
                    ObjectUtilities.CreateSpecialEffects(passive.applicationSpecialEffects, e.caster, true);

                    Passive effectPassive = new Passive(passive.passiveAbility, e.level);

                    unit.spellbook.LearnPassive(effectPassive);

                    e.storedPassive = effectPassive;

                    break;
                }
            case EffectDamageTransfer damageTransfer:
                {
                    break;
                }
            case EffectConditionalTrigger conditional:
                {
                    TriggerConditionalEffect(e);

                    return;
                }
            case EffectCooldownReduction cooldownReduction:
                {
                    cooldownReduction.ReduceCooldown(e.caster);

                    return;
                }
            case EffectSpriteChange spriteChange:
                {
                    spriteChange.ChangeSprite(unit);

                    break;
                }
            case EffectDispel dispel:
                {
                    dispel.DispelEffects(unit);

                    return;
                }
        }

        if (e.effectObject.unique)
        {
            RemoveApplications(e);
        }

        if (addEffectToList)
        {
            effectsList.Add(e);
        }
    }

    public void OnActive(Effect e)
    {
        if (e.effectObject is EffectOverTime)
        {
            foreach (TimedAction timedAction in e.timedActions)
            {
                if (timedAction.actionType == TimedActionType.OnActive)
                {
                    TriggerSource(timedAction, e);
                }
            }
        }
    }

    public void OnExpiration(Effect effect, bool removeEffectFromList = true, bool triggerEffect = true)
    {
        // Only remove the effect from the list one-by-one
        if (removeEffectFromList)
        {
            effectsList.Remove(effect);
        }

        if (!triggerEffect)
        {
            return;
        }
        
        ObjectUtilities.CreateSpecialEffects(effect.effectObject.expirationSpecialEffects, effect.target, true);
        
        switch (effect.effectObject)
        {
            case EffectOverTime overTime:
                {
                    foreach (TimedAction timedAction in effect.timedActions)
                    {
                        if (timedAction.actionType == TimedActionType.OnExpiration)
                        {
                            TriggerSource(timedAction, effect);
                        }
                    }

                    return;
                }
            case EffectAttributeModifier modifier:
                {
                    ModifyAttribute(effect.target, modifier.attributeModified, effect.storedModValue, modifier.isIncrease, false, modifier.modifierType);

                    return;
                }
            case EffectActivatePassive passive:
                {
                    unit.spellbook.UnlearnPassive(effect.storedPassive);

                    return;
                }
            case EffectCrowdControl crowdControl:
                {
                    if (crowdControl.addTypeImmunity)
                    {
                        if (crowdControl.type == CrowdControlType.Stun)
                        {
                            ApplyEffect(GameAssets.i.stunImmune, unit, unit, 1, effect.sourceAbility);
                        } 
                        else if (crowdControl.type == CrowdControlType.Taunt)
                        {
                            ApplyEffect(GameAssets.i.tauntImmune, unit, unit, 1, effect.sourceAbility);
                        }
                    }

                    return;
                }
        }
    }

    public void ModifyAttribute(Unit target, AttributeType type, float value, bool isIncrease, bool isApplication, ModifierType modifierType)
    {
        int mod1 = isIncrease == true ? 1 : -1;
        int mod2 = isApplication == true ? 1 : -1;

        float newValue = value * mod1 * mod2;

        if (modifierType == ModifierType.Flat)
        {
            int valueIncrease = GeneralUtilities.RoundFloat(newValue, 0);

            target.statsManager.ModifyAttribute(type, AttributeValue.bonusValue, valueIncrease);         
        }
        else if (modifierType == ModifierType.Multiplier)
        {
            target.statsManager.ModifyAttribute(type, AttributeValue.multiplier, newValue);
        }
    }

    public void EffectDurationHandler(ProcType procType)
    {
        // Trigger every active effect 
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].procType == procType)
            {
                OnActive(effectsList[i]);
            }

            if (unit.statsManager.isDead)
            {
                return;
            }
        }

        ReduceDuration(procType);
    }
    
    private void ReduceDuration(ProcType procType)
    {
        // Reduce the duration and do expiration effect if duration is 0
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].procType == procType)
            {
                bool expireEffect = false;

                if (!effectsList[i].effectObject.permanent)
                {
                    effectsList[i].duration--;

                    if (effectsList[i].duration <= 0)
                    {
                        expireEffect = true;
                    }
                }

                if (effectsList[i].effectObject.loseStacks > 0)
                {
                    Effect.DropStacks(effectsList[i]);
                    if (effectsList[i].stacks <= 0)
                    {
                        expireEffect = true;
                    }
                }

                if (expireEffect)
                {
                    OnExpiration(effectsList[i]);
                }
            }

            if (unit.statsManager.isDead)
            {
                return;
            }          
        }
    }

    public void ExpireAll()
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            OnExpiration(effectsList[i], true);
        }

        effectsList.Clear();
    }

    public bool HasEffect(EffectObject effectObject)
    {
        bool hasEffect = false;

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].effectObject == effectObject)
            {
                hasEffect = true;
            }    
        }

        return hasEffect;
    }

    public void SortEffectDurations(bool isEnemy)
    {
        if (isEnemy)
        {
            effectsList.Sort((e1, e2) => e1.duration.CompareTo(e2.duration));
        }
        else
        {
            effectsList.Sort((e1, e2) => e2.duration.CompareTo(e1.duration));
        }
    }
}
