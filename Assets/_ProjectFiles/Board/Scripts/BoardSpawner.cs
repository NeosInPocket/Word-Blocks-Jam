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
    protected List<T> _cells;
    private float _preferredCellSize;
    private float _realCellSize;
    private float _preferredSpacing;
    public float PreferredCellSize => _preferredCellSize;
    public float RealCellSize => _realCellSize;
    public float PreferredSpacing => _preferredSpacing;
    
    public void BuildEmptyBoard(int rows, int cols, float cellSize = 0)
    {
        int cellsPerRow = cols;
        var boardBounds = spawnArea.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;

        float xBoardSize = maxX - minX;
        float cellSideSize = cellSize == 0 ? xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1)) : cellSize;
        _preferredCellSize = cellSideSize;
        
        float spacing = spacingPerCellSize * cellSideSize;
        _preferredSpacing = spacing;
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
        
        var minX = boardBounds.min.x;
        var minZ = boardBounds.min.z;
        
        float startX = minX + _preferredCellSize / 2;
        float startZ = minZ + _preferredCellSize / 2;
        float spacing = spacingPerCellSize * _preferredCellSize;
        
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