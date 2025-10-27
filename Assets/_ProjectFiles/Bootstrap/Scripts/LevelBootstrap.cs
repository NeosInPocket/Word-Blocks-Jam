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
    [SerializeField] private BoostersCanvas boostersCanvas;
    [SerializeField] private int overrideLevel;
    [SerializeField] private AudioManager manager;
    
    private WordsQueue _queue;
    private int _currentLevelIndex;
    private LevelAsset _currentLevel;
    
    [SerializeField] private GameCameraPositionAdjuster cameraAdjuster;

    private void Awake()
    {
        Application.targetFrameRate = 120;

        if (clearPlayerData)
        {
            SaveManager.ResetProgress();
        }
    }

    private void Start()
    {
        UpdateLevel(true);
        AudioManager.Instance.PlayMusic();
        
        SaveManager.Load();
        SaveManager.Data.gameSession++;
        SaveManager.Save();
        
        AdjustEventManager.Instance.SetGlobalParams(SaveManager.Data.gameSession);
    }

    public void UpdateLevel(bool instantCameraApply)
    {
        levelSelection.Load();
        _currentLevelIndex = levelSelection.BoardLevelNumber;
        boardView.ClearBoard();
        SelectLevel(instantCameraApply);
        levelSelection.UpdateLevel();
    }
    
    public async void OnPlay()
    {
        fadeScreen.CloseMenu(EnableLevelControls);
        boostersCanvas.gameObject.SetActive(true);
        CameraGameTransition();
        
        LevelTimer.Instance.StartLevel();
        AnalyticsManager.Instance.SendLevelStartedEvent(levelSelection.AnalyticsLevelNumber);
        AdjustEventManager.Instance.TrackLevelStart("qjbgpm", levelSelection.BoardLevelNumber, $"Level {levelSelection.BoardLevelNumber}");
        
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
        SelectLevel(true);
    }
    
    private void SelectLevel(bool instantCameraApply)
    {
        var levels = LevelAssetLoader.LoadAllLevels();

        LevelAsset level = null;

        if (overrideLevel > 0)
        {
            _currentLevelIndex = overrideLevel;
        }
        
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

        cameraAdjuster.CalculateMenuCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList(), instantCameraApply);
        
        boostersManager.Initialize();
    }

    public void OnLevelComplete()
    {
        LevelTimer.Instance.FinishLevel();
        AnalyticsManager.Instance.SendLevelCompletedEvent(levelSelection.AnalyticsLevelNumber);
        AdjustEventManager.Instance.TrackLevelFinish("lmf6ne", levelSelection.AnalyticsLevelNumber, $"Level {levelSelection.AnalyticsLevelNumber}", "Completed", LevelTimer.Instance.GetLevelTime());
        
        SaveManager.Load();
        SaveManager.Data.extraCellBoostersCount += _currentLevel.ExtraCellBoostersCount;
        SaveManager.Data.unlockBoostersCount +=  _currentLevel.UnlockBoostersCount;
        SaveManager.Save();
    }

    public void OnNextLevel()
    {
        AdjustEventManager.Instance.TrackLevelStart("qjbgpm", levelSelection.BoardLevelNumber, $"Level {levelSelection.BoardLevelNumber}");
        AnalyticsManager.Instance.SendNextLevelEvent(levelSelection.AnalyticsLevelNumber);
    }
    
    public void CameraMenuTransition()
    {
        var parameters = cameraAdjuster.CalculateMenuCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList());
        cameraTransition.MoveTo(parameters.position, parameters.size);
        HideBoosters();
    }

    public void ApplyGameCamera()
    {
        cameraAdjuster.CalculateGameCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList(), true);
    }
    
    private void CameraGameTransition()
    {
        var parameters = cameraAdjuster.CalculateGameCameraParameters(boardView.Cells.Select(x => x.Occupant.transform).ToList());
        cameraTransition.MoveTo(parameters.position, parameters.size);
    }

    private void HideBoosters()
    {
        boostersCanvas.HideBoosters();
    }
}
