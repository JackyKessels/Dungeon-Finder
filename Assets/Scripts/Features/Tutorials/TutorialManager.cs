using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialsContainer;

    public GameObject learningAbilitiesPrefab;
    public GameObject usingPathSystemPrefab;

    private bool skipTutorials = false;

    private bool learningAbilities = false;
    private bool usingPathSystem = false;

    public bool SkipTutorials { 
        get => skipTutorials; 
        set => skipTutorials = value; 
    }

    public void ShowLearningAbilities()
    {
        if (skipTutorials)
            return;

        if (learningAbilities)
            return;

        ObjectUtilities.CreateSimplePrefab(learningAbilitiesPrefab, tutorialsContainer);
        learningAbilities = true;
    }

    public void UsingPathSystem()
    {
        if (skipTutorials)
            return;

        if (usingPathSystem)
            return;

        ObjectUtilities.CreateSimplePrefab(usingPathSystemPrefab, tutorialsContainer);
        usingPathSystem = true;
    }
}
