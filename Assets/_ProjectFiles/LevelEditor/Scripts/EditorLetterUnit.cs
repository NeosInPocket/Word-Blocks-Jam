using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorLetterUnit : LetterUnitBase
{
    [SerializeField] private TMP_Text characterText;
    [SerializeField] private Image characterFrame;
    
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

    public override bool Available
    {
        get => _available;
        set
        {
            _available = value;
            characterFrame.color = value ? Color.white : Color.gray;
        }
    }

    protected override void SetCharacterData(string character)
    {
        characterText.text = character;
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterFrame.color = value ? Color.white : Color.black;
    }
}