#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LevelEditingToggleState
{
    public static bool State = true;
    
    static LevelEditingToggleState()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 10, 180, 40), GUI.skin.window);

        GUILayout.BeginHorizontal();
        State = GUILayout.Toggle(State, GUIContent.none, GUILayout.Width(20));
        GUILayout.Label("Редактирование уровня");
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        Handles.EndGUI();
    }
}
#endif