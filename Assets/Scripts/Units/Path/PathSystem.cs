using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PathSystem : MonoBehaviour
{
    public GameObject pathsContainer;

    public TextMeshProUGUI pathPoints;

    public GameObject pathPrefab;

    public void Setup(Hero hero)
    {
        ObjectUtilities.ClearContainer(pathsContainer);

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
}
