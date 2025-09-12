using Cysharp.Threading.Tasks;
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

        boardView.EnableAgents();
        var mainPoolHasCharacter = wordPool.HasNonFilledCharacter(obj.Occupant.Character);

        if (mainPoolHasCharacter)
        {
            if (wordPool.IsBuildingPool) return;
            
            wordPool.SetCharacter(obj.Occupant).Forget(x => Debug.Log(x.Message));
            obj.RemoveOccupant();
        }
        else
        {
            if (unusedLettersPoolView.TryAdd(obj.Occupant))
                obj.RemoveOccupant();
        }
        
        boardView.RefreshCells();
    }
}
