#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EquipmentStatsValidator
{
    [MenuItem("Tools/Equipment/Validate Power Budget")]
    public static void ValidateEquipment()
    {
        string[] guids = AssetDatabase.FindAssets("t:EquipmentObject");

        var under = new List<EquipmentObject>();
        var over = new List<EquipmentObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var eq = AssetDatabase.LoadAssetAtPath<EquipmentObject>(path);

            if (eq == null)
                continue;

            if (eq.slot == EquipmentSlot.Flask)
                continue;

            float value = CalculateTotal(eq);

            if (value < 100f)
            {
                under.Add(eq);
                Debug.Log($"UNDER 100: {eq.name} | Value: {value:0.0}\n{path}", eq);
            }
            else if (value > 100f)
            {
                over.Add(eq);
                Debug.Log($"OVER 100: {eq.name} | Value: {value:0.0}\n{path}", eq);
            }
        }

        Debug.Log($"Equipment validation complete.");
        Debug.Log($"Under budget (<100): {under.Count}");
        Debug.Log($"Over budget (>100): {over.Count}");
    }

    private static float CalculateTotal(EquipmentObject e)
    {
        float total = 0f;

        total += e.healthFactor * 100f;
        total += e.powerFactor * 100f;
        total += e.wisdomFactor * 100f;

        total += e.armor * 3f;
        total += e.resistance * 3f;
        total += e.vitality * 2f;
        total += e.speed * 15f;
        total += e.accuracy * 5f;
        total += e.crit * 3f;

        total += e.healingMultiplier * 2f;
        total += e.physicalMultiplier * 2f;
        total += e.fireMultiplier * 2f;
        total += e.iceMultiplier * 2f;
        total += e.natureMultiplier * 2f;
        total += e.arcaneMultiplier * 2f;
        total += e.holyMultiplier * 2f;
        total += e.shadowMultiplier * 2f;

        total += e.critMultiplier * 1f;
        total += e.sacrificialMultiplier * -1f;

        if (e.useAbility == null && e.passives.Count == 0)
            total -= 20f;

        if (e.slot == EquipmentSlot.Trinket)
            total = 100f;

        return total;
    }
}
#endif