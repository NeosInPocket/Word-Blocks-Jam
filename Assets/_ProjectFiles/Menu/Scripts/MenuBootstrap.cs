using UnityEngine;

public class MenuBootstrap : MonoBehaviour
{
    [SerializeField] private LevelPreviewContainer levelPreview;
    [SerializeField] private LevelSelection levelSelection;

    private void Start()
    {
        SaveManager.Load();
        var currentLevelId = SaveManager.Data.levelsCompleted + 1;
        var currentLevel = LevelAssetLoader.LoadLevelById(currentLevelId);
        
        levelSelection.UpdateLevel();
        levelPreview.ShowPreview(currentLevel);
    }
}
