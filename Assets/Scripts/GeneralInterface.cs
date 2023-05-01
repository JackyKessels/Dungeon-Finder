using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInterface : MonoBehaviour
{
    public GameObject gameMenu;

    void Update()
    {
        if (KeyboardHandler.Escape() && GameManager.Instance.gameState != GameState.START_SCREEN)
        {
            ToggleMenu();
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        {
            //TeamManager.Instance.SaveTeam();

            UnityEditor.EditorApplication.isPlaying = false;
        }
        #else
        {
            //TeamManager.Instance.SaveTeam();

            Application.Quit();
        }         
        #endif
    }

    public void ToggleMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
    }
}
