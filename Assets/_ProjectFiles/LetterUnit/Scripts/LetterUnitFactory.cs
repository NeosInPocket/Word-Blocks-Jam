using UnityEngine;

public class LetterUnitFactory
{
    private LetterUnitBase _prefab;
    private Transform _parent;
    
    
    public LetterUnitFactory(LetterUnitBase prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
    
    public T Create<T>(BoardEntityData data, float size) where T : LetterUnitBase
    {
        var unit = Object.Instantiate(_prefab, _parent);
        unit.SetData(data);
        unit.Size = size;
        return unit.GetComponent<T>();
    }
}
