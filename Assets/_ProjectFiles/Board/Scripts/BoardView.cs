using System;
using System.Collections.Generic;
using System.Linq;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEngine;

public class BoardView : BoardSpawner<BoardCellView>
{
    [SerializeField] private LetterDispenserView dispenserPrefab;
    private List<LetterDispenserView> _dispensers = new List<LetterDispenserView>();
    
    public void BuildBoard(LevelAsset levelAsset)
    {
        ClearBoard();
        BuildEmptyBoard(levelAsset.Rows,levelAsset.Columns);

        for (int i = 0; i < levelAsset.Rows; i++)
        {
            for (int j = 0; j < levelAsset.Columns; j++)
            {
                var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                var entity = levelAsset.Board.FirstOrDefault(x => x.position == new Vector2Int(i, j));
                if (entity == null)
                {
                    _cells.Remove(cell);
                    Destroy(cell.gameObject);
                    continue;
                }
                
                var isBlockCell = !entity.isOccupiedByDispenser && string.IsNullOrEmpty(entity.character);
                
                if (isBlockCell)
                {
                    _cells.Remove(cell);
                    Destroy(cell.gameObject);
                }
                else
                {
                    if (entity.isOccupiedByDispenser)
                    {
                        var dispenser = Instantiate(dispenserPrefab, cell.transform.position, dispenserPrefab.transform.rotation, cell.transform.parent);
                        var dispenserData = levelAsset.Dispensers.First(x => x.Board[0].position == new Vector2Int(i, j));
                        
                        dispenser.SetData(dispenserData);
                        dispenser.SetPosition(new Vector2Int(i, j), cell.transform.position);
                        _dispensers.Add(dispenser);

                        _cells.Remove(cell);
                        Destroy(cell.gameObject);
                    }
                    else 
                    
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

        foreach (var dispenser in _dispensers)
        {
            var nextCellToDispenser = _cells.First(x => x.positionData.position == new Vector2Int(dispenser.Position.x - 1, dispenser.Position.y));

            if (nextCellToDispenser.Occupant == null)
            {
                if (dispenser.TryGetNext(out BoardEntityData data))
                {
                    nextCellToDispenser.SetData(data);
                    nextCellToDispenser.Occupant.Avaliable = true;
                }
            }
        }
    }
}
