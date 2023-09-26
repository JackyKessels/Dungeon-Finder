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
    }

    public string GetDescription(TooltipObject tooltipInfo, string itemName, string itemDescription)
    {
        if (activeAbilityLearned != null)
        {
            return itemName + string.Format("<color={1}>\nUse: Learn the {0} ability.</color>", activeAbilityLearned.name, ColorDatabase.EffectColor()) + "\n\n" + activeAbilityLearned.GetDescription(tooltipInfo) + itemDescription + HowToUseText();
        }
        else if (passiveAbilityLearned != null)
        {
            return itemName + string.Format("<color={1}>\nUse: Learn the {0} ability.</color>", passiveAbilityLearned.name, ColorDatabase.EffectColor()) + itemDescription + HowToUseText();
        }
        else
        {
            return "This tome contains no valuable information.";
        }
    }
}
