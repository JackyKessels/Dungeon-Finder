using System.Collections;
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

    public static void ApplyEffect(EffectObject effectObject, Unit caster, Unit target, int level)
    {
        Effect effect = new Effect();
        effect.Setup(effectObject, caster, target, level);
        target.effectManager.OnApplication(effect);
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
            if (preBattleEffects[i].data == effectObject)
                hasEffect = true;
        }

        if (!hasEffect)
        {
            Effect effect = new Effect();
            effect.Setup(effectObject, caster, caster, level);
            preBattleEffects.Add(effect);
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
        EffectConditionalTrigger trigger = effect.data as EffectConditionalTrigger;

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
                ApplyEffect(trigger.conditionalEffect, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level);

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
                ApplyEffect(trigger.conditionalEffect, effect.caster, GetConditionalTarget(effect, trigger.effectTarget), effect.level);

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
            if ((effectsList[i].data is EffectCrowdControlImmunity immunity) && (immunity.HasImmunityType(ccType)))
                return true;
        }

        return false;
    }

    // Used from outside the EffectManager
    public void RemoveEffect(EffectObject removeEffect)
    {
        for (int i = effectsList.Count; i-- > 0;)         
        {
            if (effectsList[i].data == removeEffect)
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
            EffectAbilityModifier modifier = modifiers[i].data as EffectAbilityModifier;

            if (modifier.affectedAbility == AffectedAbility.AnyAbility ||
               (modifier.affectedAbility == AffectedAbility.TypedAbility && activeAbility.type == modifier.abilityType) ||
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
            if (effectsList[i].data is EffectAbilityModifier)
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
                                let maxPercentage = GetDamageTransfers().Max(m => (m.data as EffectDamageTransfer).percentage)
                                where (e.data as EffectDamageTransfer).percentage == maxPercentage
                                select e).FirstOrDefault();

        return highestEffect;
    }

    private List<Effect> GetDamageTransfers()
    {
        List<Effect> list = new List<Effect>();

        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].data is EffectDamageTransfer)
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
            if (effectsList[i].data is EffectCrowdControl cc && cc.type == ccType)
            {
                list.Add(effectsList[i]);
            }
        }

        return list;
    }

    private bool IsTaunt(Effect effect)
    {
        EffectCrowdControl cc = effect.data as EffectCrowdControl;
  
        return cc.type == CrowdControlType.Taunt ? true : false;
    }

    public void TriggerSource(TimedAction timedAction, Effect e)
    {
        if (timedAction.actionTargets == TimedActionTargets.Single)
        {
            timedAction.abilitySource.TriggerSource(e.caster, e.target, timedAction.storedValue, timedAction.triggersPassives);

            CreateSpecialEffect(e.target, timedAction.specialEffects);
        }
        else if (timedAction.actionTargets == TimedActionTargets.Team)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.Adjacent)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            foreach (Unit unit in AbilityUtilities.GetAdjacentUnits(e.target))
            {
                timedAction.abilitySource.TriggerSource(e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomAlly)
        {
            Team team = e.target.isEnemy ? teamManager.enemies : teamManager.heroes;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(target, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.EnemyTeam)
        {
            Team team = e.target.isEnemy ? teamManager.heroes: teamManager.enemies;

            foreach (Unit unit in team.LivingMembers)
            {
                timedAction.abilitySource.TriggerSource(e.caster, unit, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(unit, timedAction.specialEffects);
            }
        }
        else if (timedAction.actionTargets == TimedActionTargets.RandomEnemy)
        {
            Team team = e.target.isEnemy ? teamManager.heroes : teamManager.enemies;

            if (team.LivingMembers.Count > 0)
            {
                Unit target = AbilityUtilities.GetRandomUnit(team);

                timedAction.abilitySource.TriggerSource(e.caster, target, timedAction.storedValue, timedAction.triggersPassives);

                CreateSpecialEffect(target, timedAction.specialEffects);
            }
        }
    }

    public void OnApplication(Effect e)
    {
        // Check if effect has any effects that it removes and do so
        for (int i = 0; i < e.data.removeEffects.Count; i++)
        {
            RemoveEffect(e.data.removeEffects[i]);
        }

        if (e.data is EffectCrowdControl cc)
        {
            if (HasImmunity(cc.type))
            {
                string text = GeneralUtilities.GetCorrectCrowdControlText(cc.type) + " Immune";

                FCTData fctData = new FCTData(false, unit, text, Color.cyan);
                unit.fctHandler.AddToFCTQueue(fctData);

                return;
            }
        }
        else if (e.data is EffectCrowdControlImmunity immune)
        {
            for (int i = 0; i < immune.immuneTypes.Count; i++)
            {
                BreakCrowdControl(immune.immuneTypes[i]);
            }
        }
        else if (e.data is EffectOverTime)
        {
            foreach (TimedAction timedAction in e.timedActions)
            {
                timedAction.storedValue = timedAction.abilitySource.CalculateValue(e.caster, e.level, 1, 1);

                if (timedAction.actionType == TimedActionType.OnApplication)
                {
                    TriggerSource(timedAction, e);
                }
            }
        }
        else if (e.data is EffectAttributeModifier modifier)
        {
            CreateSpecialEffect(e.target, modifier.specialEffects);

            if (modifier.modifierType == ModifierType.Flat)
            {
                e.storedModValue = CalculateModifiedValue(e);
            }
            else
            {
                e.storedModValue = modifier.multiplier + modifier.multiplierPerLevel * (e.level - 1);
            }

            ModifyAttribute(e.target, modifier.attributeModified, e.storedModValue, modifier.isIncrease, true, modifier.modifierType);
        }
        else if (e.data is EffectSpawnEnemy spawn)
        {
            TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawn.instant, spawn.specialEffects);

            if (spawn.two)
                TeamManager.Instance.SpawnEnemy(spawn.enemyObject, spawn.instant, spawn.specialEffects);
        }
        else if (e.data is EffectActivatePassive passive)
        {
            CreateSpecialEffect(e.caster, passive.specialEffects);

            Passive effectPassive = new Passive(passive.passiveAbility, e.level);

            effectPassive.ActivatePassive(unit);

            e.storedPassive = effectPassive;
        }
        else if (e.data is EffectDamageTransfer)
        {
            //effectsList.Remove(GetDamageTransfer());
        }

        // Effect Types below are not showing in the HUD
        if (e.data is EffectConditionalTrigger)
        {
            TriggerConditionalEffect(e);
        }
        else if (e.data is EffectCooldownReduction cdr)
        {
            cdr.ReduceCooldown(e.caster.spellbook.activeSpellbook, e.caster);
        }
        else
        {
            effectsList.Add(e);
        }
    }
    
    public void OnActive(Effect e)
    {
        if (e.data is EffectOverTime)
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

        if (e.data is EffectOverTime)
        {
            foreach (TimedAction timedAction in e.timedActions)
            {
                if (timedAction.actionType == TimedActionType.OnExpiration)
                {
                    TriggerSource(timedAction, e);
                }
            }
        }
        else if (e.data is EffectAttributeModifier modifier)
        {
            ModifyAttribute(e.target, modifier.attributeModified, e.storedModValue, modifier.isIncrease, false, modifier.modifierType);
        }
        else if (e.data is EffectActivatePassive)
        {
            e.storedPassive.DeactivatePassive(unit);
        }
        else if (e.data is EffectCrowdControl cc)
        {
            if (cc.addTauntImmune)
                ApplyEffect(GameAssets.i.tauntImmune, unit, unit, 1);
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
            int valueIncrease = GeneralUtilities.RoundFloat(newValue);

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

                target.statsManager.GetAttribute(type).mulitplier += newValue;

                target.statsManager.SetHealthPercentage(percentage);
            }
            else
            {
                target.statsManager.GetAttribute(type).mulitplier += newValue;
            }
        }
    }

    public float CalculateModifiedValue(Effect e)
    {
        AbilitySource a = (e.data as EffectAttributeModifier).modifierSource;

        int totalBase = a.baseValue + a.levelBase * (e.level - 1);

        float totalScaling = a.scaling + a.levelScaling * (e.level - 1);

        int totalAttribute = e.caster.statsManager.GetAttributeValue((int)a.attributeType);      

        float totalValue = totalBase + totalScaling * totalAttribute;

        return totalValue;
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
            if (!effectsList[i].data.permanent)
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
            if (effectsList[i].data == effectObject)
                hasEffect = true;
        }

        return hasEffect;
    }
}
