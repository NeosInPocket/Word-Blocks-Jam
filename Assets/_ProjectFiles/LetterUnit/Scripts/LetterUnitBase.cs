using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class LetterUnitBase : MonoBehaviour
{
    [SerializeField] private PositionData positionData;
    [SerializeField] protected GameObject rendererObject;
    [SerializeField] protected LetterUnitNavigation navigation;
    private BoardEntityData _data;
    protected bool _available;
    protected string _character;
    protected bool _visible;
    protected Renderer _renderer;
    
    public BoardEntityData Data => _data;
    public PositionData PositionData => positionData;
    public LetterUnitNavigation Navigation => navigation;
    public string Character => _character;
    public abstract bool Available { get; set; }

    public float Size
    {
        get
        {
            if (_renderer == null)
                _renderer = rendererObject.GetComponent<Renderer>(); 

            return _renderer.bounds.size.x;
        }

        set => transform.localScale *= value / Size;
    }
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        positionData.position = data.position;
        _character = data.character;
        
        SetCharacterData(data);
        SetVisibility(data.isVisible);
    }

    protected abstract void SetCharacterData(BoardEntityData data);
    
    public abstract void SetVisibility(bool value);
    public bool GetVisibility() => _visible;

    public void ToggleVisibility()
    {
        SetVisibility(!_visible);
    }
}
