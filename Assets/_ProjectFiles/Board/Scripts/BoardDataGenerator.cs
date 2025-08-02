using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardDataGenerator
{
    public BoardEntityData[,] Generate(LevelData levelData)
    {
        int rows = levelData.Rows;
        int columns = levelData.Columns;
        bool[,] activeCells = levelData.ActiveCells;

        BoardEntityData[,] board = new BoardEntityData[rows, columns];
        bool[,] occupied = new bool[rows, columns];

        int totalLetters = levelData.Words.Sum(w => w.word.Length);

        int totalActiveCells = 0;
        int[] activeCellsPerRow = new int[rows];
        for (int r = 0; r < rows; r++)
        {
            int count = 0;
            for (int c = 0; c < columns; c++)
                if (activeCells[r, c]) count++;
            activeCellsPerRow[r] = count;
            totalActiveCells += count;
        }

        if (totalLetters > totalActiveCells)
        {
            throw new InvalidOperationException("Букв слишком много для доступных ячеек!");
        }

        int lettersPlaced = 0;
        int maxFilledRowIndex = -1;
        int fullRows = 0;
        int partialRowLetters = 0;

        {
            int sum = 0;
            for (int r = 0; r < rows; r++)
            {
                if (sum + activeCellsPerRow[r] < totalLetters)
                {
                    sum += activeCellsPerRow[r];
                    fullRows++;
                }
                else
                {
                    maxFilledRowIndex = r;
                    partialRowLetters = totalLetters - sum;
                    fullRows++;
                    break;
                }
            }
            if (maxFilledRowIndex == -1) maxFilledRowIndex = fullRows - 1;
        }

        List<(char letter, WordWithRange wordInfo)> lettersQueue = new List<(char, WordWithRange)>();
        foreach (var w in levelData.Words)
            lettersQueue.AddRange(w.word.Select(ch => (ch, w)));

        int letterIndex = 0;

        for (int r = 0; r <= maxFilledRowIndex; r++)
        {
            int lettersInThisRow = (r < maxFilledRowIndex) ? activeCellsPerRow[r] : partialRowLetters;

            List<int> indexesForRow = new List<int>();
            int i = letterIndex;
            while (indexesForRow.Count < lettersInThisRow && i < lettersQueue.Count)
            {
                var (ch, w) = lettersQueue[i];
                if (r + 1 >= w.rowMin && r + 1 <= w.rowMax && ch != '\0')
                {
                    indexesForRow.Add(i);
                }
                i++;
            }
            for (int j = 0; j < lettersQueue.Count && indexesForRow.Count < lettersInThisRow; j++)
            {
                if (indexesForRow.Contains(j)) continue;
                var (ch, w) = lettersQueue[j];

                if (w == null)
                {
                    throw new InvalidOperationException(
                        "Не удалось расставить буквы в ячейки поля, проверьте количество ячеек");
                }
                
                if (r + 1 >= w.rowMin && r + 1 <= w.rowMax && ch != '\0')
                    indexesForRow.Add(j);
            }
            if (indexesForRow.Count < lettersInThisRow)
            {
                int toFill = lettersInThisRow - indexesForRow.Count;
                for (int fill = 0; fill < toFill; fill++)
                    indexesForRow.Add(-1);
            }

            List<int> activePositions = new List<int>();
            for (int c = 0; c < columns; c++)
                if (activeCells[r, c] && !occupied[r, c])
                    activePositions.Add(c);
            activePositions.Shuffle();

            activePositions = activePositions.Take(lettersInThisRow).ToList();

            for (int posIndex = 0; posIndex < lettersInThisRow; posIndex++)
            {
                int col = activePositions[posIndex];
                int letterPos = indexesForRow[posIndex];
                if (letterPos == -1)
                {
                    board[r, col] = null;
                    occupied[r, col] = true;
                }
                else
                {
                    var (ch, w) = lettersQueue[letterPos];
                    board[r, col] = new BoardEntityData(ch.ToString(), new Vector2Int(r, col));
                    occupied[r, col] = true;
                    lettersQueue[letterPos] = ('\0', null);
                    while (letterIndex < lettersQueue.Count && lettersQueue[letterIndex].letter == '\0')
                        letterIndex++;
                }
            }
            
            foreach (var col in activePositions.Skip(lettersInThisRow))
            {
                if (!occupied[r, col])
                {
                    board[r, col] = null;
                    occupied[r, col] = true;
                }
            }
            
            for (int c = 0; c < columns; c++)
            {
                if (activeCells[r, c] && !occupied[r, c])
                {
                    board[r, c] = null;
                    occupied[r, c] = true;
                }
            }
        }

        
        for (int r = maxFilledRowIndex + 1; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (activeCells[r, c])
                {
                    board[r, c] = null;
                    occupied[r, c] = true;
                }
                else
                {
                    board[r, c] = null;
                    occupied[r, c] = true;
                }
            }
        }

        bool allUsed = lettersQueue.All(l => l.letter == '\0');
        if (!allUsed)
        {
            var leftovers = lettersQueue.Where(l => l.letter != '\0').Select(l => l.letter).ToList();
            Debug.LogWarning($"Не все буквы были размещены! Остались: {string.Join(", ", leftovers)}");
        }

        ShuffleLettersBetweenRows(board, levelData, maxFilledRowIndex, columns);

        return board;
    }

    private void ShuffleLettersBetweenRows(BoardEntityData[,] board, LevelData levelData, int maxFilledRowIndex, int columns)
    {
        System.Random rnd = new System.Random();

        for (int row = 0; row < maxFilledRowIndex; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                BoardEntityData letterA = board[row, col];
                if (letterA == null) continue;

                WordWithRange wordA = levelData.Words.FirstOrDefault(w => w.word.Contains(letterA.character));
                if (wordA == null) continue;

                if (wordA.rowMax <= row + 1) continue;

                int nextRow = row + 1;
                var candidates = new List<int>();
                for (int c2 = 0; c2 < columns; c2++)
                {
                    BoardEntityData letterB = board[nextRow, c2];
                    if (letterB == null) continue;

                    WordWithRange wordB = levelData.Words.FirstOrDefault(w => w.word.Contains(letterB.character));
                    if (wordB == null) continue;

                    bool letterBCanInArow = (row + 1) >= wordB.rowMin && (row + 1) <= wordB.rowMax;
                    bool letterACanInBrow = (nextRow + 1) >= wordA.rowMin && (nextRow + 1) <= wordA.rowMax;

                    if (letterBCanInArow && letterACanInBrow)
                    {
                        candidates.Add(c2);
                    }
                }

                if (candidates.Count > 0)
                {
                    int swapCol = candidates[rnd.Next(candidates.Count)];
                    BoardEntityData letterB = board[nextRow, swapCol];

                    board[row, col] = letterB;
                    board[nextRow, swapCol] = letterA;
                }
            }
        }
    }
}