using System;
using System.Linq;
using _ProjectFiles.WordCompletion.Scripts;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private bool clearPlayerData;
    [SerializeField] private BoostersManager boostersManager;
    
    private WordsQueue _queue;
    private int _currentLevelIndex;
    private LevelAsset _currentLevel;
    
    private CameraWidthApplier _cameraWidthApplier;
    [SerializeField] private GameCameraPositionAdjuster cameraAdjuster;

    private void Awake()
    {
        _cameraWidthApplier = new CameraWidthApplier(gameCamera);
        _cameraWidthApplier.CalculateScale();
        
        if (clearPlayerData)
        {
            SaveManager.ResetProgress();
        }
    }

    private void Start()
    {
        UpdateLevel(true);
        AudioManager.Instance.PlayMusic();
    }

    public void UpdateLevel(bool replay)
    {
        _currentLevelIndex = levelSelection.Load();
        boardView.ClearBoard();
        SelectLevel(replay);
        levelSelection.UpdateLevel();
    }
    
    public async void OnPlay()
    {
        fadeScreen.CloseMenu(EnableLevelControls);
        CameraGameTransition();

        await UniTask.Delay(1000);
        
        if (!SaveManager.Data.tutorialCompleted)
        {
            tutorialManager.StartTutorial(_queue);
        }
    }

    private void EnableLevelControls()
    {
        tracker.Enable(true);
    }

    public void SelectLevel(int levelId)
    {
        _currentLevelIndex = levelId;
        SelectLevel(false);
    }
    
    private async void SelectLevel(bool applyMenuCamera)
    {
        var levels = LevelAssetLoader.LoadAllLevels();

        LevelAsset level = null;

        try
        {
            level = levels.First(x => x.LevelId == _currentLevelIndex);
        }
        catch (Exception e)
        {
            Debug.LogError("Данный уровень не найден: " + _currentLevelIndex);
            return;
        }

        _currentLevel = level;
        _queue = new(level.Words.Select(x => x.word).ToList());

        var viewModel = new CurrentWordViewModel(_queue);
        wordView.ViewModel = viewModel;
        _queue.MoveNext();
        
        boardView.BuildBoard(level);
        environmentObjectsSpawner.Spawn(level.Board, boardView.RealCellSize);
        boardView.FillBoard();
        
        unusedLettersPoolView.DestroyPlatform();
        unusedLettersPoolView.BuildPool(level.PoolCount);
        
        wordCompletionPool.DestroyPlatform();
        wordCompletionPool.BuildPool(_queue);

        var units = boardView.Cells.Select(x => x.Occupant.gameObject).ToList();
        units.RemoveAll(x => x == null);

        await UniTask.WaitForEndOfFrame(this);
        cameraAdjuster.CalculateCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList());
        
        if (applyMenuCamera)
        {
            cameraAdjuster.ApplyPreferredCameraParameters();
        }
        
        boostersManager.Initialize();
    }

    public void OnLevelComplete()
    {
        SaveManager.Load();
        SaveManager.Data.extraCellBoostersCount += _currentLevel.ExtraCellBoostersCount;
        SaveManager.Data.unlockBoostersCount +=  _currentLevel.UnlockBoostersCount;
        SaveManager.Save();
    }
    
    public void CameraMenuTransition()
    {
        cameraAdjuster.CalculateCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList());
        cameraTransition.MoveToMenuState(cameraAdjuster.PreferredPosition, cameraAdjuster.PreferredSize);
    }

    public void CameraGameTransition()
    {
        cameraTransition.MoveToGameState(_cameraWidthApplier.PreferredSize);
    }
}
