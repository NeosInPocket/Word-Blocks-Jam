using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnusedLettersPoolView : MonoBehaviour
{
    [SerializeField] private VisualArea area;
    [SerializeField] private LetterCell prefab;
    [SerializeField] private float spacingPerCellSize;
    [SerializeField] private Transform ySpawnTransform;
    [SerializeField] private Transform parentTransform;
    private List<LetterCell> _cells;
    public List<LetterCell> Cells => _cells;
    
    public void Initialize(int poolCount)
    {
        var spawnAreaBounds = area.Collider.bounds;
        var minX = spawnAreaBounds.min.x;
        var maxX = spawnAreaBounds.max.x;
        var minZ = spawnAreaBounds.min.z;
        var maxZ = spawnAreaBounds.max.z;

        var zCenter = (maxZ + minZ) / 2;
        
        float poolLength = maxX - minX;
        
        float cellSize = poolLength / (poolCount + spacingPerCellSize * (poolCount - 1));
        float spacing = spacingPerCellSize * cellSize;

        Vector3 spawnPosition = new Vector3(minX + cellSize / 2, ySpawnTransform.position.y, zCenter);
        _cells = new List<LetterCell>();
        
        for (int i = 0; i < poolCount; i++)
        {
            var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
            cell.SideSize = cellSize;
            
            _cells.Add(cell);

            spawnPosition.x += cellSize + spacing;
        }
    }

    public bool TryAdd(LetterUnit unit)
    {
        var freeCell = _cells.FirstOrDefault(x => x.Occupant == null);

        if (freeCell != null)
        {
            freeCell.SetOccupant(unit);
            return true;
        }
        else
        {
            return false;
        }
    }
}
