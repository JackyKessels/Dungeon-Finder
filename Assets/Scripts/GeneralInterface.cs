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
            SaveAndExit();

            UnityEditor.EditorApplication.isPlaying = false;
        }
        #else
        {
            SaveAndExit();

            Application.Quit();
        }         
        #endif
    }

    private void SaveAndExit()
    {
        ProgressionManager.Instance.totalDefeats++;

        TeamManager.Instance.heroes.SetInvulnerable(true);
        TeamManager.Instance.heroes.ExpireEffects();
        TeamManager.Instance.heroes.SetInvulnerable(false);

        SaveManager.Instance.SaveGame();
    }

    public void ToggleMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
    }
}
