using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codex : MonoBehaviour
{
    private GameManager gameManager;
    private TeamManager teamManager;
    private ProgressionManager progressionManager;

    public CodexObject codexDatabase;

    public GameObject codexObject;
    public TextMeshProUGUI dungeonName;
    public GameObject iconPrefab;

    public GameObject equipmentContainer;
    public GameObject consumablesContainer;

    private int nextDungeon = 0;
    private int itemLevel = 1;

    private void Start()
    {
        gameManager = GameManager.Instance;
        teamManager = TeamManager.Instance;
        progressionManager = ProgressionManager.Instance;
    }

    private void Update()
    {
        if (gameManager.gameState == GameState.TOWN ||
            gameManager.gameState == GameState.RUN)
        {
            if (KeyboardHandler.PreviousPage() && codexObject.activeSelf)
            {
                PreviousDungeon();
            }

            if (KeyboardHandler.NextPage() && codexObject.activeSelf)
            {
                NextDungeon();
            }

            if (KeyboardHandler.OpenCodex())
            {
                OpenCodex();
            }
        }
    }

    public void PreviousDungeon()
    {
        if (nextDungeon > 0)
        {
            nextDungeon--;
        }
        else
        {
            nextDungeon = codexDatabase.dungeonEntries.Count - 1;
        }

        Setup();
    }

    public void NextDungeon()
    {
        if (nextDungeon < codexDatabase.dungeonEntries.Count - 1)
        {
            nextDungeon++;
        }
        else
        {
            nextDungeon = 0;
        }

        Setup();
    }

    public void OpenCodex()
    {
        if (!codexObject.activeSelf)
        {
            codexObject.SetActive(true);
        }
        else
        {
            codexObject.SetActive(false);
        }

        Setup();
    }

    private void Setup()
    {
        dungeonName.text = codexDatabase.dungeonEntries[nextDungeon].dungeon.name;

        gameManager.audioSourceSFX.PlayOneShot(GameAssets.i.click);

        ObjectUtilities.ClearContainer(equipmentContainer);

        foreach (ItemObject equipment in codexDatabase.dungeonEntries[nextDungeon].equipments)
        {
            AddItem(equipment, equipmentContainer);
        }

        ObjectUtilities.ClearContainer(consumablesContainer);

        foreach (ItemObject consumable in codexDatabase.dungeonEntries[nextDungeon].consumables)
        {
            AddItem(consumable, consumablesContainer);
        }
    }

    private void AddItem(ItemObject itemObject, GameObject container)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(iconPrefab, container);

        CodexItem codexItem = obj.GetComponent<CodexItem>();

        if (progressionManager.discoveredItems.Contains(itemObject.item.id) || GameManager.Instance.TEST_MODE)
        {
            codexItem.Setup(true, itemObject, itemLevel);
        }
        else
        {
            codexItem.Setup(false, itemObject, itemLevel);
        }
    }

    public void DiscoverItem(ItemObject itemObject)
    {
        if (!progressionManager.discoveredItems.Contains(itemObject.item.id))
        {
            progressionManager.discoveredItems.Add(itemObject.item.id);
        }
    }

    public void UpdateItemLevel(string input)
    {
        if (int.TryParse(input, out int result))
        {
            itemLevel = result;
            Setup();
        }
    }
}
