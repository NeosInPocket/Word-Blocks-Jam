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
        characterRenderer.SetUnitColor(data.color);
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterRenderer.SetCharacterVisibility(value);
    }
}
