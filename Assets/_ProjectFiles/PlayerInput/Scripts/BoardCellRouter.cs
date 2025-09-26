using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using NSubstitute;
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
        if (obj.Occupant != null)
        {
            if (!obj.Occupant.Available)
            {
                HapticManager.Instance.PlayHaptic();
                AudioManager.Instance.PlayClick();
                
                obj.Occupant.GetComponent<LetterUnit>().Blink(true);
                return;
            }
            
            HapticManager.Instance.PlayHaptic();
            obj.Occupant.GetComponent<LetterUnit>().Blink(false);
        }
        else
        {
            return;
        }
        
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
            unusedLettersPoolView.TryAdd(obj);
        }
        
        boardView.RefreshCells();
    }
}
