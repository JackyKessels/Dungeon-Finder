#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AbilityValidator
{
    [MenuItem("Tools/Abilities/Validate IDs")]
    public static void ValidateIDs()
    {
        string[] guids = AssetDatabase.FindAssets(
            "t:AbilityObject",
            new[]
            {
                "Assets/Scriptable Objects/Unit/Abilities/Invoker",
                "Assets/Scriptable Objects/Unit/Abilities/Reaver",
                "Assets/Scriptable Objects/Unit/Abilities/Shaman",
                "Assets/Scriptable Objects/Unit/Abilities/Templar",
            });

        var ids = new Dictionary<int, List<AbilityObject>>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ability = AssetDatabase.LoadAssetAtPath<AbilityObject>(path);

            if (ability == null)
                continue;

            // Missing IDs
            if (ability.id == -1)
            {
                Debug.LogError(
                    $"Missing ID: {ability.name} ({path})",
                    ability);
            }

            // Track IDs for duplicate detection
            if (!ids.TryGetValue(ability.id, out var list))
            {
                list = new List<AbilityObject>();
                ids.Add(ability.id, list);
            }

            list.Add(ability);
        }

        // Duplicate IDs
        foreach (var pair in ids)
        {
            if (pair.Key == -1)
                continue;

            if (pair.Value.Count > 1)
            {
                Debug.LogError(
                    $"Duplicate ID {pair.Key} found on {pair.Value.Count} abilities:");

                foreach (var ability in pair.Value)
                {
                    Debug.LogError(
                        $" - {ability.name}",
                        ability);
                }
            }
        }

        Debug.Log($"Ability ID validation complete. Scanned {guids.Length} abilities.");
    }
}
#endif