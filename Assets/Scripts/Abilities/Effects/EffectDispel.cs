using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DispelTargetType
{
    All,
    Positive,
    Negative,
}

public enum DispelSelectionMethod
{
    All,
    Random,
    // ShortestRemaining,
    // LongestRemaining
}

[CreateAssetMenu(fileName = "New Dispel Effect", menuName = "Unit/Effect Object/Dispel")]
public class EffectDispel : EffectObject
{
    [Header("[ Dispel ]")]
    public DispelTargetType targetType = DispelTargetType.All;
    public DispelSelectionMethod selectionMethod = DispelSelectionMethod.All;
    [Range(0, 100)] public int dispelChance = 100;

    [Header("Random")]
    public int dispelCount = 1;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }

    public void DispelEffects(Unit unit)
    {
        var effectManager = unit != null ? unit.effectManager : null;
        if (effectManager == null)
        {
            return;
        }

        if (Random.Range(0, 100) < dispelChance)
        {
            IEnumerable<Effect> relevantEffects = effectManager.effectsList;
            if (relevantEffects.Count() == 0)
            {
                return;
            }

            switch (targetType)
            {
                case DispelTargetType.All:
                    relevantEffects = effectManager.effectsList;
                    break;
                case DispelTargetType.Positive:
                    relevantEffects = effectManager.PositiveEffects;
                    break;
                case DispelTargetType.Negative:
                    relevantEffects = effectManager.NegativeEffects;
                    break;
            }

            relevantEffects = relevantEffects.Where(e => e.effectObject.dispellable);

            switch (selectionMethod)
            {
                case DispelSelectionMethod.All:
                    {
                        for (int i = relevantEffects.Count(); i-- > 0;)
                        {
                            var effect = relevantEffects.ElementAt(i);
                            effectManager.DispelEffect(effect);
                        }
                    }
                    break;
                case DispelSelectionMethod.Random:
                    {
                        int countToDispel = Mathf.Min(dispelCount, relevantEffects.Count());

                        var randomEffects = relevantEffects
                            .OrderBy(x => System.Guid.NewGuid())
                            .Take(countToDispel)
                            .ToList();

                        for (int i = randomEffects.Count(); i-- > 0;)
                        {
                            effectManager.DispelEffect(randomEffects[i]);
                        }
                    }
                    break;
                default:
                    break;
            }

            BattleHUD.Instance.Refresh();
        }
        else
        {
            FCTData fctData = new FCTData(false, unit, "Resisted", ColorDatabase.ConvertString2Color(ColorDatabase.MagicalColor()));
            unit.fctHandler.AddToFCTQueue(fctData);
        }
    }
}