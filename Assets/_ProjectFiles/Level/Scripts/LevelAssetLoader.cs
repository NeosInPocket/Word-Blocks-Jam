using System.Collections.Generic;
using UnityEngine;

public static class LevelAssetLoader
{
    public static List<LevelAsset> LoadAllLevels(string resourcesPath = "LevelAssets")
    {
        LevelAsset[] loaded = Resources.LoadAll<LevelAsset>(resourcesPath);
        return new List<LevelAsset>(loaded);
    }

    public static LevelAsset LoadLevelById(int levelId, string resourcesPath = "LevelAssets")
    {
        var levels = LoadAllLevels(resourcesPath);
        return levels.Find(l => l.LevelId == levelId);
    }
}