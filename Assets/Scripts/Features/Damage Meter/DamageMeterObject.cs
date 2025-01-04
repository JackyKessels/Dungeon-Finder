using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DamageMeterObject : MonoBehaviour
{
    public GameObject damageMeterWindow;
    public GameObject recordsContainer;
    public GameObject recordPrefab;
    public TextMeshProUGUI heroName;
    public TextMeshProUGUI totalDamageText;
    public TextMeshProUGUI totalHealingText;

    private int totalDamage = 0;
    private int totalHealing = 0;

    private List<DamageMeterRecord> damageMeterRecords;

    public void InitializeDamageMeter(Unit unit)
    {
        damageMeterRecords = new List<DamageMeterRecord>();

        heroName.text = unit.name;

        unit.statsManager.OnCauseUnitEvent += UpdateRecord;
    }

    public DamageMeterRecord GetRecord(AbilityObject abilityObject)
    {
        for (int i = 0; i < damageMeterRecords.Count; i++)
        {
            if (damageMeterRecords[i].abilityObject == abilityObject)
            {
                return damageMeterRecords[i];
            }
        }

        return null;
    }

    public void UpdateRecord(AbilityValue abilityValue)
    {
        if (abilityValue == null || 
            abilityValue.sourceAbility == null || 
            abilityValue.target.statsManager.isInvulnerable ||
            abilityValue.target.effectManager.HasDamageImmunity())
        {
            return;
        }

        DamageMeterRecord record = GetRecord(abilityValue.sourceAbility);

        // No record has been found in the damage meter so create a new record
        if (record == null)
        {
            GameObject obj = ObjectUtilities.CreateSimplePrefab(recordPrefab, recordsContainer);

            record = obj.GetComponent<DamageMeterRecord>();
            record.SetupRecord(abilityValue);
            damageMeterRecords.Add(record);
        }

        record.UpdateRecord(abilityValue);
        UpdateTotal();
        UpdatePercentages();
        SortRecords();
    }

    public void UpdateTotal()
    {
        totalDamage = 0;
        totalHealing = 0;

        foreach (DamageMeterRecord record in damageMeterRecords)
        {
            totalDamage += record.damage;
            totalHealing += record.healing;
        }

        totalDamageText.text = "Damage: " + totalDamage.ToString();
        totalHealingText.text = "Healing: " + totalHealing.ToString();
    }

    public void UpdatePercentages()
    {
        foreach (DamageMeterRecord record in damageMeterRecords)
        {
            record.UpdatePercentage(totalDamage, totalHealing);
        }
    }

    public void SortRecords()
    {
        damageMeterRecords.Sort((a, b) => a.damage.CompareTo(b.damage));
        damageMeterRecords.Reverse();

        for (int i = 0; i < damageMeterRecords.Count; i++)
        {
            damageMeterRecords[i].transform.SetSiblingIndex(i);
        }
    }

    public void ClearRecords()
    {
        ObjectUtilities.ClearContainer(recordsContainer);

        damageMeterRecords.Clear();

        UpdateTotal();
    }

    public bool HasRecords()
    {
        return damageMeterRecords.Count > 0;
    }
}
