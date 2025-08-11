#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor editor;

    public LevelEditor Editor => editor ??= new LevelEditor();
    private GUIStyle boldLargeLabel => new (EditorStyles.label)
    {
        fontStyle = FontStyle.Bold,
        fontSize = 20,
        alignment = TextAnchor.MiddleCenter
    };
    
    private GUIStyle boldMiddleLabel => new (EditorStyles.label)
    {
        fontStyle = FontStyle.Bold,
        fontSize = 14,
        alignment = TextAnchor.MiddleCenter
    };
    
    private GUIStyle defaultBoldLabel => new (EditorStyles.label)
    {
        fontStyle = FontStyle.Bold
    };
    
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;
        
        editor = new LevelEditor();
    }

    private ILevelData CurrentLevel => editor.Available ? editor.CurrentLevel : null;

    private List<WordWithRange> CurrentWords => CurrentLevel?.Words;

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;
        if (!Editor.Available) return;
        
        GUILayout.Label("Настройка уровня", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Загрузить уровни из таблицы", GUILayout.Height(30)))
            Editor.RequestReload();

        if (GUILayout.Button("Загрузить существующие уровни", GUILayout.Height(30)))
        {
            Editor.RequestLevelsFromAssets();
            Repaint();
        }

        GUILayout.EndHorizontal();
        
        Editor.Rows = EditorGUILayout.IntField("Кол-во рядов", editor.Rows);
        Editor.Columns = EditorGUILayout.IntField("Кол-во колонок", editor.Columns);
        Editor.PoolCount = EditorGUILayout.IntField("Размер пула", editor.PoolCount);

        GUILayout.Space(10);
        
        if (GUILayout.Button("Добавить трубу"))
        {
            Editor.AddLetterDispenser();
        }

        GUILayout.Space(10);
        
        var defaultContentColor = GUI.contentColor;
        
        for (var i = 0; i < Editor.LetterDispensersDatas.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.contentColor = Editor.EditorDispensersColors[i];
            GUILayout.Label($"Труба {i + 1}:", GUILayout.Width(100));
            GUI.contentColor = defaultContentColor;
            
            Editor.LetterDispensersDatas[i].CellCount = EditorGUILayout.IntField("Ячеек:", Editor.LetterDispensersDatas[i].CellCount);

            if (GUILayout.Button("Удалить", GUILayout.Width(60)))
            {
                Editor.RemoveLetterDispenser(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        
        if (CurrentWords is { Count: > 0 })
        {
            GUILayout.Space(10);
            
            int letterCount = CurrentWords.Sum(w => w.word.Length);
            GUILayout.Label($"Всего букв на уровне: {letterCount}", EditorStyles.boldLabel);

            if (Editor.IsBoardGenerated)
            {
                EditorGUILayout.LabelField($"Текущее кол-во ячеек на уровне: {editor.GetCurrentCellsCounts(this)}", EditorStyles.boldLabel);
            }
        }
        
        GUILayout.Space(10);
        
        if (Editor.IsUsingAssets) GUILayout.Label("Режим редактирования уровня", boldLargeLabel);
        if (Editor.IsUsingDownloadedLevels) GUILayout.Label("Режим создания уровня", boldLargeLabel);
        
        var defaultColor = GUI.contentColor;
        
        if (CurrentWords is { Count: > 0 }) 
        {
            GUILayout.Space(10);
            string[] levelNames = editor.CurrentLevels.ConvertAll(x => $"Уровень {x.LevelId}").ToArray();
            Editor.SelectedLevelIndex = EditorGUILayout.Popup("Выбранный уровень", editor.SelectedLevelIndex, levelNames);

            GUILayout.Space(10);
            
            GUILayout.Space(10);
            GUILayout.Label("Диапазон расположения букв по рядам", boldMiddleLabel);
            GUILayout.Space(10);
            
            foreach (var wordEntry in CurrentWords)
            {
                var index = CurrentWords.IndexOf(wordEntry);
                
                GUI.contentColor = Editor.EditorWordsColors[index];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(wordEntry.word, GUILayout.Width(50));
                GUI.contentColor = defaultColor;
                
                wordEntry.rowMin = EditorGUILayout.IntField(wordEntry.rowMin, GUILayout.Width(50));
                wordEntry.rowMax = EditorGUILayout.IntField(wordEntry.rowMax, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10);

        GUI.enabled = !editor.IsLoading && (editor.LoadedLevels.Count > 0 || editor.LoadedLevelAssets.Count > 0);

        if (Editor.LoadedLevelAssets.Count > 0)
        {
            if (GUILayout.Button("Загрузить уровень", GUILayout.Height(30)))
                editor.BuildLevelFromAsset();
        }
        
        if (GUILayout.Button("Создать поле", GUILayout.Height(30)))
            Editor.GenerateBoard();
        
        if (Editor.IsBoardGenerated)
        {
            if (GUILayout.Button("Заполнить поле юнитами", GUILayout.Height(30)))
                editor.FillBoard(this);

            if (Editor.IsLevelGenerated)
            {
                if (GUILayout.Button("Очистить юнитов", GUILayout.Height(30)))
                    Editor.EmptyBoard();

                if (GUILayout.Button("Сохранить уровень", GUILayout.Height(30)))
                    Editor.SaveLevelAsset();
            }
        }

        GUI.enabled = true;

        if (Editor.IsLoading)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Загрузка данных...", EditorStyles.miniLabel);
        }
    }
}
#endif