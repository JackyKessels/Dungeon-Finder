using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtilities
{
    public static string GetFullUnitName(UnitObject unitObject)
    {
        if (unitObject.lastName == "")
            return unitObject.name;
        else
            return unitObject.name + " " + unitObject.lastName;
    }

    public static Unit GetCorrectUnit(TooltipObject tooltipInfo)
    {
        switch (tooltipInfo.state)
        {
            case CurrentState.Battle:
                return BattleManager.Instance.currentHero;
            case CurrentState.HeroInformation:
                return HeroManager.Instance.CurrentHero();
            case CurrentState.Reward:
                return RewardManager.Instance.CurrentHero();
            case CurrentState.Default:
                return HeroManager.Instance.CurrentHero();
            case CurrentState.Values:
                return null;
            default:
                return null;
        }
    }

    public static string FormattedEnemyObject((EnemyObject enemyObject, int level) enemy)
    {
        return enemy.enemyObject.name + " (" + enemy.level + ")";
    }

    public static string GetCorrectCrowdControlText(CrowdControlType crowdControlType)
    {
        switch (crowdControlType)
        {
            case CrowdControlType.Taunt:
                return "Taunt";
            case CrowdControlType.Stun:
                return "Stun";
            case CrowdControlType.Incapacitate:
                return "Incapacitate";
            default:
                return "";
        }
    }

    public static string GetCorrectAttributeName(AttributeType attributeType)
    {
        switch (attributeType)
        {
            case AttributeType.Crit:
                return "Critical Chance";
            case AttributeType.HealingMultiplier:
                return "Healing Done";
            case AttributeType.PhysicalMultiplier:
                return "Physical Damage";
            case AttributeType.FireMultiplier:
                return "Fire Damage";
            case AttributeType.IceMultiplier:
                return "Ice Damage";
            case AttributeType.NatureMultiplier:
                return "Nature Damage";
            case AttributeType.ArcaneMultiplier:
                return "Arcane Damage";
            case AttributeType.HolyMultiplier:
                return "Holy Damage";
            case AttributeType.ShadowMultiplier:
                return "Shadow Damage";
            case AttributeType.CritMultiplier:
                return "Critical Power";
            case AttributeType.SacrificialMultiplier:
                return "Sacrificial Damage";
            default:
                return attributeType.ToString();
        }
    }

    public static Sprite GetAttributeIcon(AttributeType attributeType)
    {
        switch (attributeType)
        {
            // General Attributes
            case AttributeType.Health:
                return GameAssets.i.health;
            case AttributeType.Power:
                return GameAssets.i.power;
            case AttributeType.Wisdom:
                return GameAssets.i.wisdom;
            case AttributeType.Armor:
                return GameAssets.i.armor;
            case AttributeType.Resistance:
                return GameAssets.i.resistance;
            case AttributeType.Vitality:
                return GameAssets.i.vitality;
            case AttributeType.Speed:
                return GameAssets.i.speed;
            case AttributeType.Accuracy:
                return GameAssets.i.accuracy;
            case AttributeType.Crit:
                return GameAssets.i.crit;

            // School Modifiers
            case AttributeType.HealingMultiplier:
                return GameAssets.i.healingMultiplier;
            case AttributeType.PhysicalMultiplier:
                return GameAssets.i.physicalMultiplier;
            case AttributeType.FireMultiplier:
                return GameAssets.i.fireMultiplier;
            case AttributeType.IceMultiplier:
                return GameAssets.i.iceMultiplier;
            case AttributeType.NatureMultiplier:
                return GameAssets.i.natureMultiplier;
            case AttributeType.ArcaneMultiplier:
                return GameAssets.i.arcaneMultiplier;
            case AttributeType.HolyMultiplier:
                return GameAssets.i.holyMultiplier;
            case AttributeType.ShadowMultiplier:
                return GameAssets.i.shadowMultiplier;
            case AttributeType.CritMultiplier:
                return GameAssets.i.critMultiplier;
            case AttributeType.SacrificialMultiplier:
                return GameAssets.i.sacrificialMultiplier;
            default:
                return null;
        }
    }

    public static Sprite GetSchoolIcon(AbilitySchool abilitySchool)
    {
        switch (abilitySchool)
        {
            case AbilitySchool.Healing:
                return GameAssets.i.healingMultiplier;
            case AbilitySchool.Physical:
                return GameAssets.i.physicalMultiplier;
            case AbilitySchool.Fire:
                return GameAssets.i.fireMultiplier;
            case AbilitySchool.Ice:
                return GameAssets.i.iceMultiplier;
            case AbilitySchool.Nature:
                return GameAssets.i.natureMultiplier;
            case AbilitySchool.Arcane:
                return GameAssets.i.arcaneMultiplier;
            case AbilitySchool.Holy:
                return GameAssets.i.holyMultiplier;
            case AbilitySchool.Shadow:
                return GameAssets.i.shadowMultiplier;
            case AbilitySchool.Sacrificial:
                return GameAssets.i.sacrificialMultiplier;
            default:
                return null;
        }
    }

    public static int GetCorrectEquipmentslot(EquipmentSlot equipmentSlot)
    {
        switch (equipmentSlot)
        {
            case EquipmentSlot.Helmet:
                return 0;
            case EquipmentSlot.Armor:
                return 1;
            case EquipmentSlot.TwoHand:
                return 2;
            case EquipmentSlot.OneHand:
                return 2;
            case EquipmentSlot.Shield:
                return 3;
            case EquipmentSlot.Relic:
                return 3;
            case EquipmentSlot.Necklace:
                return 4;
            case EquipmentSlot.Ring:
                return 5;
            case EquipmentSlot.Trinket:
                return 6;
            case EquipmentSlot.Flask:
                return 7;
            case EquipmentSlot.Nothing:
                return -1;
            default:
                return -1;
        }
    }

    public static bool GetMappedAbilityKey(int index, bool isHeroAbility)
    {
        if (isHeroAbility)
        {
            if (index == 0 && KeyboardHandler.CastAbility1())
                return true;
            else if (index == 1 && KeyboardHandler.CastAbility2())
                return true;
            else if (index == 2 && KeyboardHandler.CastAbility3())
                return true;
            else if (index == 3 && KeyboardHandler.CastAbility4())
                return true;
            else
                return false;
        }
        else
        {
            if (index == 0 && KeyboardHandler.UseItem1())
                return true;
            if (index == 1 && KeyboardHandler.UseItem2())
                return true;
            if (index == 2 && KeyboardHandler.UseItem3())
                return true;
            if (index == 3 && KeyboardHandler.UseItem4())
                return true;
            if (index == 4 && KeyboardHandler.UseItem5())
                return true;
            if (index == 5 && KeyboardHandler.UseItem6())
                return true;
            if (index == 6 && KeyboardHandler.UseItem7())
                return true;
            if (index == 7 && KeyboardHandler.UseItem8())
                return true;
        }

        return false;
    }

    public static int RoundFloat(float value, int decimals)
    {
        return (int)System.Math.Round(value, decimals, System.MidpointRounding.AwayFromZero);
    }

    public static Color ConvertString2Color(string s)
    {
        ColorUtility.TryParseHtmlString(s, out Color c);

        return c;
    }

    public static int RandomWeighted(List<int> weights)
    {
        int weightTotal = 0;

        foreach (int weight in weights)
            weightTotal += weight;

        if (weightTotal == 0)
            return 0;

        int result;
        int total = 0;

        int randVal = Random.Range(0, weightTotal);
        for (result = 0; result < weights.Count; result++)
        {
            total += weights[result];
            if (total > randVal) break;
        }

        return result;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static float DefensiveReductionValue_Linear(float amount)
    {
        return amount * 0.01f;
    }

    public static float DefensiveReductionValue_League_Tweaked(float amount)
    {
        // Positive Armor
        if (amount >= 0)
            return 100 / (100 + amount);
        // Negative Armor
        else
        {
            float tempAmount = -1 * amount;

            return 2 - (100 / (100 + tempAmount));
        }

        // -100 -> 1.5
        // -50  -> 1.33
        // 0    -> 1
        // 50   -> 0.67
        // 100  -> 0.5

    }

    public static ReductionType GetReductionType(AbilitySchool abilitySchool)
    {
        if (abilitySchool == AbilitySchool.Physical)
            return ReductionType.Armor;
        else if (abilitySchool == AbilitySchool.Sacrificial)
            return ReductionType.None;
        else
            return ReductionType.Resistance;
    }

    // Turns a list of strings into a string where each element is seperated by a string
    // The last two elements are seperated with a different string
    // Each seperate element (not the commas or and) is also colored
    public static string JoinString(List<string> strings, string separator, string finalSeparator, string color = null)
    {
        if (strings == null || strings.Count == 0)
            return "";

        string fullString;

        if (strings.Count > 1)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < strings.Count - 1; i++)
            {
                if (color != null)
                {
                    stringList.Add(string.Format("<color={1}>{0}</color>", strings[i], color));
                }
                else
                {
                    stringList.Add(strings[i]);
                }
            }

            fullString = string.Join(separator, stringList);

            if (color != null)
            {
                fullString += finalSeparator + string.Format("<color={1}>{0}</color>", strings[strings.Count - 1], color);
            }
            else
            {
                fullString += finalSeparator + strings[strings.Count - 1];
            }
        }
        else
        {
            if (color != null)
            {
                fullString = string.Format("<color={1}>{0}</color>", strings[0], color);
            }
            else
            {
                fullString = strings[0];
            }
        }

        return fullString;
    }
}
