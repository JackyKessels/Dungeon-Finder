using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarterPathGameObject : MonoBehaviour
{
    public GameObject tooltipIconPrefab;

    public TextMeshProUGUI title;

    public GameObject weaponContainer;
    public GameObject abilityContainer;

    private HeroPathObject heroPathObject;
    private Hero hero;

    public void Setup(HeroPathObject _heroPathObject, Hero _hero)
    {
        heroPathObject = _heroPathObject;
        hero = _hero;

        title.text = heroPathObject.name;

        //ObjectUtilities.ClearContainer(abilityContainer);

        //foreach (AbilityObject abilityObject in heroPathObject.baseAbilities)
        //{
        //    AddTooltipIcon(abilityContainer, abilityObject);
        //}

        ObjectUtilities.ClearContainer(weaponContainer);

        foreach (ItemObject itemObject in heroPathObject.baseWeapons)
        {
            AddTooltipIcon(weaponContainer, itemObject);
        }
    }

    private void AddTooltipIcon(GameObject container, AbilityObject abilityObject)
    {
        GameObject obj = Instantiate(tooltipIconPrefab, container.transform.position, Quaternion.identity);
        obj.transform.SetParent(container.transform);
        obj.transform.localScale = Vector3.one;

        TooltipIcon icon = obj.GetComponent<TooltipIcon>();
        icon.Setup(abilityObject);
    }

    private void AddTooltipIcon(GameObject container, ItemObject itemObject)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(tooltipIconPrefab, container);

        TooltipIcon icon = obj.GetComponent<TooltipIcon>();
        icon.Setup(itemObject);
    }

    public void ChoosePath()
    {
        Debug.Log(hero + " has choosen the " + heroPathObject.name + " path.");

        HeroManager.Instance.PickStarterPath(heroPathObject, hero);
    }
}