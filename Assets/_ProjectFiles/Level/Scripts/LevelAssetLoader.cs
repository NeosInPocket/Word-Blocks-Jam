using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LevelAssetLoader
{
    public static List<LevelAsset> LoadAllLevelAssets(string folderPath)
    {
        List<LevelAsset> assets = new();

        string[] guids = AssetDatabase.FindAssets("t:LevelAsset", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            LevelAsset asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }

        return assets;
    }
}