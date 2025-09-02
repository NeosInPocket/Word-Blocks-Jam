using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnusedLettersPoolView : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private List<UnusedLetterPoolInfo> poolInfos;
    [SerializeField] private float modelSize;
    protected UnusedLetterPoolObject _model;
    
    public List<LetterPoolData> Cells => _cells;
    private List<LetterPoolData> _cells;

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
    
    public bool TryAdd(LetterUnitBase unit)
    {
        var freeCell = _cells.FirstOrDefault(x => x.Occupant == null);

        if (freeCell != null)
        {
            int cellIndex = _cells.IndexOf(freeCell);
            var target = _model.SpawnPositions[cellIndex];
            
            freeCell.SetOccupant(unit);
            unit.Navigation.SetTarget(target);
            
            return true;
        }
        
        return false;
    }
}
