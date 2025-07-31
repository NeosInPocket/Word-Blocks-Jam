using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordCompletionPool : MonoBehaviour
{
    [SerializeField] private VisualArea area;
    [SerializeField] private WordPoolCell prefab;
    [SerializeField] private float spacingPerCellSize;
    [SerializeField] private Transform ySpawnTransform;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private GameObject winObject;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private UnusedLettersPoolView pool;
    private List<WordPoolCell> _cells;
    private WordsQueue _queue;
    
    public void BuildPool(WordsQueue wordsQueue)
    {
        _queue = wordsQueue;
        ClearPool();

        int wordLength = wordsQueue.CurrentWord.Length;
        
        var spawnAreaBounds = area.Collider.bounds;
        var minX = spawnAreaBounds.min.x;
        var maxX = spawnAreaBounds.max.x;
        var minZ = spawnAreaBounds.min.z;
        var maxZ = spawnAreaBounds.max.z;

        var zCenter = (maxZ + minZ) / 2;
        
        float poolLength = maxX - minX;
        
        float cellSize = poolLength / (wordLength + spacingPerCellSize * (wordLength - 1));
        float spacing = spacingPerCellSize * cellSize;

        Vector3 spawnPosition = new Vector3(minX + cellSize / 2, ySpawnTransform.position.y, zCenter);
        _cells = new List<WordPoolCell>();
        
        for (int i = 0; i < wordLength; i++)
        {
            var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
            cell.SideSize = cellSize;
            
            _cells.Add(cell);

            spawnPosition.x += cellSize + spacing;
        }
    }

    public bool HasNonFilledCharacter(string character)
    {
        var containsCharacter = _queue.CurrentWord.Contains(character);
        if (!containsCharacter) return false;
        
        var characterCount = _queue.CurrentWord.Count(x => x.ToString() == character);
        var cells = _cells.Where(x => x.Character == character).Select(x => x.Character).ToList();

        var result =  cells.Count != characterCount;
        return result;
    }

    public bool IsCharacterInPlace(string character)
    {
        var cells = _cells.Where(x => x.Character == character).ToList();
        var cellsCount = cells.Count;
        var characterCount = 0;

        foreach (var ch in _queue.CurrentWord)
        {
            if (ch.ToString() == character)
            {
                characterCount++;
            }
        }
        
        return cellsCount == characterCount;
    }

    public void SetCharacter(string character)
    {
        List<int> characterPositions = new List<int>();

        var word = _queue.CurrentWord;
        
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i].ToString() == character)
            {
                characterPositions.Add(i);
            }
        }

        int freePosition = 0;
        
        foreach (var pos in characterPositions)
        {
            if (_cells[pos].Character == character) continue;

            freePosition = pos;
            break;
        }
        
        var cell = _cells[freePosition];
        cell.SetCharacter(character);

        if (TryCompleteWord())
        {
            if (_queue.MoveNext())
            {
                BuildPool(_queue);
                var lettersInPool = pool.Cells.Where(x => x.Occupant != null).Select(x => x.Occupant.Character).ToList();
                
                var matchingCharacters = _queue.CurrentWord.Where(x => lettersInPool.Contains(x.ToString())).ToList();

                if (matchingCharacters.Any())
                {
                    foreach (var ch in lettersInPool)
                    {
                        var poolCell = pool.Cells.FirstOrDefault(x => x.Occupant != null && x.Occupant.Character == ch);
                        poolCell.RemoveOccupant();
                        SetCharacter(ch);
                    }
                }
            }
            else
            {
                winObject.SetActive(true);
                tracker.gameObject.SetActive(false);
            }
        }
    }

    private bool TryCompleteWord()
    {
        string word = String.Empty;
        
        foreach (var cell in _cells)
        {
            word += cell.Character;
        }
        
        return _queue.CurrentWord.Equals(word);
    }

    private void ClearPool()
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
