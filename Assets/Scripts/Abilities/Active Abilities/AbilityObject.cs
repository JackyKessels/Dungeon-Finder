using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality
{
    Common,
    Mystical,
    Legendary 
}

public enum AbilityType
{
    Primary,
    Protection,
    Assault,
    Passive,
    Flask
}

public enum WeaponRequirement
{
    TwoHand,
    OneHand,
    Shield,
    Relic,
    Nothing
}

public abstract class AbilityObject : ScriptableObject, IHasTooltip, ITriggerSource
{
    [Header("[ General ]")]
    public int id = -1;
    public new string name = "New Ability";
    public Quality quality = Quality.Common;
    public AbilityType abilityType = AbilityType.Primary;
    [TextArea(10, 10)] public string description;
    public Sprite icon;
    public List<ParticleSystem> casterSpecialEffects;
    public List<ParticleSystem> targetSpecialEffects;
    public MissileObject missile;

    [Header("[ Extra ]")]
    public bool hasLevels = true;
    public bool sellable = true;
    public AbilityObject replacesAbility = null;
    
    public virtual string GetCompleteTooltip(TooltipObject tooltipInfo)
    {
        int currentLevel = tooltipInfo.GetAbilityLevel();

        var levelString = (currentLevel == 0 || !hasLevels) ? string.Empty : $"\nLevel: {currentLevel}";

        return AbilityTooltipHandler.GetName(name, quality) +
               AbilityTooltipHandler.ShowAbilityType(abilityType) +
               levelString;
    }

    public string DescriptionExcludeLevel()
    {
        string color = ColorDatabase.QualityColor(quality);

        return string.Format("<smallcaps><b><color={0}>{1}</color></b></smallcaps>{2}", color, name, AbilityTooltipHandler.ShowAbilityType(abilityType));
    }

    public ITriggerSource GetTriggerSource()
    {
        return this;
    }
}
