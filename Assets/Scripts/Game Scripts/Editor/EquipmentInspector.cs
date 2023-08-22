using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(EquipmentObject))]
public class EquipmentInspector : Editor
{
    private readonly float armorWeight = 3f;
    private readonly float resistanceWeight = 3f;
    private readonly float vitalityWeight = 2f;
    private readonly float speedWeight = 15f;
    private readonly float accuracyWeight = 5f;
    private readonly float critWeight = 3f;
    private readonly float critPowerWeight = 1f;
    private readonly float schoolWeight = 2f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EquipmentObject equipmentObject = (EquipmentObject)target;

        float totalPercentage = 0f;

        totalPercentage += equipmentObject.healthFactor * 100f;
        totalPercentage += equipmentObject.powerFactor * 100f;
        totalPercentage += equipmentObject.wisdomFactor * 100f;

        totalPercentage += equipmentObject.armor * armorWeight;
        totalPercentage += equipmentObject.resistance * resistanceWeight;
        totalPercentage += equipmentObject.vitality * vitalityWeight;
        totalPercentage += equipmentObject.speed * speedWeight;
        totalPercentage += equipmentObject.accuracy * accuracyWeight;
        totalPercentage += equipmentObject.crit * critWeight;

        totalPercentage += equipmentObject.healingMultiplier * schoolWeight;
        totalPercentage += equipmentObject.physicalMultiplier * schoolWeight;
        totalPercentage += equipmentObject.fireMultiplier * schoolWeight;
        totalPercentage += equipmentObject.iceMultiplier * schoolWeight;
        totalPercentage += equipmentObject.natureMultiplier * schoolWeight;
        totalPercentage += equipmentObject.arcaneMultiplier * schoolWeight;
        totalPercentage += equipmentObject.holyMultiplier * schoolWeight;
        totalPercentage += equipmentObject.shadowMultiplier * schoolWeight;
        totalPercentage += equipmentObject.critMultiplier * critPowerWeight;

        if (equipmentObject.useAbility == null && equipmentObject.passives.Count == 0)
        {
            totalPercentage -= 20f;
        }

        if (equipmentObject.slot == EquipmentSlot.Trinket)
        {
            totalPercentage = 100f;
        }

        GUILayout.Space(10f);
        GUILayout.TextField($"Total Percentage: {totalPercentage}%");
    }
}
