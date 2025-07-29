using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardValidator
{
    public bool Validate(BoardEntityData[,] board, List<WordWithRange> words)
    {
        foreach (var wordEntry in words)
        {
            if (!ValidateWord(board, wordEntry))
            {
                Debug.LogWarning($"Слово '{wordEntry.word}' не найдено в допустимом диапазоне ({wordEntry.rowMin}-{wordEntry.rowMax})");
                return false;
            }
        }

        return true;
    }

    private bool ValidateWord(BoardEntityData[,] board, WordWithRange wordEntry)
    {
        string word = wordEntry.word.ToLowerInvariant();
        int minRow = Mathf.Clamp(wordEntry.rowMin, 0, board.GetLength(0) - 1);
        int maxRow = Mathf.Clamp(wordEntry.rowMax, 0, board.GetLength(0) - 1);

        List<char> wordChars = word.ToList();

        List<char> lettersInRange = new();

        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                var cell = board[row, col];
                if (cell != null)
                {
                    lettersInRange.Add(char.ToLowerInvariant(cell.character[0]));
                }
            }
        }

        foreach (char c in wordChars)
        {
            if (!lettersInRange.Remove(c))
            {
                return false;
            }
        }

        return true;
    }
}