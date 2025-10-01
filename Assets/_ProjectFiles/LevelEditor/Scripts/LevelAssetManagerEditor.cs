#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelAssetManager))]
public class LevelAssetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelAssetManager manager = (LevelAssetManager)target;

        if (GUILayout.Button("Применить ко всем уровням"))
        {
            manager.ModifyAllLevels();
        }
        
        if (GUILayout.Button("Применить бустеры"))
        {
            manager.ApplyBoosters();
        }
    }
}

#endif