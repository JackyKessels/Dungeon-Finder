using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageMeterManager : MonoBehaviour
{
    #region Singleton
    public static DamageMeterManager Instance { get; private set; }

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

    private BattleManager battleManager;

    public GameObject damageMeterPrefab;
    public GameObject damageMeterContainer;

    public List<DamageMeterObject> damageMeters;

    private bool initialized = false;

    private void Start()
    {
        battleManager = BattleManager.Instance;
    }

    public void InitializeDamageMeters()
    {
        foreach (Unit unit in TeamManager.Instance.heroes.Members)
        {
            CreateDamageMeter(unit);
        }

        initialized = true;

        ToggleDamageMeters();
    }

    public void CreateDamageMeter(Unit unit)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(damageMeterPrefab, damageMeterContainer);

        DamageMeterObject damageMeter = obj.GetComponent<DamageMeterObject>();
        damageMeter.InitializeDamageMeter(unit);
        damageMeters.Add(damageMeter);
    }

    public void ClearDamageMeters()
    {
        foreach (DamageMeterObject damageMeter in damageMeters)
        {
            damageMeter.ClearRecords();
        }
    }

    public void ToggleDamageMeters()
    {
        foreach (DamageMeterObject damageMeter in damageMeters)
        {
            ToggleDamageMeter(damageMeter);
        }
    }

    private void ToggleDamageMeter(DamageMeterObject damageMeter)
    {
        if (!initialized)
            return;

        damageMeter.damageMeterWindow.gameObject.SetActive(!damageMeter.damageMeterWindow.gameObject.activeSelf);
    }

}
