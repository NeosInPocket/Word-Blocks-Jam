using UnityEngine;

public class LetterCell : MonoBehaviour
{
    public PositionData positionData;
    [SerializeField] private Transform spawnTransform;
    protected LetterUnit _occupant;
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

    public LetterUnit Occupant => _occupant;
    
    public void SetOccupant(LetterUnit unit)
    {
        _occupant = unit;
        unit.transform.SetParent(transform);
        unit.transform.position = spawnTransform.position;
    }

    public void RemoveOccupant()
    {
        Destroy(_occupant.gameObject);
        _occupant = null;
    }
}
