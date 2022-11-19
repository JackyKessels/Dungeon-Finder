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
    public TextMeshProUGUI totalDamageNumber;

    private int totalDamage = 0;

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
        if (abilityValue == null || abilityValue.sourceAbility == null)
            return;

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

        foreach (DamageMeterRecord record in damageMeterRecords)
        {
            totalDamage += record.totalDamage;
        }

        totalDamageNumber.text = totalDamage.ToString();
    }

    public void UpdatePercentages()
    {
        foreach (DamageMeterRecord record in damageMeterRecords)
        {
            record.UpdatePercentage(totalDamage);
        }
    }

    public void SortRecords()
    {
        damageMeterRecords.Sort((a, b) => a.totalDamage.CompareTo(b.totalDamage));
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
    }
}
