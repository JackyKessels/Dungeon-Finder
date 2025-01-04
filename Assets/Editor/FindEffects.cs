using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindEffects : EditorWindow
{
    [MenuItem("Tools/Find Effects with Special Effects")]
    public static void ShowFindEffectsWindow()
    {
        GetWindow<FindEffects>("Find Effects with Special Effects");
    }

    private List<EffectObject> effectObjectsWithSpecialEffects = new List<EffectObject>();
    private Vector2 scrollPosition = Vector2.zero; // Store the scroll position

    private void OnGUI()
    {
        if (GUILayout.Button("Find Effects with Special Effects"))
        {
            FindEffectsWithSpecialEffects();
        }

        GUILayout.Space(10);

        if (effectObjectsWithSpecialEffects.Count > 0)
        {
            GUILayout.Label("Effects with special effects:");

            // Begin a scrollable area
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300)); // Adjust the height as needed

            foreach (var effect in effectObjectsWithSpecialEffects)
            {
                GUILayout.Label($"{effect.name} (Effects: {string.Join(", ", effect.applicationSpecialEffects.Select(p => p.name).ToList())})");
            }

            // End the scrollable area
            GUILayout.EndScrollView();

            // Add a button to copy the contents to the clipboard
            if (GUILayout.Button("Copy to Clipboard"))
            {
                CopyToClipboard();
            }
        }
        else
        {
            GUILayout.Label("No effects with special effects found.");
        }
    }

    private void FindEffectsWithSpecialEffects()
    {
        effectObjectsWithSpecialEffects.Clear();

        // Find all assets of type Effect (replace with the class name of your ScriptableObject)
        string[] guids = AssetDatabase.FindAssets("t:EffectObject");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EffectObject effect = AssetDatabase.LoadAssetAtPath<EffectObject>(path);

            if (effect != null && effect.applicationSpecialEffects.Count > 0)
            {
                effectObjectsWithSpecialEffects.Add(effect);
            }
        }
    }

    private void CopyToClipboard()
    {
        // Create a string to copy, formatted with each effect on a new line
        var contentToCopy = new System.Text.StringBuilder();

        foreach (var effect in effectObjectsWithSpecialEffects)
        {
            contentToCopy.AppendLine($"{effect.name} (Effects: {string.Join(", ", effect.applicationSpecialEffects.Select(p => p.name).ToList())})");
        }

        // Copy the content to the clipboard
        GUIUtility.systemCopyBuffer = contentToCopy.ToString();
    }
}