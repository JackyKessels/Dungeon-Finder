using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour
{
    #region Singleton
    public static ProgressionManager Instance { get; private set; }

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

    public CampaignManager campaignManager;
    public EndlessManager endlessManager;

    [Header("[ Statistics ]")]
    public int totalVictories = 0;
    public int totalDefeats = 0;
    public int totalBossesDefeated = 0;
    public int totalMonstersDefeated = 0;

    [Header("[-- Campaign --]")]
    [Header("[ Events ]")]
    public bool firstDeath = false;

    [Header("[ Progression Unlocks ]")]
    [Header("Paths")]
    public bool unlockedPaths = false;
    public Button unlockedPaths_Button;
    public Dungeon unlockedPaths_Dungeon;
    public int unlockedPaths_Floor;

    [Header("Fourth Ability")]
    public bool unlockedFourthAbility = false;
    public Dungeon unlockedFourthAbility_Dungeon;

    [Header("Enchanter Upgrade")]
    public bool unlockedEnchanterUpgrade = false;
    public Dungeon unlockedEnchanterUpgrade_Dungeon;

    [Header("[-- Endless --]")]
    public int endlessLevel;

    public GameMode GameMode => GameManager.Instance.gameMode;

    public void ResetProgression()
    {
        totalVictories = 0;
        totalDefeats = 0;
        totalBossesDefeated = 0;
        totalMonstersDefeated = 0;

        switch (GameMode)
        {
            case GameMode.Campaign:
                {
                    firstDeath = false;

                    unlockedPaths = false;
                    unlockedFourthAbility = false;
                    unlockedEnchanterUpgrade = false;
                }
                break;
            case GameMode.Endless:
                {
                    endlessLevel = 1;
                }
                break;
            default:
                break;
        }
    }

    public void FirstDeath()
    {
        if (firstDeath)
            return;

        firstDeath = true;

        // Reward first death stuff
        // Give each hero 1 option out of 3 random abilities
        // Give ability currency for now
        GameManager.Instance.currencyHandler.IncreaseCurrency(new Currency(CurrencyType.Spirit, 30));

        NotificationObject.SendNotification("That was rough... maybe these 30 Spirit Fragments will help you.", new List<Reward>() { new Reward(new Currency(CurrencyType.Spirit, 30)) });

        Debug.Log("This is your first death.");   
    }

    public void UnlockPaths(Dungeon dungeon, int floor)
    {
        if (unlockedPaths)
            return;

        if (unlockedPaths_Dungeon == dungeon && unlockedPaths_Floor == floor)
        {
            unlockedPaths = true;

            SetPathButtonState();

            NotificationObject.SendNotification("The Path system has been unlocked.");
        }
    }

    public void SetPathButtonState()
    {
        TooltipObject tooltip = unlockedPaths_Button.GetComponent<TooltipObject>();

        if (unlockedPaths)
        {
            unlockedPaths_Button.interactable = true;

            Destroy(tooltip);

            Debug.Log("Unlocked Paths");
        }
        else
        {
            unlockedPaths_Button.interactable = false;

            string color = ColorDatabase.GeneralInformation();

            tooltip.useGenericTooltip = true;
            tooltip.genericTooltip = string.Format("Unlocks when <color={0}>Si'keth Bloodclaw</color> has been defeated.\n\nHe can be found in the <color={0}>Crimson Burrows</color>.", color);
        }
    }

    public void UnlockFourthAbility()
    {
        if (unlockedFourthAbility)
            return;

        unlockedFourthAbility = true;

        SpellbookManager.Instance.activeAbilities[3].locked = false;

        NotificationObject.SendNotification("Your fourth ability slot has been unlocked.");
    }

    public void UnlockEnchanterUpgrade()
    {
        if (unlockedEnchanterUpgrade)
            return;

        unlockedEnchanterUpgrade = true;

        TownManager.Instance.enchanter.abilityOptions = 3;
    }
}
