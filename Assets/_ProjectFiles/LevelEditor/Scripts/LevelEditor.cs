#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class LevelEditor
{
    private WordLevelDownloader _downloader = new();
    private BoardEditorView _boardView;

    public int Rows { get; set; } = 5;
    public int Columns { get; set; } = 5;
    public int PoolCount { get; set; } = 4;

    public List<WordLevel> LoadedLevels { get; private set; } = new();
    public List<LevelAsset> LoadedLevelAssets { get; private set; } = new();
    public int SelectedLevelIndex { get; set; }

    public bool IsLoading { get; private set; }
    public bool IsLevelGenerated { get; private set; }
    public bool IsBoardGenerated { get; private set; }

    private const string _folderPath = "Assets/Resources/LevelAssets";

    public bool IsUsingDownloadedLevels => LoadedLevels.Count > 0;
    public bool IsUsingAssets => LoadedLevelAssets.Count > 0;

    public ILevelData CurrentLevel =>
        IsUsingDownloadedLevels ? new WordLevelWrapper(LoadedLevels[SelectedLevelIndex])
        : IsUsingAssets ? new LevelAssetWrapper(LoadedLevelAssets[SelectedLevelIndex])
        : null;
    
    public List<ILevelData> CurrentLevels
    {
        get
        {
            if (IsUsingDownloadedLevels)
                return LoadedLevels.Select(level => (ILevelData)new WordLevelWrapper(level)).ToList();
            if (IsUsingAssets)
                return LoadedLevelAssets.Select(asset => (ILevelData)new LevelAssetWrapper(asset)).ToList();
            return new List<ILevelData>();
        }
    }

    private int CurrentCellsCount => _boardView?.GetCellsCount() ?? 0;

    public bool Available
    {
        get
        {
            SetBoardView();
            return _boardView != null;
        }
    }

    public LevelEditor()
    {
        SetBoardView();
    }

    private void SetBoardView()
    {
        _boardView = Object.FindObjectOfType<BoardEditorView>();
    }

    private void ClearBoard()
    {
        SetBoardView();
        _boardView?.ClearBoard();
    }

    public void EmptyBoard()
    {
        _boardView?.EmptyBoard();
        IsLevelGenerated = false;
    }

    public int GetCurrentCellsCounts(EditorWindow context)
    {
        if (_boardView == null)
            return 0;

        context?.Repaint();
        return CurrentCellsCount;
    }

    public void SaveLevelAsset()
    {
        if (CurrentLevel == null)
        {
            Debug.LogError("Current level is null, cannot save.");
            return;
        }

        string assetPath = $"{_folderPath}/Level_{CurrentLevel.LevelId}.asset";
        LevelAsset asset;

        if (File.Exists(assetPath))
        {
            asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(assetPath);
            if (asset == null)
            {
                Debug.LogError($"Файл существует, но не удалось загрузить как LevelAsset: {assetPath}");
                return;
            }
        }
        else
        {
            asset = ScriptableObject.CreateInstance<LevelAsset>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        asset.levelId = CurrentLevel.LevelId;
        asset.rows = Rows;
        asset.columns = Columns;
        asset.poolCount = PoolCount;

        asset.words = new List<WordWithRange>(CurrentLevel.Words);

        asset.board = CollectData();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Level {asset.levelId} saved at {assetPath}");
    }

    private List<BoardEntityData> CollectData()
    {
        List<BoardEntityData> result = new();
        var cells = _boardView?.Cells;

        if (cells == null) return result;

        foreach (var cell in cells)
        {
            if (cell == null) continue;

            string character = null;
            bool isVisible = true;

            var letterUnit = cell.GetComponentInChildren<EditorLetterUnit>();
            if (letterUnit != null)
            {
                character = letterUnit.Character;
                isVisible = letterUnit.GetVisibility();
            }

            var data = new BoardEntityData(character, cell.positionData.position, isVisible);
            result.Add(data);
        }

        return result;
    }

    public void GenerateBoard()
    {
        if (IsLoading) return;

        IsBoardGenerated = true;
        ClearBoard();
        _boardView?.BuildEmptyBoard(Rows, Columns);
    }

    public void BuildLevelFromAsset()
    {
        var level = LoadedLevelAssets.ElementAtOrDefault(SelectedLevelIndex);
        if (level == null) return;
        _boardView?.BuildBoard(level);
        IsLevelGenerated = true;
        IsBoardGenerated = true;
    }

    public void FillBoard(EditorWindow contextWindow)
    {
        if (_boardView == null) SetBoardView();
        if (_boardView == null)
        {
            Debug.LogError("BoardView не найден на сцене.");
            return;
        }

        IsLevelGenerated = true;
        EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAfterLoadCoroutine(contextWindow));
    }

    public void RequestReload()
    {
        IsLoading = true;
        LoadedLevels.Clear();
        LoadedLevelAssets.Clear();
        _ = LoadLevelsAsync();
    }

    public void RequestLevelsFromAssets()
    {
        LoadedLevels.Clear();
        LoadedLevelAssets.Clear();
        LoadedLevelAssets = LevelAssetLoader.LoadAllLevels();
    }

    private async System.Threading.Tasks.Task LoadLevelsAsync()
    {
        var levels = await _downloader.LoadLevelsAsync();

        if (levels != null)
        {
            LoadedLevels = new List<WordLevel>();

            foreach (var level in levels)
            {
                var words = level.words.Select(w => new WordWithRange(w, 0, 0)).ToList();
                LevelData.CalculateOptimalRowRanges(words, Rows, Columns);
                LoadedLevels.Add(new WordLevel(level.levelId, words));
            }

            SelectedLevelIndex = 0;
            Debug.Log($"Загружено {levels.Count} уровней.");
        }
        else
        {
            Debug.LogError("Ошибка загрузки уровней.");
        }

        IsLoading = false;
    }

    private System.Collections.IEnumerator GenerateAfterLoadCoroutine(EditorWindow repaintTarget)
    {
        while (IsLoading) yield return null;

        var words = CurrentLevel?.Words;
        if (words == null)
        {
            Debug.LogError("Уровень не выбран или не загружен.");
            yield break;
        }

        bool[,] activeCells = new bool[Rows, Columns];
        var cells = _boardView.Cells;

        for (int i = 0; i < Rows; i++)
        for (int j = 0; j < Columns; j++)
            activeCells[i, j] = cells.Any(x => x.positionData.position.x == i && x.positionData.position.y == j);

        var levelData = new LevelData(words, Rows, Columns, PoolCount, activeCells);
        _boardView.EmptyBoard();
        _boardView.FillBoard(levelData);

        IsBoardGenerated = true;
        repaintTarget?.Repaint();
    }


    public interface ILevelData
    {
        int LevelId { get; }
        List<WordWithRange> Words { get; }
    }

    private readonly struct WordLevelWrapper : ILevelData
    {
        private readonly WordLevel _wordLevel;
        public WordLevelWrapper(WordLevel wordLevel) => _wordLevel = wordLevel;
        public int LevelId => _wordLevel.levelId;
        public List<WordWithRange> Words => _wordLevel.words;
    }

    private readonly struct LevelAssetWrapper : ILevelData
    {
        private readonly LevelAsset _levelAsset;
        public LevelAssetWrapper(LevelAsset levelAsset) => _levelAsset = levelAsset;
        public int LevelId => _levelAsset.levelId;
        public List<WordWithRange> Words => _levelAsset.words;
    }
}
#endif