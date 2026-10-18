//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//public class FindEffectsWithInteractions : EditorWindow
//{
//    [MenuItem("Tools/Find Effects with interactions")]
//    public static void ShowFindEffectsWindow()
//    {
//        GetWindow<FindEffectsWithInteractions>("Find Effects with interactions");
//    }

//    private List<EffectObject> effectObjectsWithInteractions = new List<EffectObject>();
//    private Vector2 scrollPosition = Vector2.zero; // Store the scroll position

//    private void OnGUI()
//    {
//        if (GUILayout.Button("Find Effects with interactions"))
//        {
//            FindEffects();
//        }

//        GUILayout.Space(10);

//        if (effectObjectsWithInteractions.Count > 0)
//        {
//            GUILayout.Label("Effects with interactions:");

//            // Begin a scrollable area
//            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300)); // Adjust the height as needed

//            foreach (var effect in effectObjectsWithInteractions)
//            {
//                GUILayout.Label($"{effect.name}");
//            }

//            // End the scrollable area
//            GUILayout.EndScrollView();

//            // Add a button to copy the contents to the clipboard
//            if (GUILayout.Button("Copy to Clipboard"))
//            {
//                CopyToClipboard();
//            }
//        }
//        else
//        {
//            GUILayout.Label("No effects with interactions found.");
//        }
//    }

//    private void FindEffects()
//    {
//        effectObjectsWithInteractions.Clear();

//        // Find all assets of type Effect (replace with the class name of your ScriptableObject)
//        string[] guids = AssetDatabase.FindAssets("t:EffectObject");
//        foreach (string guid in guids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);
//            EffectObject effect = AssetDatabase.LoadAssetAtPath<EffectObject>(path);

//            if (effect != null && effect.effectInteractions.Count > 0)
//            {
//                effectObjectsWithInteractions.Add(effect);
//            }
//        }
//    }

//    private void CopyToClipboard()
//    {
//        // Create a string to copy, formatted with each effect on a new line
//        var contentToCopy = new System.Text.StringBuilder();

//        foreach (var effect in effectObjectsWithInteractions)
//        {
//            contentToCopy.AppendLine($"{effect.name}");
//        }

//        // Copy the content to the clipboard
//        GUIUtility.systemCopyBuffer = contentToCopy.ToString();
//    }
//}