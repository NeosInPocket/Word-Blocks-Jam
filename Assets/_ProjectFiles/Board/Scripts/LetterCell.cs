using UnityEngine;

public class LetterCell : MonoBehaviour
{
    public PositionData positionData;
    [SerializeField] protected Transform spawnTransform;
    [SerializeField] protected GameObject renderObject;
    protected LetterUnitBase _occupant;
    private Renderer _renderer;
    protected BoardEntityData _data;
    public BoardEntityData Data => _data;
    
    public float SideSize
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = renderObject.GetComponent<Renderer>();   
            }

            return _renderer.bounds.size.x;
        }

        set => transform.localScale *= value / SideSize;
    }

    public LetterUnitBase Occupant => _occupant;
    
    public void SetOccupant(LetterUnitBase unit)
    {
        _data = unit.Data;
        _occupant = unit;
        unit.transform.SetParent(transform);
        unit.transform.position = spawnTransform.position;
    }

    public void RemoveOccupant()
    {
        _occupant = null;
    }
}
