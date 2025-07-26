using UnityEngine;

public class BoardEntityData
{
    private char _character;
    
    public char Character => _character;

    public BoardEntityData(char character)
    {
        SetCharacter(character);
    }

    public void SetCharacter(char character)
    {
        _character = character;
    }
}
