using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class ReserializeAllScriptableObjects
{
    [MenuItem("Utils/Reserialize/All Scriptable Objects")]
    private static void onClick_ReserializeAllAssets()
    {
        // Get all ScriptableObject assets (adjust the type if necessary)
        AssetDatabase.ForceReserializeAssets(GetAllAssetsOfScriptableObject<ScriptableObject>());
    }

    public static string[] GetAllAssetsOfScriptableObject<T>() where T : ScriptableObject
    {
        List<string> _assets = new List<string>();

        foreach (string _path in AssetDatabase.GetAllAssetPaths())
        {
            if (_path.Contains(".asset"))
            {
                T _asset = AssetDatabase.LoadAssetAtPath<T>(_path);

                if (_asset != null)
                {
                    _assets.Add(_path);
                }
            }
        }

        return _assets.ToArray();
    }
}
