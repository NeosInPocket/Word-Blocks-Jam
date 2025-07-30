using TMPro;
using UnityEngine;

public class LetterUnit : MonoBehaviour
{
    [SerializeField] private PositionData positionData;
    [SerializeField] private TMP_Text characterText;
    private BoardEntityData _data;
    public BoardEntityData Data => _data;
    public PositionData PositionData => positionData;
    public string Character => characterText.text;
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        characterText.text = data.character;
        positionData.position = data.position;
    }
}
