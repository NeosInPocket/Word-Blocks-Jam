using UnityEngine;

public class EditorLetterUnit : LetterUnitBase
{
    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterText.color = value ? Color.black : Color.white;
        characterFrame.color = value ? Color.white : Color.black;
    }
}