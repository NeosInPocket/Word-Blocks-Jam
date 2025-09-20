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

        if (GUILayout.Button("Собрать все уровни"))
        {
            manager.CollectAllLevels();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Применить ко всем уровням"))
        {
            manager.ModifyAllLevels();
        }
    }
}

#endif