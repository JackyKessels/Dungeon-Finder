using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Attack,
    None
}

public abstract class ActiveAbility : AbilityObject
{
    [Header("[ Base Functionality ]")]
    public float castTime = .75f;
    public int cooldown = 0;
    public int initialCooldown = 0;
    public bool endTurn = true;
    public int resetChance = 0;

    [Header("[ Additional Variables ]")]
    public WeaponRequirement weaponRequirement = WeaponRequirement.Nothing;
    public AudioClip soundEffect;
    public AnimationType animationType = AnimationType.Attack;

    [Header("[ Ability Sources ]")]
    public List<AbilitySource> allyAbilitySources;
    public List<AbilitySource> enemyAbilitySources;

    [Header("[ Effects ]")]
    [Tooltip("True = Triggers Self Effect for each target hit\nFalse = Just once")]
    public bool selfEffectsPerTarget = false;
    public List<EffectObject> selfEffects;
    public List<EffectObject> targetEffects;

    public override string GetDescription(TooltipObject tooltipInfo)
    {
        return base.GetDescription(tooltipInfo) + // Name + Level
               FormatWeaponRequirement() + // Weapon Requirement
               string.Format("\nCooldown: {0}", cooldown) + // Cooldown
               InitialCooldownTooltip() + // Initial Cooldown
               "\nEffect: " + ParseDescription(description, tooltipInfo); // Effect
    }

    public string GetNameAndDescription(TooltipObject tooltipObject)
    {
        string color = ColorDatabase.QualityColor(quality);

        string text = string.Format("<smallcaps><b><color={0}>{1}</color></b></smallcaps>", color, name);

        string effect = "\nEffect: " + ParseDescription(description, tooltipObject);

        return text + effect;
    }

    public string GetFlaskDescription(TooltipObject tooltipObject)
    {
        return string.Format("\nCooldown: {0}", cooldown) + // Cooldown
               InitialCooldownTooltip() + // Initial Cooldown
               "\nEffect: " + ParseDescription(description, tooltipObject); // Effect
    }

    private string FormatWeaponRequirement()
    {
        switch (weaponRequirement)
        {
            case WeaponRequirement.TwoHand:
                return string.Format("\nRequires: Two-hand");
            case WeaponRequirement.OneHand:
                return string.Format("\nRequires: One-hand");
            case WeaponRequirement.Shield:
                return string.Format("\nRequires: Shield");
            case WeaponRequirement.Relic:
                return string.Format("\nRequires: Relic");
            case WeaponRequirement.Nothing:
                return "";
            default:
                return "";
        }
    }

    private string InitialCooldownTooltip()
    {
        if (initialCooldown > 0)
            return string.Format("\nInitial Cooldown: {0}", initialCooldown);
        else
            return "";
    }

    public abstract void TriggerAbility(Unit caster, Unit target, int level, float effectiveness);

    public void AbilityActions(Unit caster, Unit target, int level, bool selfEffectPerTarget, float adjacentModifier, float abilityMultiplier)
    {
        ObjectUtilities.CreateSpecialEffects(targetSpecialEffects, target);

        if (caster.IsTargetEnemy(target))
        {
            // Trigger the ability's hostile sources to do damage/heal
            foreach (AbilitySource source in enemyAbilitySources)
            {
                source.TriggerSource(this, false, false, caster, target, level, adjacentModifier, true, abilityMultiplier, abilityType);
            }
        }
        else
        {
            // Trigger the ability's friendly sources to do damage/heal
            foreach (AbilitySource source in allyAbilitySources)
            {
                source.TriggerSource(this, false, false, caster, target, level, adjacentModifier, true, abilityMultiplier, abilityType);
            }
        }

        // Apply self effects to the caster
        if (selfEffectPerTarget)
        {
            if (selfEffects.Count > 0)
            {
                EffectManager.ApplyEffects(selfEffects, caster, caster, level, this);
            }
        }

        // Apply effects to the target
        if (targetEffects.Count > 0)
        {
            EffectManager.ApplyEffects(targetEffects, caster, target, level, this);
        }
    }

    public void SelfEffectOnly(Unit caster, int level, bool selfEffectPerTarget)
    {
        if (!selfEffectPerTarget)
        {
            if (selfEffects.Count > 0)
            {
                EffectManager.ApplyEffects(selfEffects, caster, caster, level, this);
            }
        }
    }


    public string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

        temp = AbilityTooltipHandler.ParseName(temp, "<name>", this);

        temp = AbilityTooltipHandler.ParseAbilityType(temp, "<primary>", "<assault>", "<protection>");

