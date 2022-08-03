using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonButton : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI recommendedLevel;
    [SerializeField] GameObject lockedScreen;
    [SerializeField] Button button;

    private Dungeon dungeon;

    public void Setup(Dungeon _dungeon)
    {
        dungeon = _dungeon;

        title.text = dungeon.name;
        title.color = dungeon.nameColor;

        recommendedLevel.text = string.Format("Recommended Level: {0} - {1}", dungeon.recommendedMinimumLevel, dungeon.recommendedMaximumLevel);
        button.onClick.AddListener(delegate { TownManager.Instance.StartRun(dungeon); });
    }

    public void EnterDungeon()
    {
        Debug.Log("Entering: " + dungeon.name);
    }
}
