using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JourneyManager
{
    private Canvas UI;
    private TextMeshProUGUI battleText, defeatedText;

    public JourneyManager (Canvas c)
    {
        UI = c;
        battleText = UI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        defeatedText = UI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void ShowResults(int battles, int defeated)
    {
        battleText.text = "Battles Survived: " + battles;
        defeatedText.text = "Monsters Defeated: " + defeated;
    }
}
