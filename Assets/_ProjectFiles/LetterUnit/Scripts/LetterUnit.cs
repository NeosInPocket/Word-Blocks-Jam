public class LetterUnit : LetterUnitBase
{
    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterText.enabled = value;
    }
}
