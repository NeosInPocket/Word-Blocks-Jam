using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UnusedLetterPoolObject : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private List<GameObject> feedbacks;
    [SerializeField] private ButtonCellView buttonPrefab;
    [SerializeField] private PoolCanvasLetters canvasLetters;
    [SerializeField] private GameObject extraCell;
    [SerializeField] private Transform extraCellSpawnPosition;
    private List<ButtonCellView> _buttons;
    
    public List<Transform> SpawnPositions => spawnPositions;
    public List<GameObject> Feedbacks => feedbacks;
    public List<ButtonCellView> Buttons => _buttons;
    public PoolCanvasLetters CanvasLetters => canvasLetters;
    public Transform ExtraCellSpawnPosition => extraCellSpawnPosition;

    public void AddButtons()
    {
        _buttons = new  List<ButtonCellView>();
        
        foreach (var pos in spawnPositions)
        {
            var button = Instantiate(buttonPrefab, pos.position, buttonPrefab.transform.rotation, transform);
            _buttons.Add(button);
        }
    }

    public void AddExtraCell()
    {
        extraCell.gameObject.SetActive(true);
    }
}