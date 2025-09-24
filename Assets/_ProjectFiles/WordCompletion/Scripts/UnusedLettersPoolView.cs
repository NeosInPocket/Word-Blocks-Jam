using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnusedLettersPoolView : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private List<UnusedLetterPoolInfo> poolInfos;
    [SerializeField] private float modelSize;
    [SerializeField] protected BoardView boardView;
    [SerializeField] protected WordCompletionPool wordCompletionPool;
    [SerializeField] protected PlayerSelectionTracker tracker;
    
    protected UnusedLetterPoolObject _model;
    
    public List<LetterPoolData> Cells => _cells;
    private List<LetterPoolData> _cells;
    public bool IsFull => _cells.All(x => x.Occupant != null);

    public void BuildPool(int poolCount)
    {
        var modelPrefab = poolInfos.First(x => x.CellsCount == poolCount).Model;
        _model = Instantiate(modelPrefab, spawnPosition.position, modelPrefab.transform.rotation, transform);
        _model.transform.localScale = Vector3.one * modelSize;

        _cells = new List<LetterPoolData>();

        for (int i = 0; i < poolCount; i++)
        {
            _cells.Add(new LetterPoolData());
        }
    }

    public void DestroyPlatform()
    {
        if (_model != null) Destroy(_model.gameObject);
    }

    public async void AddExtraCell()
    {
        _cells.Add(new LetterPoolData());
        
        tracker.Enable(false);
        await _model.AddExtraCell();
        tracker.Enable(true);
    }
    
    public async UniTask TryAdd(BoardCellView cell)
    {
        var unit = cell.Occupant;
        var freeCell = _cells.FirstOrDefault(x => x.Occupant == null);

        if (freeCell != null)
        {
            cell.RemoveOccupant();
            
            int cellIndex = _cells.IndexOf(freeCell);

            var target = cellIndex == _model.SpawnPositions.Count ? _model.ExtraCellSpawnPosition : _model.SpawnPositions[cellIndex];
            
            var letterUnit = unit.GetComponent<LetterUnit>();
            freeCell.SetOccupant(letterUnit);
            
            var gridPath = GetLetterUnitPathGrid(letterUnit.PositionData.position);
            await unit.Navigation.SetTarget(target.position, false, null, gridPath);
            letterUnit.transform.SetParent(transform);
            letterUnit.DisableNavMesh();ghhghgj
            await wordCompletionPool.CheckLose();
        }
    }

    protected List<BoardCellView> GetLetterUnitPathGrid(Vector2Int unitPosition)
    {
        if (unitPosition.x == 0) 
            return null;

        BoardCellView startCell = boardView.Cells.FirstOrDefault(c => c.positionData.position == unitPosition);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<(BoardCellView cell, List<BoardCellView> path)> queue = new Queue<(BoardCellView, List<BoardCellView>)>();

        visited.Add(startCell.positionData.position);
        queue.Enqueue((startCell, new List<BoardCellView> { startCell }));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            if (current.positionData.position.x == 0)
            {
                path.RemoveAll(x => x.positionData.position == unitPosition);
                return path;
            }
                

            foreach (var neighborPos in GetNeighbors(current.positionData.position))
            {
                if (visited.Contains(neighborPos)) continue;

                BoardCellView neighborCell = boardView.Cells.FirstOrDefault(c => c.positionData.position == neighborPos);
                if (neighborCell == null || neighborCell.Occupant != null) continue;

                visited.Add(neighborPos);
                var newPath = new List<BoardCellView>(path) { neighborCell };
                queue.Enqueue((neighborCell, newPath));
            }
        }

        return null;
    }
    
    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        yield return new Vector2Int(pos.x + 1, pos.y);
        yield return new Vector2Int(pos.x - 1, pos.y);
        yield return new Vector2Int(pos.x, pos.y + 1);
        yield return new Vector2Int(pos.x, pos.y - 1);
    }
}
