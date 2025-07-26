using TMPro;
using UnityEngine;

public class LetterUnit : MonoBehaviour
{
    [SerializeField] private TMP_Text _characterText;
    private BoardEntityData _data;
    public BoardEntityData Data => _data;
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        _characterText.text = data.character.ToString();
    }
}
