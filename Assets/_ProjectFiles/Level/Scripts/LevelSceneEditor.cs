#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class LevelSceneEditor : MonoBehaviour
{
    private LetterUnit selectedA;
    private LetterUnit selectedB;
    private LetterUnit hoveredUnit;
    private LetterCell selectedCell;

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        HandleMouseInput(Event.current);
        HighlightUnits();
    }

    private void ClearSelection()
    {
        selectedA = null;
        selectedB = null;
        selectedCell = null;
        SceneView.RepaintAll();
    }
    
    private void HandleMouseInput(Event e)
    {
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
            DeleteSelected();
        
        if (e.type != EventType.MouseDown || e.button != 0 || e.alt || e.shift)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        
        var raycasts = Physics.RaycastAll(ray).ToList();
        var targetRaycast = raycasts.FirstOrDefault(x =>
            x.collider != null && (x.collider.GetComponent<LetterUnit>() || x.collider.GetComponent<BoardCellView>()));
        
        if (targetRaycast.collider != null)
        {
            var unit = targetRaycast.collider.GetComponent<LetterUnit>();
            BoardCellView freeCellView = null;
            
            if (unit == null)
            {
                var cell = targetRaycast.collider.GetComponent<BoardCellView>();
                if (cell != null)
                    unit = cell.GetComponentInChildren<LetterUnit>();
        
                if (cell != null && unit == null)
                    freeCellView = cell;
            }
        
            if (unit != null)
            {
                selectedCell = null;
                
                if (selectedA == null)
                    selectedA = unit;
                else if (unit == selectedA)
                {
                    unit.ToggleVisibility();
                    e.Use();
                    return;
                } 
                else if (selectedB == null && unit != selectedA)
                {
                    selectedB = unit;
                    SwapUnits(selectedA, selectedB);
                    ClearSelection();
                }
            }
            else
            {
                if (selectedA != null && freeCellView != null)
                {
                    MoveUnitToCell(selectedA, freeCellView);
                    ClearSelection();
                }
                else if (selectedA == null && freeCellView != null)
                {
                    selectedCell = freeCellView;
                }
                else
                {
                    ClearSelection();
                }
            }
        }
        else
        {
            ClearSelection();
            return;
        }
        
        e.Use();
    }
    
    private void DeleteSelected()
    {
        if (selectedCell == null) return;
        
        Selection.activeObject = null;
        DestroyImmediate(selectedCell.gameObject);
    }
    
    private void HighlightUnits()
    {
        if (selectedA != null)
            DrawHighlight(selectedA.transform.position, Color.green);

        if (selectedB != null)
            DrawHighlight(selectedB.transform.position, Color.cyan);
        
        if (selectedCell != null)
            DrawHighlight(selectedCell.transform.position, Color.red);
    }

    private void DrawHighlight(Vector3 position, Color color)
    {
        Handles.color = color;
        Handles.DrawWireCube(position, Vector3.one * 0.95f);
    }

    private void SwapUnits(LetterUnit a, LetterUnit b)
    {
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;

        Undo.RecordObject(a.transform, "Swap LetterUnit A");
        Undo.RecordObject(b.transform, "Swap LetterUnit B");

        a.transform.SetParent(null);
        b.transform.SetParent(null);

        a.transform.SetParent(parentB);
        b.transform.SetParent(parentA);

        (a.transform.position, b.transform.position) = (b.transform.position, a.transform.position);
        (a.PositionData.position, b.PositionData.position) = (b.PositionData.position, a.PositionData.position);
        
        EditorUtility.SetDirty(a);
        EditorUtility.SetDirty(b);
        EditorSceneManager.MarkSceneDirty(a.gameObject.scene);
    }

    private void MoveUnitToCell(LetterUnit unit, BoardCellView cellView)
    {
        unit.transform.SetParent(null);

        var position = cellView.SpawnPositionTransform.position;
        unit.transform.SetParent(cellView.SpawnPositionTransform);
        unit.transform.position = position;
    }
}
#endif
