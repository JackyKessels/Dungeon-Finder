using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageMeterRecord : MonoBehaviour
{
    public Image abilityIcon;

    public TextMeshProUGUI instancesText;
    public TextMeshProUGUI totalDamageText;
    public TextMeshProUGUI totalPercentageText;

    public AbilityObject abilityObject;

    private int damageInstances = 0;
    private int healingInstances = 0;
    public int totalDamage = 0;
    public int totalHealing = 0;


    public void SetupRecord(AbilityValue abilityValue)
    {
        abilityObject = abilityValue.sourceAbility;

        abilityIcon.sprite = abilityObject.icon;

        totalDamageText.text = totalDamage.ToString();
        totalDamageText.color = GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(abilityValue.school));

        instancesText.text = damageInstances.ToString();
    }

    public void UpdateRecord(AbilityValue abilityValue)
    {
        if (abilityValue.school != AbilitySchool.Healing)
        {
            damageInstances++;
            instancesText.text = damageInstances.ToString();

            totalDamage += abilityValue.Rounded();
            totalDamageText.text = totalDamage.ToString();
        }
        else
        {
            healingInstances++;
            instancesText.text = healingInstances.ToString();

            totalHealing += abilityValue.Rounded();
            totalDamageText.text = totalHealing.ToString();
        }

        Color mixedSchool = GeneralUtilities.ConvertString2Color("#FFFFFF");

        if (totalDamageText.color == mixedSchool)
            return;

        if (totalDamageText.color != GeneralUtilities.ConvertString2Color(ColorDatabase.SchoolColor(abilityValue.school)))
            totalDamageText.color = mixedSchool;
    }

    public void UpdatePercentage(int total)
    {
        if (total == 0)
        {
            totalPercentageText.text = "0%";
        }        
        else
        {
            float percentage = ((float)totalDamage / (float)total * 100);

            totalPercentageText.text = GeneralUtilities.RoundFloat(percentage).ToString() + "%";
        }
    }
}
