#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using _ProjectFiles._Features.LetterDispenser.Scripts;

[ExecuteAlways]
public class LevelAssetManager : MonoBehaviour
{
    [SerializeField] private LetterUnitColorData data;
    public List<LevelAsset> allLevels = new List<LevelAsset>();

    private void Awake()
    {
        CollectAllLevels();
    }

    public void CollectAllLevels()
    {
        allLevels.Clear();
        string[] guids = AssetDatabase.FindAssets("t:LevelAsset", new[] { "Assets/Resources/LevelAssets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelAsset asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(path);
            if (asset != null)
                allLevels.Add(asset);
        }
        EditorUtility.SetDirty(this);
    }

    public void ModifyAllLevels()
    {
        foreach (var asset in allLevels)
        {
            if (asset == null) continue;

            int counter = 0;
            foreach (var word in asset.Words)
            {
                var elements = asset.Board.Where(x => x.color == word.color).ToList();
                
                foreach (var element in elements)
                {
                    element.color = data.Colors[counter];
                }

                word.color = data.Colors[counter];
                counter++;
            }
            
            EditorUtility.SetDirty(asset);
        }
        AssetDatabase.SaveAssets();
    }
}

#endif