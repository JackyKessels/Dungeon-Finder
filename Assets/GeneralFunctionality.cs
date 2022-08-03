using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFunctionality : MonoBehaviour
{
    public GameObject gameMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ToggleMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
    }
}
