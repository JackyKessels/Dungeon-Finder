using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeroSelectionManager : MonoBehaviour
{
    public List<HeroObject> heroes;
    public GameObject positionHero;
    public GameObject heroInformation;

    private int heroIndex;

    private TextMeshProUGUI text;

    private string textColor = "#FFFFFF";
    private string nameColor = "#FFDF13";

    public void Setup()
    {
        text = heroInformation.GetComponentInChildren<TextMeshProUGUI>();

        heroIndex = 0;

        positionHero.SetActive(true);
        DisplayHero(heroes[heroIndex]);
        UpdateText(heroes[heroIndex]);
    }

    public void SetupFullTeam()
    {
        for (int i = 0; i < 3; i++)
        {
            TeamManager.Instance.CreateHero(i, i);
            GameManager.Instance.cameraScript.GoToCamera(HeroManager.Instance.cameraObject, false);
            GameManager.Instance.EnableUI(HeroManager.Instance.userInterface);
        }       
    }

    private void DisplayHero(HeroObject heroObject)
    {
        positionHero.SetActive(true);
        CharacterSelecter character = positionHero.GetComponent<CharacterSelecter>();
        character.unitObject = heroObject;
        Image image = positionHero.GetComponent<Image>();
        image.sprite = heroObject.sprite;
        image.SetNativeSize();
        RectTransform rt = positionHero.GetComponent<RectTransform>();
        //rt.sizeDelta = new Vector2(heroObject.sprite.rect.size.x, heroObject.sprite.rect.size.y);
        rt.localScale = new Vector3(1.8f, 1.8f, 1f);
    }

    public void NextHero()
    {
        heroIndex++;

        if (heroIndex >= heroes.Count)
            heroIndex = 0;

        positionHero.SetActive(true);
        DisplayHero(heroes[heroIndex]);
        UpdateText(heroes[heroIndex]);
    }

    public void SelectHero()
    {
        //TeamManager.Instance.CreateHero(heroes[heroIndex], 0);
        GameManager.Instance.cameraScript.GoToCamera(HeroManager.Instance.cameraObject, false);
        GameManager.Instance.EnableUI(HeroManager.Instance.userInterface);
    }

    public void UpdateText(HeroObject h)
    {     
        text.text = string.Format("<color={0}>" +
            "<color={1}><size=42>{2}</size></color>" +
            "\nClass: {3}" +
            "\nHealth: {4}" +
            "\nPower: {5}" +
            "\nWisdom: {6}" +
            "</color>",
            textColor, 
            nameColor,
            h.name, 
            h.heroClass, 
            h.baseHealth,
            h.basePower,
            h.baseWisdom);
    }
}
