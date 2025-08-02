using System.Collections.Generic;
using UnityEngine;

public abstract class BoardSpawner<T> : MonoBehaviour where T : LetterCell
{
    [SerializeField] private T prefab;
    [SerializeField] private VisualArea spawnArea;
    [Range(0, 1f)] 
    [SerializeField] private float spacingPerCellSize;
    [SerializeField] private Transform ySpawnTransform;
    [SerializeField] private Transform parentTransform;
    protected List<T> _cells;

    public void BuildEmptyBoard(int rows, int cols)
    {
        int cellsPerRow = cols;
        var boardBounds = spawnArea.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;

        float xBoardSize = maxX - minX;
        float cellSideSize = xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1));
        float spacing = spacingPerCellSize * cellSideSize;
        float startX = minX + cellSideSize / 2;
        Vector3 spawnPosition = new Vector3(startX, ySpawnTransform.position.y, minZ + cellSideSize / 2);

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
}