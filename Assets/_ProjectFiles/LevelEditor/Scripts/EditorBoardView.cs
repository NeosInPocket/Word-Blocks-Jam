#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class EditorBoardView : BoardSpawner<EditorBoardCellView>
{
    [SerializeField] private EditorGhostBoardCellView ghostPrefab;
    [SerializeField] private EditorLetterDispenserView dispenserPrefab;

    public List<EditorBoardCellView> Cells
    {
        get
        {
            _cells.RemoveAll(obj => obj == null);
            return _cells;
        }
    }

    public List<EditorGhostBoardCellView> GhostCells
    {
        get
        {
            if (_ghostCells == null) 
                _ghostCells = new List<EditorGhostBoardCellView>();
            
            _ghostCells.RemoveAll(obj => obj == null);
            return _ghostCells;
        }
    }
    private List<EditorGhostBoardCellView> _ghostCells;
    
    private void Start()
    {
        _ghostCells = new List<EditorGhostBoardCellView>();
        ClearBoard();
    }

    public void FillBoard(LevelData levelData)
    {
        var boardGenerator = new BoardDataGenerator();
        var data = boardGenerator.Generate(levelData);

        for (int i = 0; i < levelData.Rows; i++)
        {
            for (int j = 0; j < levelData.Columns; j++)
            {
                if (data[i, j] != null)
                {
                    var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                    if (cell != null)
                        cell.SetData(data[i, j]);
                }
            }
        }
    }

    public void InsertDispensers(List<EditorLetterDispenserView> dispensers)
    {
        var deletedCells = DeleteLastCells(dispensers.Count);
        
        for (int i = 0; i < deletedCells.Count; i++)
        {
            dispensers[i].SetPosition(deletedCells[i].position, deletedCells[i].worldPosition);
        }
    }

    public void SetDispensers(LevelAsset levelAsset, List<EditorLetterDispenserView> dispensers)
    {
        List<EditorBoardCellView> occupiedCells = new List<EditorBoardCellView>();
        List<BoardEntityData> usedEntities = new List<BoardEntityData>();
        
        for (int i = 0; i < dispensers.Count; i++)
        {
            var data = levelAsset.Board.First(x => x.isOccupiedByDispenser && !usedEntities.Contains(x));
            var cellToDelete = _cells.FirstOrDefault(x => x.positionData.position == data.position && !occupiedCells.Contains(x));
            if (cellToDelete == null)
            {
                dispensers[i].SetPosition(data.position, GetPositionFromPositionData(data.position));
            }
            else
            {
                occupiedCells.Add(cellToDelete);
                dispensers[i].SetPosition(cellToDelete.positionData.position, cellToDelete.transform.position);
                DestroyImmediate(cellToDelete.gameObject);
            }
            
            usedEntities.Add(data);
        }
    }
    
    public void BuildBoard(LevelAsset levelAsset)
    {
        ClearBoard();
        BuildEmptyBoard(levelAsset.Rows,levelAsset.Columns);
        int dispenserIndex = 0;
        
        for (int i = 0; i < levelAsset.Rows; i++)
        {
            for (int j = 0; j < levelAsset.Columns; j++)
            {
                var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                var entity = levelAsset.Board.FirstOrDefault(x => x.position == new Vector2Int(i, j));

                if (entity == null || entity.isOccupiedByDispenser)
                {
                    _cells.Remove(cell);
                    DestroyImmediate(cell.gameObject);
                    continue;
                }
                
                if (!string.IsNullOrEmpty(entity.character))
                    cell.SetData(entity);
            }
        }
        
        SpawnAdditionalCells(levelAsset.Board);
    }

    private void SpawnAdditionalCells(List<BoardEntityData> board)
    {
        var boardCellsPositions = Cells.Select(x => x.positionData.position).ToList();
        var dataCellsPositions = board.Where(x => !x.isOccupiedByDispenser && !x.isGhostCell).Select(x => x.position).ToList();
        
        List<Vector2Int> newCellsPositions = boardCellsPositions
            .Where(a => !dataCellsPositions.Contains(a))
            .Concat(dataCellsPositions.Where(b => !boardCellsPositions.Contains(b)))
            .ToList();

        foreach (var pos in newCellsPositions)
        {
            var spawnPosition = GetPositionFromPositionData(pos);
            
            var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
            cell.positionData.position = pos;
                
            cell.SideSize = PreferredCellSize;
            _cells.Add(cell);
        }
    }
    
    public int GetCellsCount()
    {
        if (_cells == null) GetCells();
        
        var count = _cells.Count(x => x != null);
        return count;
    }

    private void GetCells()
    {
        _cells = parentTransform.GetComponentsInChildren<EditorBoardCellView>().ToList();
    }
    
    public void EmptyBoard()
    {
        foreach (var cell in _cells)
        {
            if (cell == null) continue;
            var unit = cell.GetComponentInChildren<EditorLetterUnit>();

            if (unit != null)
            {
                DestroyImmediate(unit.gameObject);
            }
        }
    }
    
    public void ClearBoard()
    {
        while (parentTransform.childCount > 0)
        {
            foreach (Transform child in parentTransform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
#endif