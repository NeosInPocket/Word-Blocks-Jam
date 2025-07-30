using UnityEngine;

[ExecuteInEditMode]
public class BoxColliderVisualizerHandler : MonoBehaviour
{
    [SerializeField] private BoxColliderVisualizer _visualizer;

    private void Awake()
    {
        _visualizer.Initialize();
    }

    private void Update()
    {
        _visualizer.UpdateVisibility();
        _visualizer.UpdateMesh();
        _visualizer.RebuildObject();
    }
}