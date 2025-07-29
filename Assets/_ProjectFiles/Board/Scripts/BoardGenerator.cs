using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator
{
    public BoardEntityData[,] Generate(LevelData levelData)
    {
        int rows = levelData.Rows;
        int columns = levelData.Columns;

        BoardEntityData[,] board = new BoardEntityData[rows, columns];
        bool[,] occupied = new bool[rows, columns];

        List<Vector2Int>[] rowSlots = new List<Vector2Int>[rows];
        for (int r = 0; r < rows; r++)
        {
            rowSlots[r] = new List<Vector2Int>();
            for (int c = 0; c < columns; c++)
                rowSlots[r].Add(new Vector2Int(r, c));
        }

        foreach (var row in rowSlots)
            row.Shuffle();

        int activeRow = 0;

        foreach (var wordEntry in levelData.Words)
        {
            string word = wordEntry.word;
            int minRow = Mathf.Clamp(wordEntry.rowMin - 1, 0, rows - 1);
            int maxRow = Mathf.Clamp(wordEntry.rowMax - 1, 0, rows - 1);

            List<Vector2Int> validSlots = new();

            for (int r = Mathf.Max(activeRow, minRow); r <= maxRow && r < rows; r++)
            {
                validSlots.AddRange(rowSlots[r].Where(pos => !occupied[pos.x, pos.y]));
            }

            if (validSlots.Count < word.Length)
            {
                Debug.LogWarning($"Недостаточно места для слова '{word}' в строках {minRow}-{maxRow}, начиная с {activeRow}");
                continue;
            }

            validSlots.Shuffle();

            for (int i = 0; i < word.Length; i++)
            {
                Vector2Int pos = validSlots[i];

                board[pos.x, pos.y] = new BoardEntityData(word[i].ToString(), pos);
                occupied[pos.x, pos.y] = true;
                rowSlots[pos.x].Remove(pos);

                if (IsRowFull(occupied, pos.x, columns) && pos.x == activeRow)
                    activeRow++;
            }
        }

        return board;
    }

    private bool IsRowFull(bool[,] occupied, int row, int totalColumns)
    {
        for (int c = 0; c < totalColumns; c++)
        {
            if (!occupied[row, c])
                return false;
        }
        return true;
    }
}
