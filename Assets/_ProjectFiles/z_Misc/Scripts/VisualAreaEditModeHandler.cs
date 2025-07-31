using UnityEngine;

[ExecuteInEditMode]
public class VisualAreaEditModeHandler : MonoBehaviour
{
    [SerializeField] private VisualArea _visualizer;

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