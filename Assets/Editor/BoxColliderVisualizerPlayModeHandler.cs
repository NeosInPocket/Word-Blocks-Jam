using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class BoxColliderVisualizerPlayModeHandler
    {
        static BoxColliderVisualizerPlayModeHandler()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                foreach (var visualizer in Object.FindObjectsOfType<BoxColliderVisualizer>())
                {
                    if (visualizer != null)
                    {
                        visualizer.RebuildObject();
                    }
                }
            }
        }
    
        private static void OnAfterAssemblyReload()
        {
            foreach (var visualizer in Object.FindObjectsOfType<BoxColliderVisualizer>())
            {
                if (visualizer != null)
                {
                    visualizer.RebuildObject();
                }
            }
        }
    }
}