using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialsContainer;

    public GameObject learningAbilitiesPrefab;
    public GameObject usingPathSystemPrefab;

    private bool skipTutorials = false;

    private bool showLearningAbilities = false;
    private bool showUsingPathSystem = false;

    public bool SkipTutorials 
    { 
        get => skipTutorials; 
        set => skipTutorials = value; 
    }
    public bool ShowLearningAbilities
    { 
        get => showLearningAbilities; 
        set => showLearningAbilities = value; 
    }
    public bool ShowUsingPathSystem 
    { 
        get => showUsingPathSystem; 
        set => showUsingPathSystem = value; 
    }

    public void TutorialLearningAbilities()
    {
        if (SkipTutorials)
            return;

        if (ShowLearningAbilities)
            return;

        ObjectUtilities.CreateSimplePrefab(learningAbilitiesPrefab, tutorialsContainer);
        ShowLearningAbilities = true;
    }

    public void TutorialPathSystem()
    {
        if (SkipTutorials)
            return;

        if (ShowUsingPathSystem)
            return;

        ObjectUtilities.CreateSimplePrefab(usingPathSystemPrefab, tutorialsContainer);
        ShowUsingPathSystem = true;
    }
}
