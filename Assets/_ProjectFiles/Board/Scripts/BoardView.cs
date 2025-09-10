using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEngine;

public class BoardView : BoardSpawner<BoardCellView>
{
    [SerializeField] private LetterDispenserView dispenserPrefab;
    [SerializeField] private float dispenserUnitAppearDelay;
    private List<LetterDispenserView> _dispensers = new List<LetterDispenserView>();
    private int currentPriority = 0;
    private LevelAsset _levelAsset;
    private bool _agentsEnabled;
    
    public void BuildBoard(LevelAsset levelAsset)
    {
        _agentsEnabled = false;
        _dispensers = new List<LetterDispenserView>();
        _levelAsset = levelAsset;
        ClearBoard();
        BuildEmptyBoard(levelAsset.Rows,levelAsset.Columns);
    }

    public void FillBoard()
    {
        FillBoard(_levelAsset);
        UnlockFirstRow();
    }

    private void FillBoard(LevelAsset levelAsset)
    {
        foreach (var entity in levelAsset.Board)
        {
            var cell = _cells.FirstOrDefault(x => x.positionData.position == entity.position);
            
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

                    try
                    {
                        var dispenserData =
                            levelAsset.Dispensers.FirstOrDefault(x => x.Board[0].position == entity.position);

                        if (dispenserData != null)
                        {
                            dispenser.SetData(dispenserData);
                        }
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                        
                    dispenser.SetPosition(entity.position, cell.transform.position);
                    var dispenserCell = _cells.First(x => x.positionData.position == new Vector2Int(entity.position.x - 1, entity.position.y));
                    dispenserCell.ConnectedDispenser = dispenser;
                    
                    _dispensers.Add(dispenser);

                    _cells.Remove(cell);
                    Destroy(cell.gameObject);
                }
                else 
                    
                if (!string.IsNullOrEmpty(entity.character))
                {
                    cell.SetData(entity, currentPriority);
                    cell.Occupant.Available = false;
                    currentPriority++;
                }
                   
            }
        }

        DeleteFreeCells();
    }

    private void DeleteFreeCells()
    {
        _cells.RemoveAll(x => x.Occupant == null);
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
                targetCell.Occupant.Available = true;
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
        RefreshDispensers();
        
        foreach (var cell in _cells)
        {
            if (cell.Occupant == null || cell.Occupant.Available) continue;
            
            var cellNeighbors = FindNeighborsForCell(cell);
            var availableCells = cellNeighbors.Where(x => x.Occupant == null);
            
            if (availableCells.All(x => x.ConnectedDispenser == null))
            {
                bool isAvaliable = cellNeighbors.Any(x => x.Occupant == null);
                cell.Occupant.Available = isAvaliable;
                cell.Occupant.SetVisibility(isAvaliable || cell.Occupant.GetVisibility());
            }
            else
            {
                var dispenserCells = availableCells.Where(x => x.ConnectedDispenser != null).ToList();
                
                bool anyAvailable = dispenserCells.Any(x => x.ConnectedDispenser.IsEmpty);
                if (anyAvailable)
                {
                    cell.Occupant.Available = true;
                    cell.Occupant.SetVisibility(true);
                }
            }
        }
    }

    public void EnableAgents()
    {
        if (_agentsEnabled) return;

        _agentsEnabled = true;
        _cells.Select(x => x.Occupant.Navigation).ToList().ForEach(x => x.Enabled = true);
    }
    
    private async void RefreshDispensers()
    {
        foreach (var dispenser in _dispensers)
        {
            var nextCellToDispenser = _cells.First(x => x.positionData.position == new Vector2Int(dispenser.Position.x - 1, dispenser.Position.y));

            if (nextCellToDispenser.Occupant == null)
            {
                if (dispenser.TryGetNext(out BoardEntityData data))
                {
                    await Task.Delay((int)(dispenserUnitAppearDelay * 1000));
                    nextCellToDispenser.SetData(data, currentPriority);
                    nextCellToDispenser.Occupant.Available = true;
                    nextCellToDispenser.Occupant.Navigation.Enabled = true;
                    currentPriority++;
                }
            }
        }
    }
}
