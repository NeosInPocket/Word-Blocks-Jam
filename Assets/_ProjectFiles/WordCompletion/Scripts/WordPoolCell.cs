
using TMPro;
using UnityEngine;

public class WordPoolCell : LetterCell
{
    [SerializeField] private TMP_Text characterText;
    private string _character;
    public string Character => _character;

    public void SetCharacter(string character)
    {
        characterText.text = character;
        _character = character;
    }
}