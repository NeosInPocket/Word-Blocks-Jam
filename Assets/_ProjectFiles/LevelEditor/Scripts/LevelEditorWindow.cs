#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor editor;
    public LevelEditor Editor => editor;
    
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        editor = new LevelEditor();
    }

    private void OnGUI()
    {
        GUILayout.Label("Настройка уровня", EditorStyles.boldLabel);

        editor.Rows = EditorGUILayout.IntField("Кол-во рядов", editor.Rows);
        editor.Columns = EditorGUILayout.IntField("Кол-во колонок", editor.Columns);
        editor.PoolCount = EditorGUILayout.IntField("Размер пула", editor.PoolCount);

        GUILayout.Space(10);

        if (GUILayout.Button("Обновить данные таблицы"))
        {
            editor.RequestReload();
        }

        if (editor.LoadedLevels.Count > 0)
        {
            GUILayout.Space(10);

            string[] levelNames = editor.LoadedLevels.ConvertAll(l => $"Уровень {l.levelId}").ToArray();
            editor.SelectedLevelIndex = EditorGUILayout.Popup("Выбранный уровень", editor.SelectedLevelIndex, levelNames);
            
            int letterCount = editor.LoadedLevels[editor.SelectedLevelIndex]
                .words
                .Sum(w => w.word.Length);

            GUILayout.Label($"Всего букв на уровне: {letterCount}");
            
            GUILayout.Space(10);
            GUILayout.Label("Диапазон расположения букв слова по рядам", EditorStyles.boldLabel);

            List<WordWithRange> words = editor.LoadedLevels[editor.SelectedLevelIndex].words;
            
            for (int i = 0; i < words.Count; i++)
            {
                var wordEntry = words[i];

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(wordEntry.word, GUILayout.Width(50));

                wordEntry.rowMin = EditorGUILayout.IntField(wordEntry.rowMin, GUILayout.Width(50));
                wordEntry.rowMax = EditorGUILayout.IntField(wordEntry.rowMax, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10);

        GUI.enabled = !editor.IsLoading && editor.LoadedLevels.Count > 0;

        if (GUILayout.Button("Сгенерировать поле"))
        {
            editor.GenerateBoard();
        }
        
        if (editor.IsBoardGenerated)
        {
            EditorGUILayout.LabelField($"Текущее кол-во ячеек на уровне: {editor.GetCurrentCellsCounts(this)}");
            
            if (GUILayout.Button("Заполнить поле"))
            {
                editor.FillBoard(this);
            }
            
            if (editor.IsLevelGenerated)
            {
                if (GUILayout.Button("Очистить уровень"))
                {
                    editor.EmptyBoard();
                }
                
                if (GUILayout.Button("Сохранить уровень"))
                {
                    editor.SaveLevelAsset();
                }
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