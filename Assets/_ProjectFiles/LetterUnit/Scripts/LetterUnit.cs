using UnityEngine;

public class LetterUnit : LetterUnitBase
{
    [SerializeField] private LetterUnitCharacterRenderer characterRenderer;

    public override bool Available
    {
        get => _available;
        set => _available = value;
    }

    protected override void SetCharacterData(BoardEntityData data)
    {
        characterRenderer.SetCharacter(data.character);
        SetUnitColor(data.color);
    }

    private void SetUnitColor(Color color)
    {
        if (_renderer == null)
            _renderer = rendererObject.GetComponent<Renderer>();
        
        var mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(mpb);
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterRenderer.SetCharacterVisibility(value);
    }
}
