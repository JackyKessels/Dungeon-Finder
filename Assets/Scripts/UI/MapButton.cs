using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    public TextMeshProUGUI text;


    private void Update()
    {
        if (KeyboardHandler.OpenMap() &&
            GameManager.Instance.gameMode == GameMode.Campaign)
        {
            ActivateButton();
        }
    }

    public void UpdateButton(GameState state)
    {
        if (state == GameState.RUN)
        {
            text.text = "Abandon";
            text.color = GeneralUtilities.ConvertString2Color("#D60000");
        }
        else if (state == GameState.TOWN)
        {
            text.text = "Map";
            text.color = GeneralUtilities.ConvertString2Color("#A9FF81");
        }
    }

    public void ActivateButton()
    {
        GameState state = GameManager.Instance.gameState;

        if (state == GameState.RUN)
        {
            DungeonManager.Instance.AbandonRunButton();
        }
        else if (state == GameState.TOWN)
        {
            TownManager.Instance.OpenMap();
        }
    }
}
