using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryManager : MonoBehaviour
{
    #region Singleton
    public static BestiaryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public GameObject bestiaryPanelPrefab;
    public GameObject bestiaryPanelContainer;

    public void Setup()
    {
        if (GameManager.Instance.gameState != GameState.BATTLE)
        {
            return;
        }

        ObjectUtilities.ClearContainer(bestiaryPanelContainer);

        foreach (Enemy enemy in TeamManager.Instance.enemies.Members)
        {
            CreatePanel(enemy);
        }
    }

    public void ToggleBestiary()
    {
        if (bestiaryPanelContainer.activeSelf)
        {
            bestiaryPanelContainer.SetActive(false);
        }
        else
        {
            bestiaryPanelContainer.SetActive(true);
            Setup();
        }
    }

    private void CreatePanel(Enemy enemy)
    {
        var obj = ObjectUtilities.CreateSimplePrefab(bestiaryPanelPrefab, bestiaryPanelContainer);
        var panel = obj.GetComponent<BestiaryPanel>();
        panel.Setup(enemy);
    }
}
