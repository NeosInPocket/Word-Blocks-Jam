using UnityEngine;

public class BoardCellRouter : MonoBehaviour
{
    [SerializeField] private UnusedLettersPoolView unusedLettersPoolView; 
    [SerializeField] private WordCompletionPool wordPool; 
    
    private void Start()
    {
        PlayerSelectionTracker.Instance.OnBoardCellTapped += OnBoardCellTapped;
    }

    private void OnBoardCellTapped(BoardCell obj)
    {
        var mainPoolHasCharacter = wordPool.HasCharacter(obj.Occupant.Character);

        if (mainPoolHasCharacter)
        {
            wordPool.SetCharacter(obj.Occupant.Character);
            Destroy(obj.gameObject);
        }
        else
        {
            unusedLettersPoolView.Add(obj.Occupant);
        }
    }

    private void OnDestroy()
    {
        PlayerSelectionTracker.Instance.OnBoardCellTapped -= OnBoardCellTapped;
    }
}
