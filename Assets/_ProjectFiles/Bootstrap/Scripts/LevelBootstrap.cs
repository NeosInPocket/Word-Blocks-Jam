using System;
using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private int levelIndexPlaceholder;
    [SerializeField] private BoardView boardView;
    private const string _levelsAssetsPath = "Assets/_ProjectFiles/Level/LevelAssets";
    
    private void Awake()
    {
        boardView.ClearBoard();
        SelectLevel();
    }

    private void SelectLevel()
    {
        var levels = LevelAssetLoader.LoadAllLevelAssets(_levelsAssetsPath);

        LevelAsset level = null;

        try
        {
            level = levels[levelIndexPlaceholder];
        }
        catch (Exception e)
        {
            Debug.LogError("Данный уровень не найден");
        }
        
        //boardView.BuildBoard(level.boardArray);
    }
}
