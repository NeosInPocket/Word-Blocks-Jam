using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class WordCompletionPool : UnusedLettersPoolView
{
    [SerializeField] private GameObject winObject;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private UnusedLettersPoolView pool;
    [SerializeField] private float poolAnimationDuration;
    private WordsQueue _queue;
    private bool _isBuildingPool;
    public bool IsBuildingPool => _isBuildingPool;

    public void BuildPool(WordsQueue wordsQueue)
    {
        _queue = wordsQueue;
        BuildPool(wordsQueue.CurrentWord.Length);
        _model.AddButtons();
        _model.CanvasLetters.SetWord(wordsQueue.CurrentWord);
    }

    public bool HasNonFilledCharacter(string character)
    {
        var containsCharacter = _queue.CurrentWord.Contains(character);
        if (!containsCharacter) return false;
        
        var characterCount = _queue.CurrentWord.Count(x => x.ToString() == character);
        var cells = Cells.Where(x => x.Occupant != null && x.Occupant.Character == character).Select(x => x.Occupant.Character).ToList();

        var result =  cells.Count != characterCount;
        return result;
    }

    public async Task SetCharacter(LetterUnitBase character)
    {
        List<int> characterPositions = new List<int>();

        var word = _queue.CurrentWord;
        
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i].ToString() == character.Character)
            {
                characterPositions.Add(i);
            }
        }

        int freePosition = 0;
        
        foreach (var pos in characterPositions)
        {
            if (Cells[pos].Occupant != null && Cells[pos].Occupant.Character == character.Character) continue;

            freePosition = pos;
            break;
        }
        
        var cell = Cells[freePosition];
        cell.SetOccupant(character.GetComponent<LetterUnit>());
        await character.Navigation.SetTarget(_model.SpawnPositions[freePosition], true, _model.Buttons[freePosition]);

        var someAreNotInPlace = Cells.Any(x => !x.Occupant.Navigation.InPlace);
        var missingLetters = Cells.Count(x => x.Occupant != null) != _queue.CurrentWord.Length;
        
        if (missingLetters || someAreNotInPlace)
        {
            return;
        }
        
        if (TryCompleteWord())
        {
            if (_queue.MoveNext())
            {
                await BuildNextPool();
                
                var lettersInPool = pool.Cells.Where(x => x.Occupant != null).Select(x => x.Occupant.Character).ToList();
                var matchingCharacters = _queue.CurrentWord.Where(x => lettersInPool.Contains(x.ToString())).ToList();
        
                if (matchingCharacters.Any())
                {
                    foreach (var ch in matchingCharacters)
                    {
                        var poolCell = pool.Cells.FirstOrDefault(x => x.Occupant != null && x.Occupant.Character == ch.ToString());
                        if (poolCell == null) continue;
                        
                        SetCharacter(poolCell.Occupant);
                        poolCell.RemoveOccupant();
                    }
                }
            }
            else
            {
                winObject.SetActive(true);
                tracker.Enable(false);
                SaveManager.Load();
                SaveManager.Data.levelsCompleted++;
                SaveManager.Save();
            }
        }
    }

    private async Task WaitForAllUnits()
    {
        while (Cells.Any(x => !x.Occupant.Navigation.InPlace))
        {
            await Task.Yield();
        }
    }
    
    private async Task BuildNextPool()
    {
        _isBuildingPool = true;
        
        foreach (var cell in Cells)
        {
            var occupant = cell.Occupant;
            occupant.PopEffect.Play(occupant.transform.position);
            Destroy(occupant.gameObject);
        }

        foreach (var button in _model.Buttons)
        {
            button.StopMove();
        }

        await _model.transform
            .DOMoveX(_model.transform.position.x - Camera.main.orthographicSize, poolAnimationDuration)
            .SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        
        BuildPool(_queue);
        
        var modelOffsetPosition = _model.transform.position;
        var modelStartPosition = modelOffsetPosition;
        modelOffsetPosition.x += Camera.main.orthographicSize;
        
        _model.transform.position = modelOffsetPosition;
        
        await _model.transform.DOMove(modelStartPosition, poolAnimationDuration).SetEase(Ease.InOutQuad)
            .AsyncWaitForCompletion();
        
        _isBuildingPool = false;
    }
    
    private bool TryCompleteWord()
    {
        if (_queue.CurrentWord == null) return false;
        
        string word = String.Empty;
        
        foreach (var cell in Cells)
        {
            if (cell.Occupant == null) continue;
            word += cell.Occupant.Character;
        }
        
        return _queue.CurrentWord.Equals(word);
    }
}
