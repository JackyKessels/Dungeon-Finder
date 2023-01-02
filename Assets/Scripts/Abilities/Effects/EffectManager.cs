using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private TeamManager teamManager;

    // In-battle effect list
    public List<Effect> effectsList = new List<Effect>();

    // Effects that will be applied at the start of the next battle
    public List<Effect> preBattleEffects = new List<Effect>();

    private Unit unit;

    private void Start()
    {
        teamManager = TeamManager.Instance;
        unit = GetComponent<Unit>();
    }

    public static void ApplyEffects(Unit caster, Unit target, List<EffectObject> effectsList, int level, AbilityObject sourceAbility)
    {
        if (effectsList.Count <= 0)
            return;

        foreach (EffectObject e in effectsList)
        {
            if (!target.statsManager.isDead)
            {
                ApplyEffect(e, caster, target, level, sourceAbility);
            }
        }
    }

    public static void ApplyEffect(EffectObject effectObject, Unit caster, Unit target, int level, AbilityObject sourceAbility)
    {
        Effect sameEffect = target.effectManager.GetSameEffect(effectObject);
        Effect applyEffect = new Effect(effectObject, 1, caster, target, level, sourceAbility);

        // Target has the same effect active already and the effect to be applied is stackable
        if (sameEffect != null && effectObject.stackable)
        {
            Effect stackedEffect = Effect.StackEffects(sameEffect, applyEffect);
            target.effectManager.OnApplication(stackedEffect);

            target.effectManager.OnExpiration(sameEffect);
        }
        else
        {
            target.effectManager.OnApplication(applyEffect);
        }
    }

    public void ApplyPreBattleEffects()
    {
        for (int i = preBattleEffects.Count; i-- > 0;)
        {
            unit.effectManager.OnApplication(preBattleEffects[i]);
        }

        preBattleEffects.Clear();
    }

    public void ApplyPreBattleEffect(EffectObject effectObject, Unit caster, int level)
    {
        bool hasEffect = false;

        for (int i = 0; i < preBattleEffects.Count; i++)
        {
            if (preBattleEffects[i].effectObject == effectObject)
                hasEffect = true;
        }

        if (!hasEffect)
        {
            Effect effect = new Effect(effectObject, 1, caster, caster, level, null);
            preBattleEffects.Add(effect);
        }
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
                effectsList.RemoveAt(i);
            }
        }
    }


    private bool NoEffects()
    {
        if (effectsList.Count == 0)
            return true;
        else
            return false;
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
                ApplyEffect(trigger.conditionalEffect, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level, effect.sourceAbility);

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


            bool meetCondition = false;

            foreach (EffectObject specificEffect in trigger.specificEffects)
            {
                if (target.effectManager.HasEffect(specificEffect))
                    meetCondition = true;
            }

            if (meetCondition)
            {
                ApplyEffect(trigger.conditionalEffect, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level, effect.sourceAbility);

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
    private bool HasImmunity(CrowdControlType ccType)
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            if ((effectsList[i].effectObject is EffectCrowdControlImmunity immunity) && (immunity.HasImmunityType(ccType)))
                return true;
        }

        return false;
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
                bonus += modifier.GetBonusMultiplier(modifiers[i].level);

                if (modifier.getConsumed)
                    OnExpiration(modifiers[i]);

                ObjectUtilities.CreateSpecialEffects(modifier.specialEffects, target);
            } 
        }

        return bonus;
    }

    private List<Effect> GetAbilityModifiers()
    {
        List<Effect> list = new List<Effect>();

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].effectObject is EffectAbilityModifier)
            {
                list.Add(effectsList[i]);
            }
        }

        return list;
    }

    public Effect GetHighestDamageTransfer()
    {
        if (NoEffects())
            return null;

        Effect highestEffect = (from e in GetDamageTransfers()
                                let maxPercentage = GetDamageTransfers().Max(m => (m.effectObject as EffectDamageTransfer).percentage)
                                where (e.effectObject as EffectDamageTransfer).percentage == maxPercentage
                                select e).FirstOrDefault();

        return highestEffect;
    }

    private List<Effect> GetDamageTransfers()
    {
        List<Effect> list = new List<Effect>();

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].effectObject is EffectDamageTransfer && !effectsList[i].caster.statsManager.isDead)
            {
                list.Add(effectsList[i]);
            }
        }

        return list;
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
            timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, e.target, timedAction.storedValue, timedAction.triggersPassives);

            CreateSpecialEffect(e.target, timedAction.specialEffects);
        }
        else if (timedAction.actionTargets == TimedActionTargets.Team)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.Adjacent)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in AbilityUtilities.GetAdjacentUnits(e.target))
            {
                timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomAlly)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(target, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.EnemyTeam)
        {
            Team team = e.target.isEnemy ? teamManager.heroes: teamManager.enemies;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomEnemy)
        {
            Team team = e.target.isEnemy ? teamManager.heroes : teamManager.enemies;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.sourceAbility, true, e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(target, timedAction.specialEffects);
            }
        }
    }

    public void OnApplication(Effect e)
    {
        // Check if effect has any effects that it removes and do so
        for (int i = 0; i < e.effectObject.removeEffects.Count; i++)
        {
            RemoveEffect(e.effectObject.removeEffects[i]);
        }

        if (e.effectObject is EffectCrowdControl cc)
        {
            if (HasImmunity(cc.type))
            {
                string text = "Immune";

                FCTData fctData = new FCTData(false, unit, text, Color.cyan);
                unit.fctHandler.AddToFCTQueue(fctData);

                return;
            }
        }
        else if (e.effectObject is EffectCrowdControlImmunity immune)
        {
            for (int i = 0; i < immune.immuneTypes.Count; i++)
            {
                BreakCrowdControl(immune.immuneTypes[i]);
            }
        }
        else if (e.effectObject is EffectOverTime)
        {
            foreach (TimedAction timedAction in e.timedActions)
            {
                if (timedAction.actionType == TimedActionType.OnApplication)
                {
                    TriggerSource(timedAction, e);
                }
            }
        }
        else if (e.effectObject is EffectAttributeModifier modifier)
        {
            CreateSpecialEffect(e.target, modifier.specialEffects);

            ModifyAttribute(e.target, modifier.attributeModified, e.storedModValue, modifier.isIncrease, true, modifier.modifierType);
        }
        else if (e.effectObject is EffectSpawnEnemy spawn)
        {
            TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawn.instant, spawn.specialEffects);

            if (spawn.two)
                TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawn.instant, spawn.specialEffects);
        }
        else if (e.effectObject is EffectActivatePassive passive)
        {
            CreateSpecialEffect(e.caster, passive.specialEffects);

            Passive effectPassive = new Passive(passive.passiveAbility, e.level);

            effectPassive.ActivatePassive(unit);

            e.storedPassive = effectPassive;
        }
        else if (e.effectObject is EffectDamageTransfer)
        {
            //effectsList.Remove(GetDamageTransfer());
        }

        // Effect Types below are not showing in the HUD
        if (e.effectObject is EffectConditionalTrigger)
        {
            TriggerConditionalEffect(e);
        }
        else if (e.effectObject is EffectCooldownReduction cdr)
        {
            cdr.ReduceCooldown(e.caster);
        }
        else
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



    public void OnExpiration(Effect e)
    {
        effectsList.Remove(e);

        if (e.effectObject is EffectOverTime)
        {
            foreach (TimedAction timedAction in e.timedActions)
            {
                if (timedAction.actionType == TimedActionType.OnExpiration)
                {
                    TriggerSource(timedAction, e);
                }
            }
        }
        else if (e.effectObject is EffectAttributeModifier modifier)
        {
            ModifyAttribute(e.target, modifier.attributeModified, e.storedModValue, modifier.isIncrease, false, modifier.modifierType);
        }
        else if (e.effectObject is EffectActivatePassive)
        {
            e.storedPassive.DeactivatePassive(unit);
        }
        else if (e.effectObject is EffectCrowdControl cc)
        {
            if (cc.addTauntImmune)
                ApplyEffect(GameAssets.i.tauntImmune, unit, unit, 1, e.sourceAbility);
        }
    }

    public static void CreateSpecialEffect(Unit target, List<ParticleSystem> specialEffects)
    {
        if (specialEffects != null && specialEffects.Count > 0)
        {
            foreach (ParticleSystem specialEffect in specialEffects)
            {
                Instantiate(specialEffect, target.transform.position, Quaternion.identity);
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

            if (type == AttributeType.Health)
            {
                float percentage = target.statsManager.GetHealthPercentage();

                target.statsManager.GetAttribute(type).bonusValue += valueIncrease;

                target.statsManager.SetHealthPercentage(percentage);
            }
            else
            {
                target.statsManager.GetAttribute(type).bonusValue += valueIncrease;
            }
        }
        else if (modifierType == ModifierType.Multiplier)
        {
            if (type == AttributeType.Health)
            {
                float percentage = target.statsManager.GetHealthPercentage();

                target.statsManager.GetAttribute(type).multiplier += newValue;

                target.statsManager.SetHealthPercentage(percentage);
            }
            else
            {
                target.statsManager.GetAttribute(type).multiplier += newValue;
            }
        }
    }

    public void EffectDurationHandler(ProcType procType)
    {
        // Trigger every active effect 
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (effectsList[i].procType == procType)
                OnActive(effectsList[i]);

            if (unit.statsManager.isDead)
                return;
        }

        ReduceDuration(procType);
    }
    
    private void ReduceDuration(ProcType procType)
    {
        // Reduce the duration and do expiration effect if duration is 0
        for (int i = effectsList.Count; i-- > 0;)
        {
            if (!effectsList[i].effectObject.permanent)
            {
                if (effectsList[i].procType == procType)
                {
                    effectsList[i].duration--;

                    if (effectsList[i].duration <= 0)
                    {
                        OnExpiration(effectsList[i]);
                    }
                }
            }

            if (unit.statsManager.isDead)           
                return;
            
        }
    }

    public void ExpireAll()
    {
        for (int i = effectsList.Count; i-- > 0;)
        {
            OnExpiration(effectsList[i]);
        }
    }

    public bool HasEffect(EffectObject effectObject)
    {
        bool hasEffect = false;

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].effectObject == effectObject)
                hasEffect = true;
        }

        return hasEffect;
    }
}
