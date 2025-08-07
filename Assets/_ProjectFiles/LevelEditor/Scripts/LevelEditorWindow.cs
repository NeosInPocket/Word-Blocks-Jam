#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor editor;
    public LevelEditor Editor => editor;
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
    
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        editor = new LevelEditor();
    }

    private ILevelData CurrentLevel => editor.Available ? editor.CurrentLevel : null;

    private List<WordWithRange> CurrentWords => CurrentLevel?.Words;

    private void OnGUI()
    {
        editor ??= new LevelEditor();
        if (!editor.Available) return;
        
        GUILayout.Label("Настройка уровня", EditorStyles.boldLabel);

        editor.Rows = EditorGUILayout.IntField("Кол-во рядов", editor.Rows);
        editor.Columns = EditorGUILayout.IntField("Кол-во колонок", editor.Columns);
        editor.PoolCount = EditorGUILayout.IntField("Размер пула", editor.PoolCount);

        if (CurrentWords is { Count: > 0 })
        {
            GUILayout.Space(10);
            
            int letterCount = CurrentWords.Sum(w => w.word.Length);
            GUILayout.Label($"Всего букв на уровне: {letterCount}", EditorStyles.boldLabel);

            if (editor.IsBoardGenerated)
            {
                EditorGUILayout.LabelField($"Текущее кол-во ячеек на уровне: {editor.GetCurrentCellsCounts(this)}", EditorStyles.boldLabel);
            }
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Загрузить уровни из таблицы", GUILayout.Height(30)))
            editor.RequestReload();

        if (GUILayout.Button("Загрузить существующие уровни", GUILayout.Height(30)))
        {
            editor.RequestLevelsFromAssets();
            Repaint();
        }
            

        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        if (editor.IsUsingAssets) GUILayout.Label("Режим редактирования уровня", boldLargeLabel);
        if (editor.IsUsingDownloadedLevels) GUILayout.Label("Режим создания уровня", boldLargeLabel);
        
        if (CurrentWords is { Count: > 0 }) 
        {
            GUILayout.Space(10);
            string[] levelNames = editor.CurrentLevels.ConvertAll(x => $"Уровень {x.LevelId}").ToArray();
            editor.SelectedLevelIndex = EditorGUILayout.Popup("Выбранный уровень", editor.SelectedLevelIndex, levelNames);

            GUILayout.Space(10);
            
            if (GUILayout.Button("Добавить трубу"))
            {
                editor.AddLetterDispenser();
            }

            for (var i = 0; i < editor.LetterDispensers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label($"Труба {i + 1}:", GUILayout.Width(100));

                int newCellCount = EditorGUILayout.IntField("Ячеек:", editor.LetterDispensers[i].CellCount);
                editor.UpdateLetterDispenserCellCount(i, newCellCount);

                if (GUILayout.Button("Удалить", GUILayout.Width(60)))
                {
                    editor.RemoveLetterDispenser(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            
            GUILayout.Space(10);
            GUILayout.Label("Диапазон расположения букв по рядам", boldMiddleLabel);
            GUILayout.Space(10);
            
            foreach (var wordEntry in CurrentWords)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(wordEntry.word, GUILayout.Width(50));
                wordEntry.rowMin = EditorGUILayout.IntField(wordEntry.rowMin, GUILayout.Width(50));
                wordEntry.rowMax = EditorGUILayout.IntField(wordEntry.rowMax, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10);

        GUI.enabled = !editor.IsLoading && (editor.LoadedLevels.Count > 0 || editor.LoadedLevelAssets.Count > 0);

        if (editor.LoadedLevelAssets.Count > 0)
        {
            if (GUILayout.Button("Загрузить уровень", GUILayout.Height(30)))
                editor.BuildLevelFromAsset();
        }
        
        if (GUILayout.Button("Создать поле", GUILayout.Height(30)))
            editor.GenerateBoard();
        
        if (editor.IsBoardGenerated)
        {
            if (GUILayout.Button("Заполнить поле юнитами", GUILayout.Height(30)))
                editor.FillBoard(this);

            if (editor.IsLevelGenerated)
            {
                if (GUILayout.Button("Очистить юнитов", GUILayout.Height(30)))
                    editor.EmptyBoard();

                if (GUILayout.Button("Сохранить уровень", GUILayout.Height(30)))
                    editor.SaveLevelAsset();
            }
        }

        GUI.enabled = true;

        if (editor.IsLoading)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Загрузка данных...", EditorStyles.miniLabel);
        }
    }
}
#endif