using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoxColliderVisualizer))]
public class BoxColliderVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild Object"))
        {
            BoxColliderVisualizer visualizer = (BoxColliderVisualizer)target;
            visualizer.RebuildObject();
        }
    }
}