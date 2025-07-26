using UnityEngine;

public class LetterUnitFactory
{
    private LetterUnit _prefab;
    private Transform _parent;
    
    
    public LetterUnitFactory(LetterUnit prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
    
    public LetterUnit Create(BoardEntityData data)
    {
        var unit = Object.Instantiate(_prefab, _parent);
        unit.SetData(data);
        return unit;
    }
}
