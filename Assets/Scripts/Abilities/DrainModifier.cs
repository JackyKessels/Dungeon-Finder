using UnityEngine;

[System.Serializable]
public class DrainModifier
{
    [Header("[ Drain Check ]")]
    public bool hasModifier = false;

    [Header("[ Drain Values ]")]
    public float fractionDrained;

    public void TriggerModifier(AbilityValue abilityValue)
    {
        if (!hasModifier)
        {
            return;
        }

        if (fractionDrained > 0 && abilityValue.value > 0)
        {
            Color color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(AbilitySchool.Healing));

            float drainedValue = abilityValue.value * fractionDrained;

            AbilityValue drainedAbilityValue = new AbilityValue(
                abilityValue.sourceAbility,
                abilityValue.sourceLevel,
                true,
                false,
                drainedValue,
                AbilitySchool.Healing,
                AbilityType.Passive,
                true,
                true,
                abilityValue.caster,
                abilityValue.caster,
                color,
                true,
                abilityValue.ignorePassives
                );

            drainedAbilityValue.Trigger();
        }
    }
}
