using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroAbilityOverview : MonoBehaviour
{
    public GameObject abilityPrefab;

    public TextMeshProUGUI heroName;

    public TextMeshProUGUI spec1;
    public TextMeshProUGUI spec2;
    public TextMeshProUGUI spec3;

    public GameObject container1;
    public GameObject container2;
    public GameObject container3;

    public void Setup(Hero hero)
    {
        heroName.text = hero.name;

        SetupSpecialization(hero.heroPathManager.paths[0].path, spec1, container1);
        SetupSpecialization(hero.heroPathManager.paths[1].path, spec2, container2);
        SetupSpecialization(hero.heroPathManager.paths[2].path, spec3, container3);
    }

    private void SetupSpecialization(HeroPathObject heroPathObject, TextMeshProUGUI spec, GameObject container)
    {
        spec.text = heroPathObject.name;

        foreach (ActiveAbility activeAbility in heroPathObject.activeAbilities)
        {
            CreateAbility(activeAbility, container);
        }
    }

    private void CreateAbility(ActiveAbility ability, GameObject container)
    {
        if (ability == null)
            return;

        GameObject obj = ObjectUtilities.CreateSimplePrefab(abilityPrefab, container);

        TooltipObject info = obj.GetComponent<TooltipObject>();
        info.state = CurrentState.Values;
        info.active = new Active(ability, 1);

        Image image = obj.GetComponent<Image>();
        image.sprite = ability.icon;   
    }
}
