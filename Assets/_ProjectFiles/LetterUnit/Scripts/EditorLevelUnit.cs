public class EditorLevelUnit : LetterUnitBase
{
    public override void SetVisibility(bool value)
    {
        _visible = value;
        var color = characterText.color;
        color.a = value ? 1f :  0.3f;
        characterText.color = color;
        
        var frameColor = characterFrame.color;
        color.a = value ? 1f :  0.5f;
        characterFrame.color = frameColor;
    }
}