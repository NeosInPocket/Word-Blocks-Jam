using System;
using System.Linq;
using _ProjectFiles.WordCompletion.Scripts;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private CurrentWordView wordView;
    [SerializeField] private UnusedLettersPoolView unusedLettersPoolView;
    [SerializeField] private WordCompletionPool wordCompletionPool;
    [SerializeField] private BoardEnvironmentObjectsSpawner environmentObjectsSpawner;
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private FadeScreenView fadeScreen;
    [SerializeField] private NavMeshSurface meshSurface;
    [SerializeField] private Image maskImage;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private CameraTransition cameraTransition;
    
    private WordsQueue _queue;
    private int _currentLevel;
    private GameCameraPositionAdjuster _cameraAdjuster;
    private Vector3 _optimalCameraPosition;
    private float _optimalCameraSize;
    
    
    private void Start()
    {
        UpdateLevel();
        //cameraTransition.SetDefaultState();
        AudioManager.Instance.PlayMusic();
    }

    public void UpdateLevel()
    {
        _currentLevel = levelSelection.Load();
        boardView.ClearBoard();
        SelectLevel();
        levelSelection.UpdateLevel();
    }
    
    public void OnPlay()
    {
        fadeScreen.CloseMenu(EnableLevelControls);
    }

    private void EnableLevelControls()
    {
        tracker.Enable(true);
    }

    public void SelectLevel(int levelId)
    {
        _currentLevel = levelId;
        SelectLevel();
    }
    
    private void SelectLevel()
    {
        var levels = LevelAssetLoader.LoadAllLevels();

        LevelAsset level = null;

        try
        {
            level = levels.First(x => x.LevelId == _currentLevel);
        }
        catch (Exception e)
        {
            Debug.LogError("Данный уровень не найден: " + _currentLevel);
            return;
        }

        _queue = new(level.Words.Select(x => x.word).ToList());

        var viewModel = new CurrentWordViewModel(_queue);
        wordView.ViewModel = viewModel;
        _queue.MoveNext();
        
        boardView.BuildBoard(level);
        environmentObjectsSpawner.Spawn(level.Board, boardView.RealCellSize);
        boardView.FillBoard();
        
        unusedLettersPoolView.BuildPool(level.PoolCount);
        wordCompletionPool.BuildPool(_queue);

        var units = boardView.Cells.Select(x => x.Occupant.gameObject).ToList();
        units.RemoveAll(x => x == null);
        
        // _cameraAdjuster = new GameCameraPositionAdjuster(units, maskImage, gameCamera);
        // _cameraAdjuster.GetOptimalCamera(out Vector3 optimalCameraPos, out var size);
        //
        // _optimalCameraPosition = optimalCameraPos;
        // _optimalCameraSize = size;
        //
        // cameraTransition.Initialize(optimalCameraPos, size);
    }

    public void CameraMenuTransition()
    {
        cameraTransition.MoveToMenuState();
    }
    
}
