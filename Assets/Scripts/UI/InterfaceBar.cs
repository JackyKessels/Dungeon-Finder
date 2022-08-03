using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceBar : MonoBehaviour
{
    public Button tutorialButton;
    public Button openMap;
    public Button abandonRun;

    public void SetCorrectButton(GameState state)
    {
        tutorialButton.gameObject.SetActive(false);
        openMap.gameObject.SetActive(false);
        abandonRun.gameObject.SetActive(false);

        if (state == GameState.RUN)
            abandonRun.gameObject.SetActive(true);
        else if (state == GameState.TOWN)
        {
            if (TownManager.Instance.isTutorial)
                tutorialButton.gameObject.SetActive(true);
            else
                openMap.gameObject.SetActive(true);
        }

    }
}
