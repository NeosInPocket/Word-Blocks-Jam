using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class LetterUnitBase : MonoBehaviour
{
    [SerializeField] private PositionData positionData;
    [SerializeField] protected TMP_Text characterText;
    [SerializeField] protected Image characterFrame;
    [SerializeField] protected GameObject rendererObject;
    private BoardEntityData _data;
    private bool _avaliable;
    protected bool _visible;
    private Renderer _renderer;
    
    public BoardEntityData Data => _data;
    public PositionData PositionData => positionData;
    public string Character => characterText.text;
    public bool Avaliable
    {
        get => _avaliable;
        set
        {
            _avaliable = value;
            characterFrame.color = value ? Color.white : Color.gray;
        }
    }
    
    public float Size
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = rendererObject.GetComponent<Renderer>();   
            }

            return _renderer.bounds.size.x;
        }

        set => transform.localScale *= value / Size;
    }
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        characterText.text = data.character;
        positionData.position = data.position;
        SetVisibility(data.isVisible);
    }

    public abstract void SetVisibility(bool value);
    public bool GetVisibility() => _visible;

    public void ToggleVisibility()
    {
        SetVisibility(!_visible);
    }
}
