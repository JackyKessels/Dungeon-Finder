using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionObject : MonoBehaviour
{
    [SerializeField] Image heroImage;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] Button previousSpec;
    [SerializeField] Button nextSpec;
    [SerializeField] TextMeshProUGUI specName;
    [SerializeField] GameObject weaponContainer;
    [SerializeField] GameObject tooltipIconPrefab;
    [SerializeField] TooltipIcon primaryAbility;
    [SerializeField] TooltipIcon classAbility;
    [SerializeField] TooltipIcon secondaryAbility;
    [SerializeField] TooltipIcon passiveAbility;

    private int heroObjectIndex;
    private int currentSpec = 0;
    private int specLimit;

    public void Setup(int _heroObjectIndex)
    {
        heroObjectIndex = _heroObjectIndex;

        HeroObject heroObject = GetHeroObject(heroObjectIndex);

        specLimit = heroObject.paths.Count;

        heroImage.sprite = heroObject.sprite;
        heroImage.SetNativeSize();

        heroName.text = GeneralUtilities.GetFullUnitName(heroObject);

        heroName.text = heroObject.name + " " + heroObject.lastName;

        previousSpec.onClick.AddListener(PreviousSpec);
        nextSpec.onClick.AddListener(NextSpec);

        UpdateSpec();
    }

    private void UpdateSpec()
    {
        HeroObject heroObject = GetHeroObject(heroObjectIndex);

        specName.text = heroObject.paths[currentSpec].name;

        CreateWeaponIcons(heroObject);

        primaryAbility.Setup(heroObject.startingAbilities[0]);
        //primaryAbility.Setup(heroObject.paths[currentSpec].primaryAbility);
        //classAbility.Setup(heroObject.startingAbilities[1]);
        secondaryAbility.Setup(heroObject.paths[currentSpec].secondaryAbility);
        //secondaryAbility.GetComponent<Image>().color = Color.gray;
        passiveAbility.Setup(heroObject.paths[currentSpec].passiveAbility);
    }

    private void CreateWeaponIcons(HeroObject heroObject)
    {
        ObjectUtilities.ClearContainer(weaponContainer);

        foreach (EquipmentObject weapon in heroObject.paths[currentSpec].baseWeapons)
        {
            TooltipIcon weaponIcon = ObjectUtilities.CreateSimplePrefab(tooltipIconPrefab, weaponContainer).GetComponent<TooltipIcon>();

            weaponIcon.Setup(weapon, 1);
        }
    }

    private void NextSpec()
    {
        SoundUtilities.PlayClick();

        currentSpec++;

        if (currentSpec > specLimit - 1)
            currentSpec = 0;

        UpdateSpec();
    }

    private void PreviousSpec()
    {
        SoundUtilities.PlayClick();

        currentSpec--;

        if (currentSpec < 0)
            currentSpec = specLimit - 1;

        UpdateSpec();
    }

    public int[] GetData()
    {
        int[] data = new int[2];

        data[0] = heroObjectIndex;
        data[1] = currentSpec;

        return data;
    }

    private HeroObject GetHeroObject(int index)
    {
        return TeamManager.Instance.heroObjects[heroObjectIndex];
    }
}
