using System;
using System.Linq;
using _ProjectFiles.WordCompletion.Scripts;
using Unity.AI.Navigation;
using UnityEngine;

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
    private WordsQueue _queue;
    private int _currentLevel;
    
    private void Start()
    {
        _currentLevel = levelSelection.Load();
        
        boardView.ClearBoard();
        SelectLevel();
    }

    public void OnPlay()
    {
        fadeScreen.CloseMenu(EnableLevelControls);
    }

    private void EnableLevelControls()
    {
        tracker.Enable(true);
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
            Debug.LogError("Данный уровень не найден");
        }

        _queue = new(level.Words.Select(x => x.word).ToList());

        var viewModel = new CurrentWordViewModel(_queue);
        wordView.ViewModel = viewModel;
        _queue.MoveNext();
        
        boardView.BuildBoard(level);
        environmentObjectsSpawner.Spawn(level.Board, boardView.RealCellSize);
        
        unusedLettersPoolView.BuildPool(level.PoolCount);
        wordCompletionPool.BuildPool(_queue);
        
        //meshSurface.BuildNavMesh();
    }
}
