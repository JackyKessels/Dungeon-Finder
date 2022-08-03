using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleExperienceBar : MonoBehaviour
{
    public Slider bar;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI nextLevel;
    public TextMeshProUGUI currentExperience;

    private ExperienceManager experienceManager;

    private int level;

    private Coroutine routine;

    public void Setup(int experienceReward)
    {
        experienceManager = TeamManager.Instance.experienceManager;

        level = experienceManager.currentLevel;

        UpdateLevel(level);

        UpdateExperienceBar(experienceManager.currentExperience, experienceReward);

        TeamManager.Instance.RewardExperienceToTeam(experienceReward);
    }

    private void UpdateExperienceBar(float currentExperience, float experienceReward)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FillRoutine(currentExperience, experienceReward));
    }

    private IEnumerator FillRoutine(float current, float increase, float duration = 1f)
    {
        float experienceRequirement = ExperienceManager.ExperienceToNextLevel(level);
        float remaining = 0;

        if (current + increase > experienceRequirement)
        {
            remaining = (current + increase) - experienceRequirement;
        }

        float currentProgress = current / experienceRequirement;
        float increaseProgress = increase / experienceRequirement;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float percent = time / duration;
            bar.value = currentProgress + increaseProgress * percent;

            float result = bar.value * experienceRequirement;

            currentExperience.text = (int)result + " / " + experienceRequirement;

            yield return null;
        }

        if (bar.value >= 1)
        {
            LevelUp(remaining);
        }
    }

    private void LevelUp(float remaining)
    {
        UpdateLevel(level + 1);
        UpdateExperienceBar(0, remaining);
    }

    private void UpdateLevel(int level)
    {
        this.level = level;
        currentLevel.text = level.ToString();
        nextLevel.text = (level + 1).ToString();
    }
}
