using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroPathOption : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] GameObject container;
    [SerializeField] GameObject pathPrefab;

    public void CreatePathOptions(Hero hero)
    {
        if (hero.heroObject.lastName == "")
            heroName.text = hero.heroObject.name;
        else
            heroName.text = hero.heroObject.name + " " + hero.heroObject.lastName;

        ObjectUtilities.ClearContainer(container);

        foreach (HeroPath heroPath in hero.heroPathManager.paths)
        {
            CreatePath(heroPath.path, hero);
        }
    }

    private void CreatePath(HeroPathObject heroPathObject, Hero hero)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(pathPrefab, container);

        UnlockAbilityButton unlockAbilityButton = obj.GetComponent<UnlockAbilityButton>();
        unlockAbilityButton.Setup(heroPathObject, hero);
    }
}
