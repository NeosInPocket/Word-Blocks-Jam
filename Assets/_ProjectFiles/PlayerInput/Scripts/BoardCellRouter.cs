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
        if (obj.IsWaitingForDispenser) return;
        
        try
        {
            if (obj.Occupant != null)
            {
                if (!obj.Occupant.Available)
                {
                    HapticManager.Instance.PlayHaptic();
                    obj.Occupant.GetComponent<LetterUnit>().Blink(true);
                    return;
                }

                AudioManager.Instance.PlayClick();
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
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}
