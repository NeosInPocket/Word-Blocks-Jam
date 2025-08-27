using UnityEngine;

public class BoardCellRouter : MonoBehaviour
{
    [SerializeField] private UnusedLettersPoolView unusedLettersPoolView; 
    [SerializeField] private WordCompletionPool wordPool; 
    [SerializeField] private BoardView boardView; 
    
    private void Start()
    {
        PlayerSelectionTracker.Instance.OnBoardCellTapped += OnBoardCellTapped;
    }

    private void OnBoardCellTapped(BoardCellView obj)
    {
        if (obj.Occupant == null || !obj.Occupant.Available) return;
        
        var mainPoolHasCharacter = wordPool.HasNonFilledCharacter(obj.Occupant.Character);

        if (mainPoolHasCharacter)
        {
            wordPool.SetCharacter(obj.Occupant.Character);
            obj.DestroyOccupant();
        }
        else
        {
            if (unusedLettersPoolView.TryAdd(obj.Occupant))
                obj.RemoveOccupant();
        }
        
        boardView.RefreshCells();
    }

    private void OnDestroy()
    {
        PlayerSelectionTracker.Instance.OnBoardCellTapped -= OnBoardCellTapped;
    }
}
