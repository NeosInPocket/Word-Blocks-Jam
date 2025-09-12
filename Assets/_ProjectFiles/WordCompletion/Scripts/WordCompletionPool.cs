using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class WordCompletionPool : UnusedLettersPoolView
{
    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject loseObject;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private UnusedLettersPoolView unusedLettersPool;
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

    public async UniTask SetCharacter(LetterUnitBase character, bool findPath = true)
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

        if (findPath)
        {
            var gridPath = GetLetterUnitPathGrid(character.PositionData.position);
            await character.Navigation.SetTarget(_model.SpawnPositions[freePosition], true, _model.Buttons[freePosition], gridPath);
        }
        else
        {
            await character.Navigation.SetTarget(_model.SpawnPositions[freePosition], true, _model.Buttons[freePosition], null);
        }
        
        Debug.Log(CheckLose());
        
        var cellWithOccupants = Cells.Where(x => x.Occupant != null).ToList();
        var someAreNotInPlace = cellWithOccupants.Any(x => !x.Occupant.Navigation.InFinalPlace);
        var missingLetters = cellWithOccupants.Count != _queue.CurrentWord.Length;
        
        if (missingLetters || someAreNotInPlace)
        {
            return;
        }
        
        if (TryCompleteWord())
        {
            if (_queue.MoveNext())
            {
                await BuildNextPool();
                
                var lettersInPool = unusedLettersPool.Cells.Where(x => x.Occupant != null).Select(x => x.Occupant.Character).ToList();
                var matchingCharacters = _queue.CurrentWord.Where(x => lettersInPool.Contains(x.ToString())).ToList();
        
                if (matchingCharacters.Any())
                {
                    foreach (var ch in matchingCharacters)
                    {
                        var poolCell = unusedLettersPool.Cells.FirstOrDefault(x => x.Occupant != null && x.Occupant.Character == ch.ToString());
                        if (poolCell == null) continue;
                        
                        SetCharacter(poolCell.Occupant, false).Forget(x => Debug.Log(x.Message));
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
        else
        {
            if (CheckLose())
            {
                winObject.SetActive(true);
                tracker.Enable(false);
                SaveManager.Load();
                SaveManager.Data.levelsCompleted++;
                SaveManager.Save();
            }
        }
    }

    private bool CheckLose()
    {
        if (unusedLettersPool.IsFull)
        {
            Debug.Log("is full");
            var missingLetters = Cells.Where(x => x.Occupant == null).Select(x => x.Occupant.Character).ToList();
            
            if (missingLetters.Any())
            {
                Debug.Log("missing characters");
                
                var availableBoardLetters = boardView.Cells.Where(x => x.Occupant.Available);
                var matchingCharacters = availableBoardLetters.Where(x => missingLetters.Contains(x.Occupant.Character)).ToList();
                var isLose = !matchingCharacters.Any();

                Debug.Log("lose " + isLose);
                return isLose;
            }
        }

        return false;
    }
    
    private async UniTask BuildNextPool()
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
