using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PathSystem : MonoBehaviour
{
    public TextMeshProUGUI pathTitle;
    public GameObject pathsContainer;

    public TextMeshProUGUI pathPoints;

    public GameObject pathPrefab;

    private Hero hero;

    public void Setup(Hero hero)
    {
        this.hero = hero;
    }

    public void Setup()
    {
        ObjectUtilities.ClearContainer(pathsContainer);

        pathTitle.text = hero.heroClass + " Paths";
        pathPoints.text = hero.heroPathManager.points.ToString();

        foreach (HeroPath path in hero.heroPathManager.paths)
        {
            CreateHeroPath(path, pathsContainer);
        }
    }

    private void CreateHeroPath(HeroPath path, GameObject container)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(pathPrefab, container);

        obj.name = path.path.name;

        HeroPathGameObject heroPathPrefab = obj.GetComponentInChildren<HeroPathGameObject>();
        heroPathPrefab.Setup(path);
    }

    public void PathSystemButton()
    {
        if (gameObject.activeSelf)
        {
            ClosePathSystem();
        }
        else
        {
            OpenPathSystem();
        }
    }

    private void OpenPathSystem()
    {
        gameObject.SetActive(true);

        Setup();
    }

    public void ClosePathSystem()
    {
        gameObject.SetActive(false);
    }
}
