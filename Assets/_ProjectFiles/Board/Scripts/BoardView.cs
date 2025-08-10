using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardView : BoardSpawner<BoardCellView>
{
    [SerializeField] private EditorLetterDispenserView dispenserPrefab;
    
    public void BuildBoard(LevelAsset levelAsset)
    {
        ClearBoard();
        BuildEmptyBoard(levelAsset.rows,levelAsset.columns);

        for (int i = 0; i < levelAsset.rows; i++)
        {
            for (int j = 0; j < levelAsset.columns; j++)
            {
                var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                var entity = levelAsset.board.FirstOrDefault(x => x.position == new Vector2Int(i, j));

                if (entity == null)
                {
                    _cells.Remove(cell);
                    Destroy(cell.gameObject);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.character))
                    {
                        cell.SetData(entity);
                        cell.Occupant.Avaliable = false;
                    }
                   
                }
            }
        }
        
        UnlockFirstRow();
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

    private void UnlockFirstRow()
    {
        var maxCellsColumn = _cells.Max(x => x.positionData.position.y);
        
        for (int i = 0; i < maxCellsColumn + 1; i++)
        {
            var targetCell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(0, i));
            if (targetCell != null && targetCell.Occupant != null)
                targetCell.Occupant.Avaliable = true;
        }
    }

    private List<BoardCellView> FindNeighborsForCell(BoardCellView targetCell)
    {
        var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var positionMap = _cells.ToDictionary(c => c.positionData.position);

        var neighbors = directions
            .Select(dir => targetCell.positionData.position + dir)
            .Where(positionMap.ContainsKey)
            .Select(pos => positionMap[pos])
            .ToList();

        return neighbors;
    }

    
    public void RefreshCells()
    {
        foreach (var cell in _cells)
        {
            if (cell.Occupant == null || cell.Occupant.Avaliable) continue;

            var cellNeighbors = FindNeighborsForCell(cell);
            bool isAvaliable = cellNeighbors.Any(x => x.Occupant == null);
            cell.Occupant.Avaliable = isAvaliable;
            cell.Occupant.SetVisibility(isAvaliable || cell.Occupant.GetVisibility());
        }
    }
    
    private void UnlockCellAfterCell(BoardCellView cellView)
    {
        var cellPosition = cellView.positionData.position;
        cellPosition.x += 1;

        var maxCellsRow = _cells.Max(x => x.positionData.position.x);
        
        if (cellPosition.x >= maxCellsRow) return;
        var targetCell = _cells.FirstOrDefault(x => x.positionData.position == cellPosition);
        if (targetCell == null) return;
        
        if (targetCell.Occupant == null) return;
        
        targetCell.Occupant.Avaliable = true;
    }
}
