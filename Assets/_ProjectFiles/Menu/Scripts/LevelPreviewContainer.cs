using UnityEngine;

public class LevelPreviewContainer : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private BoardEnvironmentObjectsSpawner environmentObjectsSpawner;
    
    public void ShowPreview(LevelAsset levelAsset)
    {
        boardView.BuildBoard(levelAsset);
        environmentObjectsSpawner.Spawn(levelAsset.Board);
    }
}
