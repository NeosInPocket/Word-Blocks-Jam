#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class LevelSceneEditor : MonoBehaviour
{
    private LetterUnitBase selectedA;
    private LetterUnitBase selectedB;
    private LetterUnitBase hoveredUnit;
    private EditorBoardCellView selectedCell;
    private EditorLetterDispenserView selectedDispenser;
    private EditorBoardView editorBoard;
    public static LevelSceneEditor Instance => Object.FindAnyObjectByType<LevelSceneEditor>();

    private EditorBoardView EditorBoard
    {
        get
        {
            if (editorBoard == null)
                editorBoard = FindObjectOfType<EditorBoardView>();

            return editorBoard;
        }
    }

    private enum HitKind
    {
        None = 0,
        Unit = 1,
        Pipe = 2,
        Cell = 3,
        Ghost = 4
    }

    private struct PickHit
    {
        public HitKind Kind;
        public float Distance;
        public LetterUnitBase Unit;
        public EditorBoardCellView Cell;
        public EditorGhostBoardCellView Ghost;
        public EditorPipeView Pipe;
        public RaycastHit Raw;

        public static readonly PickHit None = new PickHit { Kind = HitKind.None, Distance = float.PositiveInfinity };
    }

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
        var e = Event.current;
        HandleKeyboard(e);
        HandleMouse(e);
        HighlightSelections();
    }

    private void HandleKeyboard(Event e)
    {
        if (e.type != EventType.KeyDown || e.keyCode != KeyCode.Delete)
            return;

        SpawnGhostCell();
        DeleteSelectedCell();
        ClearSelection();
        e.Use();
    }

    private void HandleMouse(Event e)
    {
        if (!IsPrimaryClick(e))
            return;

        var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        var pick = PickTarget(ray);

        ProcessPick(pick, e);
    }

    private static bool IsPrimaryClick(Event e)
    {
        if (e.type != EventType.MouseDown) return false;
        if (e.button != 0) return false;
        if (e.alt || e.shift) return false;
        if (!LevelEditingToggleState.State) return false;
        return true;
    }

    private PickTargetPriority PriorityOf(HitKind kind)
    {
        switch (kind)
        {
            case HitKind.Unit: return PickTargetPriority.Unit;
            case HitKind.Pipe: return PickTargetPriority.Dispenser;
            case HitKind.Cell: return PickTargetPriority.Cell;
            case HitKind.Ghost: return PickTargetPriority.Ghost;
            default: return PickTargetPriority.Other;
        }
    }

    private enum PickTargetPriority
    {
        Unit = 0,
        Dispenser = 1,
        Cell = 2,
        Ghost = 3,
        Other = 100
    }

    private PickHit PickTarget(Ray ray)
    {
        var hits = Physics.RaycastAll(ray);
        if (hits == null || hits.Length == 0)
            return PickHit.None;

        var candidates = new List<PickHit>(hits.Length);
        foreach (var h in hits)
        {
            var pick = ClassifyHit(h);
            if (pick.Kind != HitKind.None)
                candidates.Add(pick);
        }

        if (candidates.Count == 0)
            return PickHit.None;

        var best = candidates
            .OrderBy(p => PriorityOf(p.Kind))
            .ThenBy(p => p.Distance)
            .ThenBy(p => p.Raw.collider != null ? p.Raw.collider.GetInstanceID() : int.MaxValue)
            .First();

        return best;
    }

    private PickHit ClassifyHit(RaycastHit hit)
    {
        if (hit.collider == null)
            return PickHit.None;

        if (hit.collider.TryGetComponent<LetterUnitBase>(out var unit) && unit != null)
        {
            return new PickHit
            {
                Kind = HitKind.Unit,
                Unit = unit,
                Distance = hit.distance,
                Raw = hit
            };
        }

        var pipe = hit.collider.GetComponentInParent<EditorPipeView>();
        if (pipe != null)
        {
            return new PickHit
            {
                Kind = HitKind.Pipe,
                Pipe = pipe,
                Distance = hit.distance,
                Raw = hit
            };
        }

        if (hit.collider.TryGetComponent<EditorBoardCellView>(out var cell) && cell != null)
        {
            return new PickHit
            {
                Kind = HitKind.Cell,
                Cell = cell,
                Distance = hit.distance,
                Raw = hit
            };
        }

        if (hit.collider.TryGetComponent<EditorGhostBoardCellView>(out var ghost) && ghost != null)
        {
            return new PickHit
            {
                Kind = HitKind.Ghost,
                Ghost = ghost,
                Distance = hit.distance,
                Raw = hit
            };
        }

        return PickHit.None;
    }

    private void ProcessPick(PickHit pick, Event e)
    {
        switch (pick.Kind)
        {
            case HitKind.Unit:
                OnUnitClicked(pick.Unit, e);
                break;

            case HitKind.Pipe:
                OnPipeClicked(pick.Pipe);
                break;

            case HitKind.Cell:
                OnCellClicked(pick.Cell);
                break;

            case HitKind.Ghost:
                OnGhostCellClicked(pick.Ghost);
                break;

            default:
                ClearSelection();
                break;
        }

        e.Use();
    }

    private void OnUnitClicked(LetterUnitBase unit, Event e)
    {
        selectedCell = null;

        if (selectedDispenser != null)
        {
            SwapCellWithLetterUnitWithDispenser(unit, selectedDispenser);
            ClearSelection();
            return;
        }
        
        if (selectedA == null)
        {
            selectedA = unit;
            SceneView.RepaintAll();
            return;
        }

        if (unit == selectedA)
        {
            unit.ToggleVisibility();
            ClearSelection();
            return;
        }

        if (selectedB == null && unit != selectedA)
        {
            selectedB = unit;
            SwapUnits(selectedA, selectedB);
            ClearSelection();
            return;
        }
    }

    private void OnPipeClicked(EditorPipeView pipe)
    {
        if (selectedCell != null)
        {
            SwapDispenserWithCell(pipe.View, selectedCell);
            ClearSelection();
            return;
        }

        if (selectedA != null)
        {
            SwapCellWithLetterUnitWithDispenser(selectedA, pipe.View);
            ClearSelection();
            
            SceneView.RepaintAll();
            return;
        }

        if (selectedDispenser == pipe.View)
        {
            ClearSelection();
            return;
        }

        selectedDispenser = pipe.View;
        selectedCell = null;
        SceneView.RepaintAll();
    }

    private void OnCellClicked(EditorBoardCellView cell)
    {
        if (cell.TryGetComponentInChildren<LetterUnitBase>(out var unitInCell))
        {
            OnUnitClicked(unitInCell, Event.current);
            return;
        }

        if (selectedCell == null && selectedDispenser != null)
        {
            SwapDispenserWithCell(selectedDispenser, cell);
            ClearSelection();
            return;
        }
        
        if (selectedA != null)
        {
            MoveUnitToCell(selectedA, cell);
            ClearSelection();
        }
        else
        {
            selectedCell = cell;
            SceneView.RepaintAll();
        }
    }

    private void OnGhostCellClicked(EditorGhostBoardCellView ghost)
    {
        SpawnCell(ghost);
        ClearSelection();
    }

    private void ClearSelection()
    {
        selectedA = null;
        selectedB = null;
        selectedCell = null;
        selectedDispenser = null;
        SceneView.RepaintAll();
    }
    
    private void SpawnCell(EditorGhostBoardCellView ghostCell)
    {
        var editorBoardCellPrefab = Resources.Load<EditorBoardCellView>("LevelEditor/EditorBoardCellView");
        var cell = Instantiate(editorBoardCellPrefab, ghostCell.transform.position, editorBoardCellPrefab.transform.rotation, ghostCell.transform.parent);
        cell.positionData.position = ghostCell.positionData.position;
        cell.SideSize = ghostCell.SideSize;
        EditorBoard.Cells.Add(cell);
        EditorBoard.GhostCells.Remove(ghostCell);
        
        DestroyImmediate(ghostCell.gameObject);
        RefreshCells();
    }
    
    private void SpawnGhostCell()
    {
        if (selectedCell == null) return;

        var ghostCellPrefab = Resources.Load<EditorGhostBoardCellView>("LevelEditor/EditorGhostBoardCellView");
        var cell = Instantiate(ghostCellPrefab, selectedCell.transform.position, ghostCellPrefab.transform.rotation, selectedCell.transform.parent);
        cell.Initialize(selectedCell.SideSize, selectedCell.positionData.position);
        EditorBoard.GhostCells.Add(cell);
    }

    public void RefreshCells()
    {
        var ghostCellPrefab = Resources.Load<EditorGhostBoardCellView>("LevelEditor/EditorGhostBoardCellView");
        var deltas = new List<Vector2Int>
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1,  0),
            new Vector2Int(-1,  1),
            new Vector2Int( 0, -1),
            new Vector2Int( 0,  1),
            new Vector2Int( 1, -1),
            new Vector2Int( 1,  0),
            new Vector2Int( 1,  1),
        };

        var dispensers = FindObjectsByType<EditorLetterDispenserView>(FindObjectsSortMode.None).ToList();
        var dispensersPositions = dispensers.Select(x => x.positionData.position).ToList();
        
        foreach (var cell in EditorBoard.Cells)
        {
            var neighborsPositions = EditorBoard.Cells.Where(p =>
                p != cell &&
                deltas.Contains(p.positionData.position - cell.positionData.position)
            ).Select(x => x.positionData.position).ToList();

            foreach (var delta in deltas)
            {
                var currentPosition = cell.positionData.position + delta;
                
                if (dispensersPositions.Contains(currentPosition)) continue;
                
                if (currentPosition.x < 0 || 
                    neighborsPositions.Contains(currentPosition) || 
                    EditorBoard.GhostCells.Any(x => x.positionData.position == currentPosition
                    )) continue;
                
                var cellPosition = EditorBoard.GetPositionFromPositionData(currentPosition);
                var ghostCell = Instantiate(ghostCellPrefab, cellPosition, ghostCellPrefab.transform.rotation, cell.transform.parent);
                
                ghostCell.Initialize(cell.SideSize, currentPosition);
                EditorBoard.GhostCells.Add(ghostCell);
            }
        }
    }
    
    private void DeleteSelectedCell()
    {
        if (selectedCell == null) return;

        Selection.activeObject = null;
        EditorBoard.Cells.Remove(selectedCell);

        DestroyImmediate(selectedCell.gameObject);
    }

    private void SwapUnits(LetterUnitBase a, LetterUnitBase b)
    {
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;

        Undo.RecordObject(a.transform, "Swap LetterUnitBase A");
        Undo.RecordObject(b.transform, "Swap LetterUnitBase B");

        LetterCell cellA = a.GetComponentInParent<LetterCell>();
        LetterCell cellB = b.GetComponentInParent<LetterCell>();
        
        cellA.SetOccupant(b);
        cellB.SetOccupant(a);

        EditorUtility.SetDirty(a);
        EditorUtility.SetDirty(b);
        EditorSceneManager.MarkSceneDirty(a.gameObject.scene);
    }

    private void MoveUnitToCell(LetterUnitBase unit, EditorBoardCellView cellView)
    {
        Undo.RecordObject(unit.transform, "Move LetterUnitBase To Cell");

        unit.transform.SetParent(null);

        var position = cellView.SpawnPositionTransform.position;
        unit.transform.SetParent(cellView.SpawnPositionTransform);
        unit.transform.position = position;
        cellView.SetOccupant(unit);
        
        EditorUtility.SetDirty(unit);
        EditorSceneManager.MarkSceneDirty(unit.gameObject.scene);
    }

    private void SwapCellWithLetterUnitWithDispenser(LetterUnitBase unit, EditorLetterDispenserView dispenser)
    {
        var letterCell = unit.transform.parent.GetComponent<LetterCell>();
        
        var oldDispGrid = dispenser.positionData.position;
        var oldUnitGrid = unit.PositionData.position;

        var oldDispWorld = dispenser.Pipe.transform.position;
        var oldCellWorld = letterCell.transform.position;
        
        dispenser.SetPosition(oldUnitGrid, oldCellWorld);
        letterCell.positionData.position = oldDispGrid;
        letterCell.transform.position = oldDispWorld;
        unit.PositionData.position = oldDispGrid;
        
        EditorUtility.SetDirty(dispenser);
        EditorUtility.SetDirty(letterCell);
        EditorSceneManager.MarkSceneDirty(dispenser.gameObject.scene);
    }
    
    private void SwapDispenserWithCell(EditorLetterDispenserView dispenser, EditorBoardCellView cell)
    {
        if (cell.TryGetComponentInChildren<LetterUnitBase>(out _))
            return;

        var oldDispGrid = dispenser.positionData.position;
        var oldCellGrid = cell.positionData.position;

        var oldDispWorld = dispenser.Pipe.transform.position;
        var oldCellWorld = cell.transform.position;
        
        Undo.RegisterFullObjectHierarchyUndo(dispenser.gameObject, "Swap Dispenser With Cell");
        Undo.RecordObject(cell.transform, "Swap Dispenser With Cell");
        Undo.RecordObject(cell, "Swap Dispenser With Cell");

        dispenser.SetPosition(oldCellGrid, oldCellWorld);
        cell.positionData.position = oldDispGrid;
        cell.transform.position = oldDispWorld;

        EditorUtility.SetDirty(dispenser);
        EditorUtility.SetDirty(cell);
        EditorSceneManager.MarkSceneDirty(dispenser.gameObject.scene);
    }

    private void HighlightSelections()
    {
        if (selectedA != null)
            DrawHighlight(selectedA.transform.position, Color.green);

        if (selectedB != null)
            DrawHighlight(selectedB.transform.position, Color.cyan);

        if (selectedCell != null)
            DrawHighlight(selectedCell.transform.position, Color.red);

        if (selectedDispenser != null)
            DrawHighlight(selectedDispenser.Pipe.transform.position, Color.yellow);
    }

    private void DrawHighlight(Vector3 position, Color color)
    {
        Handles.color = color;
        Handles.DrawWireCube(position, Vector3.one * 0.95f);
    }
}

internal static class ComponentExtensions
{
    public static bool TryGetComponentInChildren<T>(this Component component, out T result) where T : Component
    {
        result = component.GetComponentInChildren<T>();
        return result != null;
    }
}
#endif
