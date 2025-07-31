using System;
using System.Linq;
using _ProjectFiles.WordCompletion.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private CurrentWordView wordView;
    [SerializeField] private UnusedLettersPoolView unusedLettersPoolView;
    [SerializeField] private WordCompletionPool wordCompletionPool;
    private const string _levelsAssetsPath = "Assets/_ProjectFiles/Level/LevelAssets";
    private WordsQueue _queue;
    
    private void Awake()
    {
        boardView.ClearBoard();
        SelectLevel();
    }

    private void SelectLevel()
    {
        var levels = LevelAssetLoader.LoadAllLevels();

        LevelAsset level = null;

        try
        {
            level = levels.First(x => x.levelId == CurrentLevelInfo.Instance.LevelId);
        }
        catch (Exception e)
        {
            Debug.LogError("Данный уровень не найден");
        }

        _queue = new(level.words.Select(x => x.word).ToList());

        var viewModel = new CurrentWordViewModel(_queue);
        wordView.ViewModel = viewModel;
        _queue.MoveNext();
        
        boardView.BuildBoard(level);
        
        unusedLettersPoolView.Initialize(level.poolCount);
        wordCompletionPool.BuildPool(_queue);
    }
}
