using UnityEngine;

public class LetterCell : MonoBehaviour
{
    public PositionData positionData;
    [SerializeField] protected Transform spawnTransform;
    protected LetterUnitBase _occupant;
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

    public LetterUnitBase Occupant => _occupant;
    
    public void SetOccupant(LetterUnitBase unit)
    {
        _occupant = unit;
        unit.transform.SetParent(transform);
        unit.transform.position = spawnTransform.position;
    }

    public void RemoveOccupant()
    {
        _occupant = null;
    }

    public void DestroyOccupant()
    {
        Destroy(_occupant.gameObject);
        RemoveOccupant();
    }
}
