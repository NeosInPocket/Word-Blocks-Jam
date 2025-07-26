using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;

public class LevelEditorWindow : EditorWindow
{
    private int rows = 5;
    private int columns = 5;

    private List<WordLevel> loadedLevels = new();
    private int selectedLevelIndex = 0;

    private bool isLoading = false;
    private bool isGenerated = false;
    private WordLevelDownloader downloader;
    private BoardView boardView;
    
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        downloader = new WordLevelDownloader();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Configuration", EditorStyles.boldLabel);

        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);

        GUILayout.Space(10);

        if (GUILayout.Button("Update Word Table"))
        {
            UpdateWordTable();
        }

        if (loadedLevels.Count > 0)
        {
            GUILayout.Space(10);

            string[] levelNames = loadedLevels.ConvertAll(l => $"Level {l.levelId}").ToArray();
            selectedLevelIndex = EditorGUILayout.Popup("Select Level", selectedLevelIndex, levelNames);
        }

        GUILayout.Space(10);

        GUI.enabled = !isLoading && loadedLevels.Count > 0;
        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel();
            isGenerated = true;
        }
        
        if (isGenerated)
        {
            if (GUILayout.Button("Clear Level"))
            {
                ClearLevel();
                isGenerated = false;
            }
        }
        GUI.enabled = true;

        if (isLoading)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Loading data...", EditorStyles.miniLabel);
        }
    }

    private void UpdateWordTable()
    {
        isLoading = true;
        loadedLevels.Clear();
        LoadLevels();
    }

    private async void LoadLevels()
    {
        var levels = await downloader.LoadLevelsAsync();
        
        if (levels != null)
        {
            loadedLevels = levels;
            selectedLevelIndex = 0;
            Debug.Log($"Loaded {levels.Count} levels from table.");
        }
        else
        {
            Debug.LogError("Failed to load levels.");
        }

        isLoading = false;
        Repaint();
    }

    private void GenerateLevel()
    {
        UpdateWordTable();

        EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAfterReloadCoroutine());
    }

    private void ClearLevel()
    {
        boardView.ClearBoard();
    }

    private List<string> SelectLevelWords(int level)
    {
        var words = loadedLevels[selectedLevelIndex];
        return words.words;
    }
    
    private IEnumerator GenerateAfterReloadCoroutine()
    {
        while (isLoading) yield return null;

        if (boardView == null)
        {
            boardView = FindObjectOfType<BoardView>();
            if (boardView == null)
            {
                Debug.LogError("BoardView not found in scene.");
                yield break;
            }
        }
        
        // TODO: сделать проверку на существующую дату, если она есть подгружать её, если нет то создавать новую

        boardView.ClearBoard();
        
        var levelWords = SelectLevelWords(selectedLevelIndex + 1);
        var levelData = new LevelData(levelWords, rows, columns);
        
        var boardData = new BoardData();
        boardData.GenerateBoard(levelData);
        boardView.BuildBoard(boardData);
    }
}
