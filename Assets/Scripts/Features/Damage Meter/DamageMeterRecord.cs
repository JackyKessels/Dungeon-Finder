using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageMeterRecord : MonoBehaviour
{
    public Image abilityIcon;

    public TextMeshProUGUI damageTotalText;
    public TextMeshProUGUI damagePercentageText;
    public TextMeshProUGUI damagenInstancesText;

    public TextMeshProUGUI healingTotalText;
    public TextMeshProUGUI healingPercentageText;
    public TextMeshProUGUI healingInstancesText;

    public AbilityObject abilityObject;

    private int damageInstances = 0;
    private int healingInstances = 0;

    public int damage = 0;
    public int healing = 0;


    public void SetupRecord(AbilityValue abilityValue)
    {
        abilityObject = abilityValue.sourceAbility;

        abilityIcon.sprite = abilityObject.icon;

        if (abilityValue.school != AbilitySchool.Healing)
        {
            damageTotalText.text = damage.ToString();
            damageTotalText.color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(abilityValue.school));

            damagenInstancesText.text = damageInstances.ToString();
        }
        else
        {
            healingTotalText.text = healing.ToString();
            healingTotalText.color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(abilityValue.school));

            healingInstancesText.text = healingInstances.ToString();
        }

        
    }

    public void UpdateRecord(AbilityValue abilityValue)
    {
        if (abilityValue.school != AbilitySchool.Healing)
        {
            damage += abilityValue.Rounded();
            damageTotalText.text = damage.ToString();

            damageInstances++;
            damagenInstancesText.text = damageInstances.ToString();
        }
        else
        {
            healing += abilityValue.Rounded();
            healingTotalText.text = healing.ToString();

            healingInstances++;
            healingInstancesText.text = healingInstances.ToString();
        }

        Color mixedSchool = GeneralUtilities.ConvertString2Color("#FFFFFF");

        if (damageTotalText.color == mixedSchool)
            return;

        if (damageTotalText.color != GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(abilityValue.school)))
            damageTotalText.color = mixedSchool;
    }

    public void UpdatePercentage(int totalDamage, int totalHealing)
    {
        if (damage == 0)
        {
            damagePercentageText.text = "0%";
        }        
        else
        {
            float percentage = ((float)damage / (float)totalDamage * 100);

            damagePercentageText.text = GeneralUtilities.RoundFloat(percentage, 1).ToString() + "%";
        }

        if (healing == 0)
        {
            healingPercentageText.text = "0%";
        }
        else
        {
            float percentage = ((float)healing / (float)totalHealing * 100);

            healingPercentageText.text = GeneralUtilities.RoundFloat(percentage, 1).ToString() + "%";
        }
    }
}
