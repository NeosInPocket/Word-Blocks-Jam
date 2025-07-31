using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualArea))]
public class VisualAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild Object"))
        {
            VisualArea visualizer = (VisualArea)target;
            visualizer.RebuildObject();
        }
    }
}