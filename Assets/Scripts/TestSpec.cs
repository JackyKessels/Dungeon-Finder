using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSpec : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI specTitle;
    [SerializeField] GameObject primaryContainer;
    [SerializeField] GameObject commonContainer;
    [SerializeField] GameObject mysticalContainer;
    [SerializeField] GameObject passivesContainer;

    [SerializeField] GameObject abilityPrefab;

    public void Setup(HeroPathObject heroPathObject)
    {
        specTitle.text = heroPathObject.name;

        ObjectUtilities.ClearContainer(primaryContainer);
        ObjectUtilities.ClearContainer(commonContainer);
        ObjectUtilities.ClearContainer(mysticalContainer);
        ObjectUtilities.ClearContainer(passivesContainer);
        
        foreach (ActiveAbility activeAbility in heroPathObject.activeAbilities)
        {
            if (activeAbility != null)
            {
                switch (activeAbility.quality)
                {
                    case Quality.Common:
                        {
                            if (activeAbility.abilityType == AbilityType.Primary)
                            {
                                CreateAbility(activeAbility, primaryContainer);
                            }
                            else
                            {
                                CreateAbility(activeAbility, commonContainer);
                            }
                        }
                        break;
                    case Quality.Mystical:
                        {
                            CreateAbility(activeAbility, mysticalContainer);
                        }
                        break;
                    case Quality.Legendary:
                        break;
                    default:
                        break;
                }
            }
        }

        CreateAbility(heroPathObject.passiveAbility, passivesContainer);

        foreach (PassiveAbility passiveAbility in heroPathObject.passiveAbilities)
        {
            if (passiveAbility != null)
            {
                CreateAbility(passiveAbility, passivesContainer);
            }
        }
    }

    private void CreateAbility(AbilityObject ability, GameObject container)
    {
        if (ability == null)
            return;

        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityPrefab, container);

        TooltipObject info = obj.GetComponent<TooltipObject>();
        info.state = CurrentState.Values;

        Image image = obj.GetComponent<Image>();
        image.sprite = ability.icon;

        if (ability is ActiveAbility)
        {
            Active active = new Active(ability as ActiveAbility, 1);
            info.active = active;
        }
        else if (ability is PassiveAbility p)
        {
            info.passive.passiveAbility = p;
        }

        if (!DatabaseHandler.Instance.abilityDatabase.abilityObjects.Contains(ability))
        {
            Debug.Log($"{ability.name} is not in the ability database.");
        }
    }
}
