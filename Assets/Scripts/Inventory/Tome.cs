using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tome", menuName = "Item/Consumable/Tome")]
public class Tome : Consumable
{
    public ActiveAbility activeAbilityLearned;
    public PassiveAbility passiveAbilityLearned;

    public override bool Consume(int i)
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

                        unit.spellbook.LearnPassive(passive);
                    }

                    HeroManager.Instance.Refresh(unit);
                }
                break;
            case ConsumptionType.Party:
                break;
            case ConsumptionType.None:
                break;
        }

        return true;
    }

    public override string GetTooltip(TooltipObject tooltipInfo)
    {
        if (activeAbilityLearned != null)
        {
            return $"\nUse: Learn the {activeAbilityLearned.name} ability." + 
                   $"\n\n{activeAbilityLearned.GetCompleteTooltip(tooltipInfo)}";
        }
        else if (passiveAbilityLearned != null)
        {
            return $"\nUse: Learn the {passiveAbilityLearned.name} ability." +
                   $"\n\n{passiveAbilityLearned.GetCompleteTooltip(tooltipInfo)}";
        }
        else
        {
            return "This tome contains no valuable information.";
        }
    }
}
