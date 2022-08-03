using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    #region Singleton

    public static BattleHUD Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 instance of BattleHUD found!");
            return;
        }

        Instance = this;
    }

    #endregion

    private TeamManager teamManager;

    public UnitHUD[] heroHUDs = new UnitHUD[3];
    public UnitHUD[] enemyHUDs = new UnitHUD[3];

    private void Start()
    {
        teamManager = TeamManager.Instance;

        HideHUDS();
    }

    public void Refresh()
    {
        HideHUDS();

        for (int i = 0; i < teamManager.heroes.Members.Count; i++)
        {
            if (!teamManager.heroes.Members[i].statsManager.isDead)
            {
                int battleNumber = teamManager.heroes.Members[i].battleNumber;

                heroHUDs[battleNumber].Show(true);
                heroHUDs[battleNumber].Setup(teamManager.heroes.GetUnit(i));
                heroHUDs[battleNumber].UpdateBuffs();
            }
        }

        for (int i = 0; i < teamManager.enemies.Members.Count; i++)
        {
            if (!teamManager.enemies.Members[i].statsManager.isDead)
            {
                int battleNumber = teamManager.enemies.Members[i].battleNumber;

                enemyHUDs[battleNumber].Show(true);
                enemyHUDs[battleNumber].Setup(teamManager.enemies.GetUnit(battleNumber));
                enemyHUDs[battleNumber].UpdateBuffs();
            }
        }
    }

    private void HideHUDS()
    {
        foreach (UnitHUD unitHUD in heroHUDs)
        {
            unitHUD.Show(false);
        }

        foreach (UnitHUD unitHUD in enemyHUDs)
        {
            unitHUD.Show(false);
        }
    }
}
