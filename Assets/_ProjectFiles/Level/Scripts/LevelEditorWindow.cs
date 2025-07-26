using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor editor;

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
        GUILayout.Label("Level Configuration", EditorStyles.boldLabel);

        editor.Rows = EditorGUILayout.IntField("Rows", editor.Rows);
        editor.Columns = EditorGUILayout.IntField("Columns", editor.Columns);
        editor.PoolCount = EditorGUILayout.IntField("Unused letters pool count", editor.PoolCount);

        GUILayout.Space(10);

        if (GUILayout.Button("Update Word Table"))
        {
            editor.RequestReload();
        }

        if (editor.LoadedLevels.Count > 0)
        {
            GUILayout.Space(10);

            string[] levelNames = editor.LoadedLevels.ConvertAll(l => $"Level {l.levelId}").ToArray();
            editor.SelectedLevelIndex = EditorGUILayout.Popup("Select Level", editor.SelectedLevelIndex, levelNames);
        }

        GUILayout.Space(10);

        GUI.enabled = !editor.IsLoading && editor.LoadedLevels.Count > 0;

        if (GUILayout.Button("Generate Level"))
        {
            editor.Generate(this);
        }

        if (editor.IsGenerated)
        {
            if (GUILayout.Button("Clear Level"))
            {
                editor.ClearBoard();
            }

            if (GUILayout.Button("Save Level"))
            {
                editor.SaveLevelAsset();
            }
        }

        GUI.enabled = true;

        if (editor.IsLoading)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Loading data...", EditorStyles.miniLabel);
        }
    }
}