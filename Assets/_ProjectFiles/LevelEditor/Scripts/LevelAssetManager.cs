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
    public List<LevelAsset> newLevels;
    public List<LevelAsset> oldLevels;

    public void ModifyAllLevels()
    {
        foreach (var asset in newLevels)
        {
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

    public void ApplyBoosters()
    {
        for (int i = 0; i < oldLevels.Count; i++)
        {
            newLevels[i].ApplyBoosters(oldLevels[i].ExtraCellBoostersCount, oldLevels[i].UnlockBoostersCount);
            EditorUtility.SetDirty(newLevels[i]);
        }
        
        AssetDatabase.SaveAssets();
    }
}

#endif