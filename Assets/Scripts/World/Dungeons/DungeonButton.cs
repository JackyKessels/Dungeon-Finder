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

    public void Setup(DungeonListEntry _dungeon)
    {
        dungeon = _dungeon.dungeon;

        GeneralUtilities.SetText(title, dungeon);

        var recommendedMinimumLevel = dungeon.recommendedMinimumLevel.ToString();
        var recommendedMaximumLevel = dungeon.recommendedMaximumLevel.ToString();

        recommendedLevel.text = $"Loot Level: {recommendedMinimumLevel} - {recommendedMaximumLevel}";
        button.onClick.AddListener(delegate { TownManager.Instance.StartDungeon(dungeon); });

        LockDungeon(_dungeon.locked);
    }

    public void EnterDungeon()
    {
        Debug.Log("Entering: " + dungeon.name);
    }

    public void LockDungeon(bool locked)
    {
        lockedScreen.SetActive(locked);
        button.interactable = !locked;
    }
}
