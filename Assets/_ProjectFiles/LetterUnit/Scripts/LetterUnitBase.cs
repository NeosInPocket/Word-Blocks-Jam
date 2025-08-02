using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class LetterUnitBase : MonoBehaviour
{
    [SerializeField] private PositionData positionData;
    [SerializeField] protected TMP_Text characterText;
    [SerializeField] protected Image characterFrame;
    private BoardEntityData _data;
    private bool _avaliable;
    protected bool _visible;
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
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        characterText.text = data.character;
        positionData.position = data.position;
    }

    public abstract void SetVisibility(bool value);

    public void ToggleVisibility()
    {
        SetVisibility(!_visible);
    }
}
