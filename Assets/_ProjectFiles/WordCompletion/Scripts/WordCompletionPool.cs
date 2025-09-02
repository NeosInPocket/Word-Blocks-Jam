using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordCompletionPool : UnusedLettersPoolView
{
    [SerializeField] private GameObject winObject;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private UnusedLettersPoolView pool;
    private WordsQueue _queue;
    
    public void BuildPool(WordsQueue wordsQueue)
    {
        _queue = wordsQueue;
        BuildPool(wordsQueue.CurrentWord.Length);
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

    public void SetCharacter(LetterUnitBase character)
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
        cell.SetOccupant(character);
        character.Navigation.SetTarget(_model.SpawnPositions[freePosition]);
        
        if (TryCompleteWord())
        {
            if (_queue.MoveNext())
            {
                BuildPool(_queue);
                var lettersInPool = pool.Cells.Where(x => x.Occupant != null).Select(x => x.Occupant.Character).ToList();
                
                var matchingCharacters = _queue.CurrentWord.Where(x => lettersInPool.Contains(x.ToString())).ToList();
        
                if (matchingCharacters.Any())
                {
                    foreach (var ch in matchingCharacters)
                    {
                        var poolCell = pool.Cells.FirstOrDefault(x => x.Occupant != null && x.Occupant.Character == ch.ToString());
                        
                        // перемещение мипла в основной пул
                        
                        //poolCell.DestroyOccupant();
                        //SetCharacter(ch.ToString());
                    }
                }
            }
            else
            {
                winObject.SetActive(true);
                tracker.gameObject.SetActive(false);
                SaveManager.Load();
                SaveManager.Data.levelsCompleted++;
                SaveManager.Save();
            }
        }
    }

    private bool TryCompleteWord()
    {
        string word = String.Empty;
        
        foreach (var cell in Cells)
        {
            if (cell.Occupant == null) continue;
            word += cell.Occupant.Character;
        }
        
        return _queue.CurrentWord.Equals(word);
    }
}
