using UnityEngine;

public class EditorLetterUnit : LetterUnitBase
{
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            characterText.color = value;
        }
    }
    
    private Color _color;
    
    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterFrame.color = value ? Color.white : Color.black;
    }
}