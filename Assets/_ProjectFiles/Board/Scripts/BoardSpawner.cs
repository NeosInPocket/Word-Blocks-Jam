using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BoardSpawner<T> : MonoBehaviour where T : LetterCell
{
    [SerializeField] protected T prefab;
    [SerializeField] protected VisualArea spawnArea;
    [Range(0, 1f)] 
    [SerializeField] protected float spacingPerCellSize;
    [SerializeField] protected Transform ySpawnTransform;
    [SerializeField] protected Transform parentTransform;
    protected List<T> _cells;

    public void BuildEmptyBoard(int rows, int cols, float cellSize = 0)
    {
        int cellsPerRow = cols;
        var boardBounds = spawnArea.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;

        float xBoardSize = maxX - minX;
        float cellSideSize = cellSize == 0 ? xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1)) : cellSize;
        float spacing = spacingPerCellSize * cellSideSize;
        float startX = minX + cellSideSize / 2;
        float startZ = minZ + cellSideSize / 2;
        
        Vector3 spawnPosition = new Vector3(startX, ySpawnTransform.position.y, startZ);
        
        _cells = new List<T>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
                cell.positionData.position = new Vector2Int(i, j);
                
                cell.SideSize = cellSideSize;
                spawnPosition.x += spacing + cellSideSize;
                _cells.Add(cell);
            }
            
            spawnPosition.x = startX;
            spawnPosition.z += spacing + cellSideSize;
        }
    }

    public List<(Vector2Int position, Vector3 worldPosition)> DeleteLastCells(int cellsCount)
    {
        List<(Vector2Int position, Vector3 worldPosition)> result = new();
        
        for (int i = 0; i < cellsCount; i++)
        {
            var lastCell = _cells.Last();
            result.Add((lastCell.positionData.position, lastCell.transform.position));
            _cells.Remove(lastCell);
            
            #if UNITY_EDITOR
            DestroyImmediate(lastCell.gameObject);
            #else
            Destroy(lastCell.gameObject);
            #endif
        }
        
        return result;
    }
    
    public void BuildEmptyBoard(int count)
    {
        var spawnAreaBounds = spawnArea.Collider.bounds;
        var minX = spawnAreaBounds.min.x;
        var maxX = spawnAreaBounds.max.x;
        var minZ = spawnAreaBounds.min.z;
        var maxZ = spawnAreaBounds.max.z;

        var zCenter = (maxZ + minZ) / 2;
        
        float poolLength = maxX - minX;
        
        float cellSize = poolLength / (count + spacingPerCellSize * (count - 1));
        float spacing = spacingPerCellSize * cellSize;

        Vector3 spawnPosition = new Vector3(minX + cellSize / 2, ySpawnTransform.position.y, zCenter);
        _cells = new List<T>();
        
        for (int i = 0; i < count; i++)
        {
            var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
            cell.SideSize = cellSize;
            
            _cells.Add(cell);

            spawnPosition.x += cellSize + spacing;
        }
    }
}