using System;
using System.Collections.Generic;
using System.Linq;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EditorBoardView : BoardSpawner<EditorBoardCellView>
{
    [SerializeField] private EditorGhostBoardCellView ghostPrefab;
    [SerializeField] private EditorLetterDispenserView dispenserPrefab;
    public List<EditorBoardCellView> Cells => _cells;

    private void Start()
    {
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
            if (cellToDelete == null) throw new NullReferenceException("Ячейка не найдена: " + dispensers[i].positionData.position);
            occupiedCells.Add(cellToDelete);
            usedEntities.Add(data);
            
            dispensers[i].SetPosition(cellToDelete.positionData.position, cellToDelete.transform.position);
            DestroyImmediate(cellToDelete.gameObject);
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

                if (entity == null)
                {
                    var ghostCell = Instantiate(ghostPrefab, cell.transform.position, prefab.transform.rotation, cell.transform.parent);
                    ghostCell.Initialize(cell.SideSize, cell.positionData.position);
                    
                    _cells.Remove(cell);
                    DestroyImmediate(cell.gameObject);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.character))
                        cell.SetData(entity);
                }
            }
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