using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    private List<LevelAsset> LoadedLevels;
    private int SelectedLevelIndex;
    private CurrentLevelInfo levelInfo;
    private const string _levelsAssetsPath = "Assets/_ProjectFiles/Level/LevelAssets";
    
    private void Awake()
    {
        dropdown.interactable = false;
        
        LoadedLevels = LevelAssetLoader.LoadAllLevels();;
        dropdown.interactable = true;
        
        List<string> options = new List<string>();
        
        foreach (var level in LoadedLevels)
        {
            options.Add($"Level {level.levelId}");
        }
        
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        levelInfo = new CurrentLevelInfo(LoadedLevels[0].levelId);
    }

    public void LoadLevel()
    {
        levelInfo.LevelId = dropdown.value + 1;
        SceneManager.LoadScene(1);
    }
}
