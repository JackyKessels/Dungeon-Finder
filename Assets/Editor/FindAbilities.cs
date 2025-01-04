using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindAbilities : EditorWindow
{
    [MenuItem("Tools/Find Abilities with Reset Chance")]
    public static void ShowFindAbilitiesWindow()
    {
        GetWindow<FindAbilities>("Find Abilities with Reset Chance");
    }

    private List<ActiveAbility> abilitiesWithResetChance = new List<ActiveAbility>();

    private void OnGUI()
    {
        if (GUILayout.Button("Find Abilities with Reset Chance"))
        {
            FindAbilitiesWithResetChance();
        }

        GUILayout.Space(10);

        if (abilitiesWithResetChance.Count > 0)
        {
            GUILayout.Label("Abilities with non-zero Reset Chance:");
            foreach (var ability in abilitiesWithResetChance)
            {
                GUILayout.Label(ability.name + " (ResetChance: " + ability.resetChance + ")");
            }
        }
        else
        {
            GUILayout.Label("No abilities with non-zero Reset Chance found.");
        }
    }

    private void FindAbilitiesWithResetChance()
    {
        abilitiesWithResetChance.Clear();

        // Find all assets of type Ability (replace 'Ability' with the class name of your ScriptableObject)
        string[] guids = AssetDatabase.FindAssets("t:ActiveAbility");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ActiveAbility ability = AssetDatabase.LoadAssetAtPath<ActiveAbility>(path);

            if (ability != null && ability.resetChance != 0)
            {
                abilitiesWithResetChance.Add(ability);
            }
        }
    }
}