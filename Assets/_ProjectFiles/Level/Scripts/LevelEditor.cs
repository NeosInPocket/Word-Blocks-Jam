using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class LevelEditor
{
    public int Rows { get; set; } = 5;
    public int Columns { get; set; } = 5;
    public int PoolCount { get; set; } = 3;

    public List<WordLevel> LoadedLevels { get; private set; } = new();
    public int SelectedLevelIndex { get; set; }

    public bool IsLoading { get; private set; }
    public bool IsGenerated { get; private set; }

    private WordLevelDownloader downloader = new();
    private BoardView boardView;

    public void SetBoardView(BoardView view)
    {
        boardView = view;
    }

    public void ClearBoard()
    {
        boardView?.ClearBoard();
        IsGenerated = false;
    }

    public void SaveLevelAsset()
    {
        string folderPath = "Assets/_ProjectFiles/Level/LevelAssets";
        string assetPath = $"{folderPath}/Level_{LoadedLevels[SelectedLevelIndex].levelId}.asset";

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
        asset.words.AddRange(SelectLevelWords());
        asset.board = CollectData();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Level {asset.levelId} saved at {assetPath}");
    }

    public BoardEntityData[,] CollectData()
    {
        if (boardView == null)
            boardView = UnityEngine.Object.FindObjectOfType<BoardView>();

        var data = new BoardEntityData[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                data[i, j] = boardView.Cells[i, j].Occupant.Data;
            }
        }

        return data;
    }

    public List<string> SelectLevelWords()
    {
        return LoadedLevels[SelectedLevelIndex].words;
    }

    public void Generate(EditorWindow contextWindow)
    {
        if (IsLoading) return;

        IsGenerated = true;
        ClearBoard();

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
        var levels = await downloader.LoadLevelsAsync();

        if (levels != null)
        {
            LoadedLevels = levels;
            SelectedLevelIndex = 0;
            Debug.Log($"Loaded {levels.Count} levels.");
        }
        else
        {
            Debug.LogError("Failed to load levels.");
        }

        IsLoading = false;
    }

    private IEnumerator GenerateAfterLoadCoroutine(EditorWindow repaintTarget)
    {
        while (IsLoading) yield return null;

        if (boardView == null)
        {
            boardView = UnityEngine.Object.FindObjectOfType<BoardView>();
            if (boardView == null)
            {
                Debug.LogError("BoardView not found in scene.");
                yield break;
            }
        }

        var words = SelectLevelWords();
        var levelData = new LevelData(words, Rows, Columns, PoolCount);

        var boardData = new BoardData();
        boardData.GenerateBoard(levelData);
        boardView.BuildBoard(boardData);

        IsGenerated = true;
        repaintTarget.Repaint();
    }
}
