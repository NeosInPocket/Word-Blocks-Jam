using UnityEngine;

public class BoardCell : MonoBehaviour
{
    [SerializeField] private LetterUnit letterUnitPrefab;
    [SerializeField] private Transform spawnPosition;
    private LetterUnitFactory _factory;
    private LetterUnit _occupant;
    private Renderer _renderer;

    public float SideSize
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();   
            }

            return _renderer.bounds.size.x;
        }

        set => transform.localScale *= value / SideSize;
    }
    
    public LetterUnit Occupant
    {
        get => _occupant;
        set => SetOccupant(value);
    }

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

    private void SetOccupant(LetterUnit unit)
    {
        _occupant = unit;
    }
}
