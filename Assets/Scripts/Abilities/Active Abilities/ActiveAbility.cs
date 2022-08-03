﻿using System.Collections;
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
               "\nEffect: " + ParseDescription(description, tooltipInfo); // Effect
    }

    public string FormatWeaponRequirement()
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

    public abstract void Initialize(GameObject obj, Active active);
    public abstract void TriggerAbility(int level);

    public string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        string temp = s;

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
            temp = AbilityTooltipHandler.ParseEffectTooltips(tooltipInfo, new List<EffectObject>() { enemyAbilitySources[i].modifier.conditionalEffect }, temp, string.Format("e{0}modCE", i + 1));
        }

        if (this is TargetAbility t)
        {
            string check = "<adjmod>";

            if (temp.Contains(check))
            {
                temp = temp.Replace(check, "<color=#BFBFBF>{0}</color>");

                temp = string.Format(temp, t.adjacentReduction * 100);
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
