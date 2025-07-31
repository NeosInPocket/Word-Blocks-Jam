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

    public bool HasCharacter(string character)
    {
        return _queue.CurrentWord.Contains(character);
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
        var characterIndex = _queue.CurrentWord.IndexOf(character);
        var cell = _cells[characterIndex];
        cell.SetCharacter(character);

        if (TryCompleteWord())
        {
            _queue.MoveNext();
            BuildPool(_queue);
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
