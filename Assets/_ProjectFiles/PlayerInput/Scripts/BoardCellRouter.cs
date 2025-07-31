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

    private void OnBoardCellTapped(BoardCell obj)
    {
        if (!obj.Occupant.Avaliable) return;
        
        var mainPoolHasCharacter = wordPool.HasNonFilledCharacter(obj.Occupant.Character);

        if (mainPoolHasCharacter)
        {
            wordPool.SetCharacter(obj.Occupant.Character);
            Destroy(obj.Occupant.gameObject);
            boardView.UnlockCellAfterCell(obj);
        }
        else
        {
            if (unusedLettersPoolView.TryAdd(obj.Occupant))
            {
                boardView.UnlockCellAfterCell(obj);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerSelectionTracker.Instance.OnBoardCellTapped -= OnBoardCellTapped;
    }
}
