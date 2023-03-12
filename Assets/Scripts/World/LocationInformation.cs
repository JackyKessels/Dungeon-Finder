using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationInformation : MonoBehaviour
{
    public GameObject mainObject;
    public Button interactButton;
    public TextMeshProUGUI titleText;
    public Image image;
    public TextMeshProUGUI description;

    public void Setup(Location location)
    {
        titleText.text = location.name;
        image.sprite = location.locationImage;
        description.text = SetDescription(location);

        if (!location.locked)
            interactButton.GetComponentInChildren<TextMeshProUGUI>().text = LocationEventText(location);
        else
            interactButton.GetComponentInChildren<TextMeshProUGUI>().text = "LOCKED";
    }

    private string SetDescription(Location location)
    {
        switch (location.locationType)
        {
            case LocationType.Battle: case LocationType.Elite: case LocationType.Boss:
                {
                    string enemies = "";

                    for (int i = 0; i < location.enemyUnits.Count; i++)
                    {
                        enemies += location.enemyUnits[i].name + "\n";
                    }

                    return enemies;
                }
            case LocationType.Treasure:
                return location.TypeDescription(); // "Your team stumbles upon treasures.";
            case LocationType.Campfire:
                return location.TypeDescription(); // "The campfire will restore your Team's spirit.";
            case LocationType.Spirit:
                return "You find some glowing stuff.";
            case LocationType.Mystery:
                return location.TypeDescription(); // "Something is going on here... what could it be?";
            default:
                return location.description;
        }
    }

    private string LocationEventText(Location location)
    {
        switch (location.locationType)
        {
            case LocationType.Empty:
                return "Interact";
            case LocationType.Battle:
                return "Fight";
            case LocationType.Elite:
                return "Fight";
            case LocationType.Boss:
                return "Fight";
            case LocationType.Treasure:
                return "Loot";
            case LocationType.Campfire:
                return "Rest";
            case LocationType.Spirit:
                return "Absorb";
            case LocationType.Mystery:
                return "Explore";
            default:
                return "";
        }
    }
}
