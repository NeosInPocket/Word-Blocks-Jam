#if UNITY_EDITOR
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
            if (LevelEditingToggleState.EnableColoring)
            {
                SetUnitColor(value);
            }
        }
    }
    
    private Color _color;

    private void SetUnitColor(Color color)
    {
        var renderer = rendererObject.GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor("_Color", color);
        renderer.SetPropertyBlock(block);
    }
    
    public override bool Available
    {
        get => _available;
        set
        {
            _available = value;
            characterFrame.color = value ? Color.white : Color.gray;
        }
    }

    protected override void SetCharacterData(BoardEntityData data)
    {
        characterText.text = data.character;
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterFrame.color = value ? Color.white : Color.black;
    }

    public override void EnableVisibilityEffect()
    {
        
    }
}
#endif