        // Friendly Ability Values <a1> <a2> etc.
        for (int i = 0; i < allyAbilitySources.Count; i++)
        {
            temp = AbilityTooltipHandler.DetermineAbilityValue(temp, string.Format("<a{0}>", i + 1), allyAbilitySources[i], tooltipInfo);
            temp = AbilityTooltipHandler.DetermineAbilityModifierBaseline(temp, string.Format("<a{0}modV>", i + 1), allyAbilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityModifierAttribute(temp, string.Format("<a{0}modP>", i + 1), allyAbilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityValue(temp, string.Format("<a{0}modB>", i + 1), allyAbilitySources[i].modifier.bonusAbilitySource.GetAbilitySource(), tooltipInfo);
            temp = AbilityTooltipHandler.DetermineAbilityConditionValue(temp, string.Format("<a{0}conV>", i + 1), allyAbilitySources[i].modifier);
        }

        // Hostile Ability Values <e1> <e2> etc.
        for (int i = 0; i < enemyAbilitySources.Count; i++)
        {
            temp = AbilityTooltipHandler.DetermineAbilityValue(temp, string.Format("<e{0}>", i + 1), enemyAbilitySources[i], tooltipInfo);
            temp = AbilityTooltipHandler.DetermineAbilityModifierBaseline(temp, string.Format("<e{0}modV>", i + 1), enemyAbilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityModifierAttribute(temp, string.Format("<e{0}modP>", i + 1), enemyAbilitySources[i].modifier);
            temp = AbilityTooltipHandler.DetermineAbilityValue(temp, string.Format("<e{0}modB>", i + 1), enemyAbilitySources[i].modifier.bonusAbilitySource.GetAbilitySource(), tooltipInfo);
            temp = AbilityTooltipHandler.DetermineAbilityConditionValue(temp, string.Format("<e{0}conV>", i + 1), enemyAbilitySources[i].modifier);
            temp = AbilityTooltipHandler.ParseEffectTooltips(temp, string.Format("e{0}modCE", i + 1), new List<EffectObject>() { enemyAbilitySources[i].modifier.conditionalEffect }, tooltipInfo);
        }

        if (this is TargetAbility t)
        {
            string check = "<adjmod>";

            if (temp.Contains(check))
            {
                temp = temp.Replace(check, "<color=#BFBFBF>{0}</color>");

                temp = string.Format(temp, t.adjacentModifier * 100);
            }
        }

        temp = AbilityTooltipHandler.ParseAllEffectTooltips(temp, tooltipInfo, selfEffects, targetEffects);

        temp = AbilityTooltipHandler.ColorAllSchools(temp);

        temp = AbilityTooltipHandler.ColorAllAttributes(temp);

        for (int i = 0; i < selfEffects.Count; i++)
        {
            if (selfEffects[i] is EffectConditionalTrigger selfTrigger)
            {
                temp = GetConditionalTriggerDescription(temp, string.Format("<selfTrigger{0}>", i + 1), selfTrigger, tooltipInfo);
            }
            else if (selfEffects[i] is EffectActivatePassive passive)
            {
                temp = GetActivatedPassiveDescription(temp, string.Format("<selfPassive{0}>", i + 1), passive, tooltipInfo);
            }
        }

        for (int i = 0; i < targetEffects.Count; i++)
        {
            if (targetEffects[i] is EffectConditionalTrigger targetTrigger)
            {
                temp = GetConditionalTriggerDescription(temp, string.Format("<targetTrigger{0}>", i + 1), targetTrigger, tooltipInfo);
            }
            else if (targetEffects[i] is EffectActivatePassive passive)
            {
                temp = GetActivatedPassiveDescription(temp, string.Format("<targetPassive{0}>", i + 1), passive, tooltipInfo);
            }
        }

        if (resetChance > 0)
            temp = AbilityTooltipHandler.ResetChance(temp, resetChance);

        if (!endTurn)
            temp = AbilityTooltipHandler.DoesNotEndTurn(temp);

        if (replacesAbility != null)
            temp = AbilityTooltipHandler.ReplacesAbility(temp, replacesAbility);

        temp = AbilityTooltipHandler.CriticalHit(temp, "<critical>");

        temp = AbilityTooltipHandler.InsertRed(temp);

        temp = AbilityTooltipHandler.InsertGeneric(temp);

        temp = AbilityTooltipHandler.InsertNonScaling(temp);

        temp = AbilityTooltipHandler.CurrentArmor(temp, "<currentarmor>", tooltipInfo);

        return temp;
    }

    public bool SuccessfulReset()
    {
        return Random.Range(0, 100) < resetChance;
    }

    private string GetConditionalTriggerDescription(string s, string check, EffectConditionalTrigger trigger, TooltipObject tooltipInfo)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, trigger.GetConditionalDescription(tooltipInfo));
        }

        return temp;
    }

    private string GetActivatedPassiveDescription(string s, string check, EffectActivatePassive passive, TooltipObject tooltipInfo)
    {
        string temp = s;

        if (temp.Contains(check))
        {
            temp = temp.Replace(check, passive.passiveAbility.ParseDescription(passive.passiveAbility.description, tooltipInfo));
        }

        return temp;
    }
}

[System.Serializable]
public class CastActiveAbility
{
    public ActiveAbility activeAbility;

    public float effectiveness = 1f;
    
    [Header("Target Ability Only")]
    public AbilityTargets abilityTarget;

    public void CastAbility(Unit caster, Unit target = null)
    {
        if (activeAbility == null)
        {
            Debug.Log("No active ability selected.");
            return;
        }

        if (activeAbility is TargetAbility t)
        {
            if (target == null)
            {
                Debug.Log("No target selected.");
                return;
            }

            (bool _, int level) = caster.spellbook.GetAbility(activeAbility);

            Active abilityToCast = new Active(t, level);

            List<Unit> targets = AbilityUtilities.GetAbilityTargets(abilityTarget, caster, target);

            foreach (Unit u in targets)
            {
                abilityToCast.Trigger(caster, u, effectiveness);
            }
        }

        if (activeAbility is InstantAbility i)
        {
            (bool _, int level) = caster.spellbook.GetAbility(activeAbility);

            Active abilityToCast = new Active(i, level);

            abilityToCast.Trigger(caster, target, effectiveness);
        }

        return;
    }
}

