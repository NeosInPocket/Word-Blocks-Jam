using System.Collections.Generic;
using UnityEngine;

public class UnusedLetterPoolObject : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private ButtonCellView buttonPrefab;
    private List<ButtonCellView> _buttons;
    
    public List<Transform> SpawnPositions => spawnPositions;

    public void AddButtons()
    {
        _buttons = new  List<ButtonCellView>();
        
        foreach (var pos in spawnPositions)
        {
            var button = Instantiate(buttonPrefab, pos.position, buttonPrefab.transform.rotation, transform);
            _buttons.Add(button);
        }
    }
}