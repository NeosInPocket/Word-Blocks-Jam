#if UNITY_EDITOR
using System.Collections;
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
    public int SelectedLevelIndex { get; set; }

    public bool IsLoading { get; private set; }
    public bool IsLevelGenerated { get; private set; }
    public bool IsBoardGenerated { get; private set; }
    private int CurrentCellsCount => _boardView.GetCellsCount();
    
    public static LevelEditor Instance { get; private set; }
    private const string _folderPath = "Assets/Resources/LevelAssets";

    public LevelEditor()
    {
        Instance = this;
    }
    
    public void SetBoardView()
    {
        _boardView = Object.FindObjectOfType<BoardEditorView>();
    }

    public void ClearBoard()
    {
        SetBoardView();
        _boardView.ClearBoard();
    }

    public void EmptyBoard()
    {
        _boardView.EmptyBoard();
        IsLevelGenerated = false;
    }

    public int GetCurrentCellsCounts(EditorWindow context)
    {
        context.Repaint();
        return CurrentCellsCount;
    }
    
    public void SaveLevelAsset()
    {
        string assetPath = $"{_folderPath}/Level_{LoadedLevels[SelectedLevelIndex].levelId}.asset";

        LevelAsset asset;

        if (File.Exists(assetPath))
        {
            asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(assetPath);
            if (asset == null)
            {
                Debug.LogError($"Файл существует, но не удалось загрузить как LevelAsset: {assetPath}");
                return;
            }

            asset.words.Clear();
        }
        else
        {
            asset = ScriptableObject.CreateInstance<LevelAsset>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        asset.levelId = LoadedLevels[SelectedLevelIndex].levelId;
        asset.rows = Rows;
        asset.columns = Columns;
        asset.poolCount = PoolCount;

        asset.words = new List<WordWithRange>();
        asset.words.AddRange(SelectLevelWords());
        asset.board = CollectData();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Level {asset.levelId} saved at {assetPath}");
    }

    public List<BoardEntityData> CollectData()
    {
        List<BoardEntityData> result = new();
        var cells = _boardView.Cells;
        
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

    public List<WordWithRange> SelectLevelWords()
    {
        return LoadedLevels[SelectedLevelIndex].words;
    }

    public void GenerateBoard()
    {
        if (IsLoading) return;

        IsBoardGenerated = true;
        ClearBoard();
        _boardView.BuildEmptyBoard(Rows, Columns);
    }

    public void FillBoard(EditorWindow contextWindow)
    {
        IsLevelGenerated = true;
        EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAfterLoadCoroutine(contextWindow));
    }

    public void RequestReload()
    {
        IsLoading = true;
        LoadedLevels.Clear();
        _ = LoadLevelsAsync();
    }

    private async System.Threading.Tasks.Task LoadLevelsAsync()
    {
        var levels = await _downloader.LoadLevelsAsync();

        if (levels != null)
        {
            var rawLevel = levels;

            LoadedLevels = new List<WordLevel>();
            
            foreach (var level in rawLevel)
            {
                List<WordWithRange> words = new();
                
                foreach (var word in level.words)
                {
                    words.Add(new WordWithRange(word, 0 ,0));
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

    private IEnumerator GenerateAfterLoadCoroutine(EditorWindow repaintTarget)
    {
        while (IsLoading) yield return null;

        if (_boardView == null)
        {
            _boardView = Object.FindObjectOfType<BoardEditorView>();
            if (_boardView == null)
            {
                Debug.LogError("BoardView не найден на сцене.");
                yield break;
            }
        }

        var words = SelectLevelWords();

        bool[,] activeCells = new bool[Rows, Columns];
        var cells = _boardView.Cells;
        
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                var cell = cells.FirstOrDefault(x => x.positionData.position.x == i && x.positionData.position.y == j);

                activeCells[i, j] = cell != null;
            }
        }
        
        var levelData = new LevelData(words, Rows, Columns, PoolCount, activeCells);
        _boardView.EmptyBoard();
        _boardView.FillBoard(levelData);

        IsBoardGenerated = true;
        repaintTarget.Repaint();
    }
}
#endif