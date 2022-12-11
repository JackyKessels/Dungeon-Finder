using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Item/Consumable/Food")]
public class Food : Consumable
{
    public int healthRestore;

    public EffectObject preBattleEffect;

    public override void Consume(int i)
    {
        switch (consumptionType)
        {
            case ConsumptionType.Single:
                {
                    ConsumeFood(TeamManager.Instance.heroes.LivingMembers[i]);
                }
                break;
            case ConsumptionType.Party:
                {
                    foreach (Unit unit in TeamManager.Instance.heroes.LivingMembers)
                    {
                        ConsumeFood(unit);
                    }
                }
                break;
            case ConsumptionType.None:
                break;
        }

        InventoryManager.Instance.UpdateCharacterAttributes(HeroManager.Instance.CurrentHero(), -1);
    }

    private void ConsumeFood(Unit unit)
    {
        if (healthRestore > 0)
        {
            AbilityValue abilityValue = new AbilityValue(null, false, healthRestore, AbilitySchool.Healing, AbilityType.Protection, true, true, unit, unit, Color.green, true, null);

            abilityValue.Trigger(true);
        }

        if (preBattleEffect != null)
        {
            unit.effectManager.ApplyPreBattleEffect(preBattleEffect, unit, 1);
        }
    }

    public override string GetDescription(TooltipObject tooltipInfo)
    {
        string healthRestoreText = "";
        string effectText = "";
        string effectDuration = "";

        if (healthRestore > 0)
        {
            string text;

            if (consumptionType == ConsumptionType.Single)
                text = "Use: Restore {0} Health to a single member.";
            else
                text = "Use: Restore {0} Health to all members.";

            healthRestoreText = string.Format("<color={1}>\n" + text + "</color>", healthRestore, ColorDatabase.EffectColor());
        }

        if (preBattleEffect != null)
        {
            string text;

            if (consumptionType == ConsumptionType.Single)
                text = "Use: Apply {0} to a single member at the start of the next battle.";
            else
                text = "Use: Apply {0} to all members at the start of the next battle.";

            effectText = string.Format("<color={1}>\n" + text + "</color>" + "\n\n{2}", preBattleEffect.name, ColorDatabase.EffectColor(), preBattleEffect.GetDescription(tooltipInfo));

            if (preBattleEffect.procType == ProcType.Turn)           
                effectDuration = "\nDuration: " + preBattleEffect.duration.ToString() + " Turns";           
            else
                effectDuration = "\nDuration: " + preBattleEffect.duration.ToString() + " Rounds";
        }

        string stacks = "";

        if (stackable)
            stacks = string.Format("\nMaximum Stacks: {0}", maxStacks);

        return base.GetDescription(tooltipInfo) +   // Name
               stacks +                             // Max stacks
               healthRestoreText +                  // Health Restore             
               effectText +                         // Effect description
               effectDuration +                     // Effect duration
               GetItemDescription() +               // Item description
               HowToUseText();                      // Right-click to use
    }

    public override void LeftClick(InteractableItem interactableItem)
    {
        // Do nothing
    }

    public override void RightClick(InteractableItem interactableItem)
    {
        // Do nothing
    }
}
