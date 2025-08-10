#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelEditor
{
    private WordLevelDownloader _downloader = new();
    private EditorBoardView editorBoardView;
    private EditorLetterDispenserFactory editorDispenserFactory;

    public int Rows { get; set; } = 5;
    public int Columns { get; set; } = 5;
    public int PoolCount { get; set; } = 4;

    public List<WordLevel> LoadedLevels { get; private set; } = new();
    public List<LevelAsset> LoadedLevelAssets { get; private set; } = new();

    public List<LetterDispenserData> LetterDispensersDatas { get; set; } = new();
    public List<EditorLetterDispenserView> LetterDispenserViews { get; set; } = new();
    public List<Color> EditorWordsColors { get; set; }
    public List<Color> EditorDispensersColors { get; set; }

    public int SelectedLevelIndex
    {
        get => _selectedLevelIndex;
        set
        {
            _selectedLevelIndex = value;
            EditorWordsColors = GenerateUniqueColors(CurrentLevel.Words.Count);
        }
    }
    private int _selectedLevelIndex;
    
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

    private int CurrentCellsCount => editorBoardView?.GetCellsCount() ?? 0;

    public bool Available
    {
        get
        {
            SetBoardView();
            return editorBoardView != null;
        }
    }

    public LevelEditor()
    {
        SetBoardView();
        editorBoardView.ClearBoard();
        editorDispenserFactory.Clear();
        EditorDispensersColors = GenerateUniqueColors(10);
    }

    private void SetBoardView()
    {
        editorBoardView = Object.FindObjectOfType<EditorBoardView>();
        editorDispenserFactory = Object.FindObjectOfType<EditorLetterDispenserFactory>();
    }

    private void ClearBoard()
    {
        SetBoardView();
        editorBoardView?.ClearBoard();
        editorDispenserFactory.Clear();
        LetterDispenserViews.Clear();
    }

    public void EmptyBoard()
    {
        editorBoardView?.EmptyBoard();
        LetterDispenserViews.ForEach(x => x.Clear());
        IsLevelGenerated = false;
    }

    public int GetCurrentCellsCounts(EditorWindow context)
    {
        if (editorBoardView == null)
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
        var cells = editorBoardView?.Cells;

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

            var data = new BoardEntityData(character, cell.positionData.position, isVisible, letterUnit.Color, false);
            result.Add(data);
        }

        return result;
    }
    
    public void BuildDispensers()
    {
        var dispenserFactory = Object.FindObjectOfType<EditorLetterDispenserFactory>();
        if (dispenserFactory == null) throw new ArgumentNullException("Фабрика труб не найдена");

        dispenserFactory.Clear();
        
        for (int i = 0; i < LetterDispensersDatas.Count; i++)
        {
            var data = LetterDispensersDatas[i];
            
            var dispenserView = dispenserFactory.Create();
            dispenserView.BuildEmptyBoard(data.CellCount, 1, editorBoardView.Cells[0].SideSize);
            dispenserView.SetColor(EditorDispensersColors[i]);
            
            LetterDispenserViews.Add(dispenserView);
        }
        
        editorBoardView.SetDispensers(LetterDispenserViews);
    }
    
    public void GenerateBoard()
    {
        if (IsLoading) return;

        IsBoardGenerated = true;
        ClearBoard();
        editorBoardView?.BuildEmptyBoard(Rows, Columns);
        BuildDispensers();
    }

    public void BuildLevelFromAsset()
    {
        var level = LoadedLevelAssets.ElementAtOrDefault(SelectedLevelIndex);
        if (level == null) return;
        
        Rows = level.rows;
        Columns = level.columns;
        PoolCount = level.poolCount;
        
        editorBoardView?.BuildBoard(level);
        
        IsLevelGenerated = true;
        IsBoardGenerated = true;
    }

    public void FillBoard(EditorWindow contextWindow)
    {
        if (editorBoardView == null)
        {
            Debug.LogError("BoardView не найден на сцене.");
            return;
        }

        IsLevelGenerated = true;
        FillBoardWithLevelData(contextWindow);
    }

    public void RequestReload()
    {
        IsLoading = true;
        editorBoardView?.ClearBoard();
        LoadedLevels.Clear();
        LoadedLevelAssets.Clear();
        _ = LoadLevelsAsync();
    }

    public void RequestLevelsFromAssets()
    {
        editorBoardView?.ClearBoard();
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
            EditorWordsColors = GenerateUniqueColors(levels.Count);
            
            foreach (var level in levels)
            {
                List<WordWithRange> words = new();

                for (int i = 0; i < level.words.Count; i++)
                {
                    var currentWord = level.words[i];
                    words.Add(new WordWithRange(currentWord, 0, 0, EditorWordsColors[i]));
                }
                
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

    private void FillBoardWithLevelData(EditorWindow repaintTarget)
    {
        var words = CurrentLevel?.Words;
        if (words == null)
        {
            Debug.LogError("Уровень не выбран или не загружен.");
        }

        bool[,] activeCells = new bool[Rows, Columns];
        var cells = editorBoardView.Cells;

        for (int i = 0; i < Rows; i++)
        for (int j = 0; j < Columns; j++)
            activeCells[i, j] = cells.Any(x => x.positionData.position.x == i && x.positionData.position.y == j);

        var levelData = new LevelData(words, Rows, Columns, PoolCount, activeCells, LetterDispensersDatas);
        editorBoardView.EmptyBoard();
        editorBoardView.FillBoard(levelData);

        for (int i = 0; i < LetterDispensersDatas.Count; i++)
        {
            var data = LetterDispensersDatas[i];
            var dispenser = LetterDispenserViews[i];
            
            dispenser.FillDispenser(data);
        }
        
        IsBoardGenerated = true;
        repaintTarget?.Repaint();
    }

    public void AddLetterDispenser()
    {
        LetterDispensersDatas.Add(new LetterDispenserData { CellCount = 0 });
    }

    public void RemoveLetterDispenser(int index)
    {
        if (index >= 0 && index < LetterDispensersDatas.Count)
        {
            LetterDispensersDatas.RemoveAt(index);
        }
    }

    public void UpdateLetterDispenserCellCount(int index, int cellCount)
    {
        if (index >= 0 && index < LetterDispensersDatas.Count)
        {
            LetterDispensersDatas[index].CellCount = cellCount;
        }
    }
    
    public List<Color> GenerateUniqueColors(int colorsCount)
    {
        List<Color> colors = new List<Color>(colorsCount);

        for (int i = 0; i < colorsCount; i++)
        {
            float hue = (i / (float)colorsCount) % 1f;
            Color color = Color.HSVToRGB(hue, 0.8f, 0.9f);
            colors.Add(color);
        }

        return colors;
    }
}
#endif