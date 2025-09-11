using UnityEngine;

public class BoardCellView : LetterCell
{
    [SerializeField] private LetterUnitBase letterUnitPrefab;
    [SerializeField] private float unitSizePerCellSize = 0.7f;
    private LetterUnitFactory _factory;
    public Transform SpawnPositionTransform => spawnTransform;
    public bool IsOccupiedByMovingUnit { get; set; }
    
    private void Start()
    {
        _factory = new LetterUnitFactory(letterUnitPrefab, transform);
    }
    
    public void SetData(BoardEntityData data, int priority)
    {
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
        
        _occupant = _factory.Create<LetterUnit>(data, SideSize * unitSizePerCellSize);
        _occupant.transform.position = SpawnPositionTransform.position;
        //_occupant.GetComponent<LetterUnit>().SetAvoidancePriority(priority);
    }
}