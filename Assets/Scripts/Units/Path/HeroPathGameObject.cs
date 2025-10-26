using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroPathGameObject : MonoBehaviour, IHasTooltip
{
    [SerializeField] Button pathButton;
    [SerializeField] TextMeshProUGUI pathLevel;
    [SerializeField] Slider progressBar;
    [SerializeField] GameObject passivesContainer;
    [SerializeField] TooltipObject tooltipObject;
    [SerializeField] TextMeshProUGUI passiveCounter;
    [SerializeField] GameObject passivePrefab;

    [SerializeField] Image checkpoint_1;
    [SerializeField] Image checkpoint_2;
    [SerializeField] Image checkpoint_3;
    [SerializeField] Image checkpoint_4;
    [SerializeField] Image checkpoint_5;

    [SerializeField] Image pathBlocker;

    public Hero hero;
    public HeroPath heroPath = null;

    public void Setup(HeroPath _heroPath)
    {
        hero = HeroManager.Instance.CurrentHero();
        heroPath = _heroPath;
        heroPath.pathGameObject = this;

        tooltipObject.state = CurrentState.HeroInformation;

        pathButton.GetComponent<Image>().sprite = heroPath.path.icon;

        UpdateInterface();

        AddPassives();
    }

    public void LevelUpPath()
    {
        if (hero.heroPathManager.points > 0 && heroPath.level < 30)
        {
            hero.heroPathManager.points--;

            heroPath.level++;

            UpdateInterface();

            hero.heroPathManager.UpdatePathBlockers();

            hero.heroPathManager.IncreaseHeroStats(heroPath);

            HeroManager.Instance.pathSystem.pathPoints.text = hero.heroPathManager.points.ToString();

            InventoryManager.Instance.UpdateCharacterAttributes(hero, -1);

            TooltipHandler.Instance.ShowTooltip(this, tooltipObject, new Vector2(transform.position.x, transform.position.y + 6));
        }
    }

    public void LevelDownPath()
    {
        if (hero.heroPathManager.points > 0)
        {
            hero.heroPathManager.points++;

            heroPath.level--;

            UpdateInterface();

            hero.heroPathManager.DecreaseHeroStats(heroPath);

            HeroManager.Instance.pathSystem.pathPoints.text = hero.heroPathManager.points.ToString();

            InventoryManager.Instance.UpdateCharacterAttributes(hero, -1);

            TooltipHandler.Instance.ShowTooltip(this, tooltipObject, new Vector2(transform.position.x, transform.position.y + 6));
        }
    }

    private void AddPassives()
    {
        ObjectUtilities.ClearContainer(passivesContainer);

        for (int i = 0; i < heroPath.path.passiveAbilities.Count; i++)
        {
            CreatePassiveAbility(heroPath.path.passiveAbilities[i], i, heroPath.unlockedPassives[i]);
        }
    }

    private void CreatePassiveAbility(PassiveAbility passiveAbility, int index, bool learned)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(passivePrefab, passivesContainer);

        obj.name = passiveAbility.name;

        obj.GetComponent<PathPassive>().Setup(passiveAbility, index, learned, this);
    }

    public void UpdateCounter()
    {
        var spentCheckpoints = heroPath.SpentCheckpoints().ToString();
        var availableCheckpoints = heroPath.AvailableCheckpoints().ToString();

        passiveCounter.text = $"[{spentCheckpoints}/{availableCheckpoints}]";
    }

    private void UpdateInterface()
    {
        pathLevel.text = heroPath.level.ToString();
        progressBar.value = heroPath.level;

        UpdateCounter();

        SetCheckpointColors();
    }

    private void SetCheckpointColors()
    {
        int points = heroPath.level;
        heroPath.checkpointsReached = 0;

        SetCheckpoint(checkpoint_1, false);
        SetCheckpoint(checkpoint_2, false);
        SetCheckpoint(checkpoint_3, false);
        SetCheckpoint(checkpoint_4, false);
        SetCheckpoint(checkpoint_5, false);

        if (points >= heroPath.checkpoints[1])
        {
            SetCheckpoint(checkpoint_1, true);
            heroPath.checkpointsReached++;
        }

        if (points >= heroPath.checkpoints[2])
        {
            SetCheckpoint(checkpoint_2, true);
            heroPath.checkpointsReached++;
        }

        if (points >= heroPath.checkpoints[3])
        {
            SetCheckpoint(checkpoint_3, true);
            heroPath.checkpointsReached++;
        }

        if (points >= heroPath.checkpoints[4])
        {
            SetCheckpoint(checkpoint_4, true);
            heroPath.checkpointsReached++;
        }

        if (points >= heroPath.checkpoints[5])
        {
            SetCheckpoint(checkpoint_5, true);
            heroPath.checkpointsReached++;
        }
    }

    private void SetCheckpoint(Image checkpoint, bool reached)
    {
        string color = reached ? ColorDatabase.ReachedCheckpoint : ColorDatabase.UnreachedCheckpoint;

        checkpoint.color = GeneralUtilities.ConvertString2Color(color);
    }

    public string GetCompleteTooltip(TooltipObject tooltipInfo)
    {
        string pathColor = "#FFEEA8";

        return string.Format("<color={1}><smallcaps><b>Path of the {0}</b></smallcaps></color>", heroPath.path.name, pathColor) + GetAttributes();
    }

    private string GetAttributes()
    {
        string attributesText = "\n";

        for (int i = 0; i < heroPath.path.attributes.Count; i++)
        {
            attributesText = attributesText + "\n+ " + heroPath.path.attributes[i].baseValue + MultiplierPercentage(heroPath.path.attributes[i]) + " " + GeneralUtilities.GetCorrectAttributeName(heroPath.path.attributes[i].attributeType) + CurrentIncrease(heroPath.path.attributes[i]);
        }

        return attributesText;
    }

    public void SetPathBlocker(bool block)
    {
        pathBlocker.gameObject.SetActive(block);
    }

    private string MultiplierPercentage(Attribute attribute)
    {
        switch (attribute.attributeType)
        {
            case AttributeType.HealingMultiplier:
                return "%";
            case AttributeType.PhysicalMultiplier:
                return "%";
            case AttributeType.FireMultiplier:
                return "%";
            case AttributeType.IceMultiplier:
                return "%";
            case AttributeType.NatureMultiplier:
                return "%";
            case AttributeType.ArcaneMultiplier:
                return "%";
            case AttributeType.HolyMultiplier:
                return "%";
            case AttributeType.ShadowMultiplier:
                return "%";
            case AttributeType.CritMultiplier:
                return "%";
            case AttributeType.SacrificialMultiplier:
                return "%";
            default:
                return "";
        }
    }
    
    private string CurrentIncrease(Attribute attribute)
    {
        string currentColor = "#9DFF68";

        return string.Format("<color={2}> (+{0}{1})</color>", attribute.baseValue * heroPath.level, MultiplierPercentage(attribute), currentColor);
    }
}
