using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardCell prefab;
    [SerializeField] private VisualArea boardBoundsCollider;
    [Range(0, 1f)]
    [SerializeField] private float spacingPerCellSize;
    [SerializeField] private Transform ySpawnTransform;
    [SerializeField] private Transform parentTransform;
    private BoardCell[,] _cells;
    public BoardCell[,] Cells => _cells;
    public GameObject LettersCellsParent => parentTransform.gameObject;

    public void FillBoard(LevelData levelData)
    {
        var boardGenerator = new BoardGenerator();
        var data = boardGenerator.Generate(levelData);

        for (int i = 0; i < levelData.Rows; i++)
        {
            for (int j = 0; j < levelData.Columns; j++)
            {
                if (data[i, j] != null)
                {
                    var cell = _cells[i, j];
                    cell.SetData(data[i, j]);
                }
            }
        }
    }

    public void BuildBoard(LevelAsset levelAsset)
    {
        ClearBoard();
        BuildEmptyBoard(levelAsset.rows,levelAsset.columns);

        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                var cell = _cells[i, j];
                var entity = levelAsset.board.FirstOrDefault(x => x.position == new Vector2Int(i, j));

                if (entity == null)
                {
                    Destroy(cell.gameObject);
                }
                else
                {
                    if (entity.character != String.Empty)
                    {
                        cell.SetData(entity);
                    }
                   
                }
            }
        }
    }
    
    public int GetCellsCount()
    {
        int result = 0;
        
        foreach (var cell in _cells)
        {
            if (cell != null)
            {
                result++;
            }
        }
        
        return result;
    }

    public void EmptyBoard()
    {
        foreach (var cell in _cells)
        {
            if (cell == null) continue;
            var unit = cell.GetComponentInChildren<LetterUnit>();

            if (unit != null)
            {
                DestroyImmediate(unit.gameObject);
            }
        }
    }

    public void BuildEmptyBoard(int rows, int cols)
    {
        int cellsPerRow = cols;
        var boardBounds = boardBoundsCollider.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;

        float xBoardSize = maxX - minX;
        float cellSideSize = xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1));
        float spacing = spacingPerCellSize * cellSideSize;
        float startX = minX + cellSideSize / 2;
        Vector3 spawnPosition = new Vector3(startX, ySpawnTransform.position.y, minZ + cellSideSize / 2);

        _cells = new BoardCell[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
                cell.positionData.position = new Vector2Int(i, j);
                
                cell.SideSize = cellSideSize;
                spawnPosition.x += spacing + cellSideSize;
                _cells[i, j] = cell;
            }
            
            spawnPosition.x = startX;
            spawnPosition.z += spacing + cellSideSize;
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
