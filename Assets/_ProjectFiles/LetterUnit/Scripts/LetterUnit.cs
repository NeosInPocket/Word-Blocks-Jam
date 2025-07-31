using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterUnit : MonoBehaviour
{
    [SerializeField] private PositionData positionData;
    [SerializeField] private TMP_Text characterText;
    [SerializeField] private Image characterFrame;
    private BoardEntityData _data;
    private bool _avaliable;
    public BoardEntityData Data => _data;
    public PositionData PositionData => positionData;
    public string Character => characterText.text;

    public bool Avaliable
    {
        get => _avaliable;
        set
        {
            _avaliable = value;
            characterFrame.color = value ? Color.white : Color.gray;
        }
    }
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        characterText.text = data.character;
        positionData.position = data.position;
    }
}
