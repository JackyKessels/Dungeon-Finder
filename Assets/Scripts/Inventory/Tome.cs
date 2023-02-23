using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tome", menuName = "Item/Consumable/Tome")]
public class Tome : Consumable
{
    public ActiveAbility activeAbilityLearned;
    public PassiveAbility passiveAbilityLearned;

    public override void Consume(int i)
    {
        switch (consumptionType)
        {
            case ConsumptionType.Single:
                {
                    Unit unit = TeamManager.Instance.heroes.Members[i];

                    if (activeAbilityLearned != null)
                    {
                        Active active = new Active(activeAbilityLearned, 1);

                        unit.spellbook.LearnAbility(active);
                    }
                    else if (passiveAbilityLearned != null)
                    {
                        Passive passive = new Passive(passiveAbilityLearned, 1);

                        unit.spellbook.AddPassive(passive);
                    }

                    HeroManager.Instance.Refresh(unit);
                }
                break;
            case ConsumptionType.Party:
                break;
            case ConsumptionType.None:
                break;
        }
    }

    public override string GetDescription(TooltipObject tooltipInfo)
    {
        if (activeAbilityLearned != null)
        {
            return base.GetDescription(tooltipInfo) + string.Format("<color={1}>\nUse: Learn the {0} ability.</color>", activeAbilityLearned.name, ColorDatabase.EffectColor()) + "\n\n" + activeAbilityLearned.GetDescription(tooltipInfo) + GetItemDescription() + HowToUseText();
        }
        else if (passiveAbilityLearned != null)
        {
            return base.GetDescription(tooltipInfo) + string.Format("<color={1}>\nUse: Learn the {0} ability.</color>", passiveAbilityLearned.name, ColorDatabase.EffectColor()) + GetItemDescription() + HowToUseText();
        }
        else
        {
            return "This tome contains no valuable information.";
        }
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
