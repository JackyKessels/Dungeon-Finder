using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroPathGameObject : MonoBehaviour, IDescribable
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

    public Hero hero;
    public HeroPath heroPath = null;

    public void Setup(HeroPath _heroPath)
    {
        hero = HeroManager.Instance.CurrentHero();
        heroPath = _heroPath;

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
        passiveCounter.text = string.Format("[{0}/{1}]", heroPath.UnlockedCheckpoints(), heroPath.HighestCheckpoint());
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

        SetColor(checkpoint_1, false);
        SetColor(checkpoint_2, false);
        SetColor(checkpoint_3, false);
        SetColor(checkpoint_4, false);
        SetColor(checkpoint_5, false);

        if (points >= heroPath.checkpoints[1])
            SetColor(checkpoint_1, true);

        if (points >= heroPath.checkpoints[2])
            SetColor(checkpoint_2, true);

        if (points >= heroPath.checkpoints[3])
            SetColor(checkpoint_3, true);

        if (points >= heroPath.checkpoints[4])
            SetColor(checkpoint_4, true);

        if (points >= heroPath.checkpoints[5])
            SetColor(checkpoint_5, true);

    }

    private void SetColor(Image checkpoint, bool colored)
    {
        string color = colored ? "#00FF81" : "#444444";

        checkpoint.color = GeneralUtilities.ConvertString2Color(color);
    }



    public string GetDescription(TooltipObject tooltipInfo)
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
