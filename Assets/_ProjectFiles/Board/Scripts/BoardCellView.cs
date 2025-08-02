using UnityEngine;

public class BoardCellView : LetterCell
{
    [SerializeField] protected Transform spawnPosition;
    [SerializeField] private LetterUnit letterUnitPrefab;
    private LetterUnitFactory _factory;
    public Transform SpawnPositionTransform => spawnPosition;
    
    private void Start()
    {
        _factory = new LetterUnitFactory(letterUnitPrefab, transform);
    }
    
    public void SetData(BoardEntityData data)
    {
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
        
        _occupant = _factory.Create(data);
        _occupant.transform.position = spawnPosition.position;
    }
}