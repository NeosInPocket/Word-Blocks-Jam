#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class LevelSceneEditor : MonoBehaviour
{
    private LetterUnitBase selectedA;
    private LetterUnitBase selectedB;
    private LetterUnitBase hoveredUnit;
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
        {
            SpawnGhostCell();
            DeleteSelected();
            ClearSelection();
            e.Use();
            return;
        }
        
        if (e.type != EventType.MouseDown || e.button != 0 || e.alt || e.shift || !LevelEditingToggleState.State)
            return;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        
        var raycasts = Physics.RaycastAll(ray).ToList();
        var targetRaycast = raycasts.FirstOrDefault(x =>
            x.collider != null && (x.collider.GetComponent<LetterUnitBase>() || x.collider.GetComponent<EditorBoardCellView>()));
        
        if (targetRaycast.collider != null)
        {
            var unit = targetRaycast.collider.GetComponent<LetterUnitBase>();
            EditorBoardCellView freeCellView = null;
            
            if (unit == null)
            {
                var cell = targetRaycast.collider.GetComponent<EditorBoardCellView>();
                if (cell != null)
                    unit = cell.GetComponentInChildren<LetterUnitBase>();
        
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
                    ClearSelection();
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
            var ghostCellRaycast = raycasts.FirstOrDefault(x =>
                x.collider != null && (x.collider.GetComponent<EditorGhostBoardCellView>()));

            if (ghostCellRaycast.collider != null)
            {
                var ghostCell = ghostCellRaycast.collider.GetComponent<EditorGhostBoardCellView>();
                
                var editorBoardCellPrefab = Resources.Load<EditorBoardCellView>("LevelEditor/BoardEditorCell");
                var cell = Instantiate(editorBoardCellPrefab, ghostCell.transform.position, editorBoardCellPrefab.transform.rotation, ghostCell.transform.parent);
                cell.SideSize = ghostCell.SideSize;
                
                DestroyImmediate(ghostCell.gameObject);
            }
            
            ClearSelection();
            return;
        }
        
        e.Use();
    }

    private void SpawnGhostCell()
    {
        if (selectedCell == null) return;
        
        var ghostCellPrefab = Resources.Load<EditorGhostBoardCellView>("LevelEditor/EditorGhostBoardCellView");
        var cell = Instantiate(ghostCellPrefab, selectedCell.transform.position, ghostCellPrefab.transform.rotation, selectedCell.transform.parent);
        cell.Initialize(selectedCell.SideSize, cell.positionData.position);
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

    private void SwapUnits(LetterUnitBase a, LetterUnitBase b)
    {
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;

        Undo.RecordObject(a.transform, "Swap LetterUnitBase A");
        Undo.RecordObject(b.transform, "Swap LetterUnitBase B");

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

    private void MoveUnitToCell(LetterUnitBase unit, EditorBoardCellView cellView)
    {
        unit.transform.SetParent(null);

        var position = cellView.SpawnPositionTransform.position;
        unit.transform.SetParent(cellView.SpawnPositionTransform);
        unit.transform.position = position;
    }
}
#endif
