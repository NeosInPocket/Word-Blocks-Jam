using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class  BoardSpawner<T> : MonoBehaviour where T : LetterCell
{
    [SerializeField] protected T prefab;
    [SerializeField] protected VisualArea spawnArea;
    [Range(0, 1f)] 
    [SerializeField] protected float spacingPerCellSize;
    [SerializeField] protected Transform ySpawnTransform;
    [SerializeField] protected Transform parentTransform;
    [SerializeField] protected float maxCellSize;
    protected List<T> _cells;
    private float _preferredCellSize;
    private float _realCellSize;
    private int _rows;
    private int _cols;
    public float PreferredCellSize => _preferredCellSize;
    public float MaxCellSize => maxCellSize;
    public float RealCellSize => _realCellSize;
    public List<T> Cells => _cells;
    
    public void BuildEmptyBoard(int rows, int cols, float cellSize = 0)
    {
        _rows = rows;
        _cols = cols;
        int cellsPerRow = cols;
        var boardBounds = spawnArea.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;
        var centerX = (boardBounds.min.x + boardBounds.max.x) / 2;

        float xBoardSize = maxX - minX;
        float cellSideSize = cellSize == 0 ? xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1)) : cellSize;
        
        if (cellSideSize > maxCellSize)
        {
            cellSideSize = maxCellSize;
        }
        
        _preferredCellSize = cellSideSize;
        
        float spacing = spacingPerCellSize * cellSideSize;
        float startX = centerX - cols / 2 * cellSideSize - (cols - 1) / 2 * spacing;
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
                _realCellSize = cell.transform.localScale.x;
                
                spawnPosition.x += spacing + cellSideSize;
                _cells.Add(cell);
            }
            
            spawnPosition.x = startX;
            spawnPosition.z += spacing + cellSideSize;
        }
    }
    
    public Vector3 GetPositionFromPositionData(Vector2Int position)
    {
        var boardBounds = spawnArea.Collider.bounds;
        
        var minZ = boardBounds.min.z;
        var centerX = (boardBounds.min.x + boardBounds.max.x) / 2;
        
        float spacing = spacingPerCellSize * _preferredCellSize;
        float startX = centerX - _cols / 2 * _preferredCellSize - (_cols - 1) / 2 * spacing;
        float startZ = minZ + _preferredCellSize / 2;
        
        Vector3 spawnPosition = new Vector3(startX + (spacing + _preferredCellSize) * position.y, ySpawnTransform.position.y, startZ + (spacing + _preferredCellSize) * position.x);
        return spawnPosition;
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
}