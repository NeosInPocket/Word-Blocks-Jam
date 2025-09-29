using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardCellView : LetterCell
{
    [SerializeField] private LetterUnitBase letterUnitPrefab;
    [SerializeField] private float unitSizePerCellSize = 0.7f;
    [SerializeField] private int waitForCellFree;
    private LetterUnitFactory _factory;
    public Transform SpawnPositionTransform => spawnTransform;

    private bool _isOccupiedByMovingUnit;
    
    public bool IsOccupiedByMovingUnit
    {
        get => _isOccupiedByMovingUnit;
        set
        {
            _isOccupiedByMovingUnit = value;
            if (value)
            {
                FreeCellDelay();
            }
        }
    }
    
    private void Start()
    {
        _factory = new LetterUnitFactory(letterUnitPrefab, transform);
    }

    private async UniTask FreeCellDelay()
    {
        await UniTask.Delay(waitForCellFree);
        IsOccupiedByMovingUnit = false;
    }
    
    public void SetData(BoardEntityData data)
    {
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
        
        _occupant = _factory.Create<LetterUnit>(data, SideSize * unitSizePerCellSize);
        _occupant.transform.position = SpawnPositionTransform.position;
    }
}