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

        int totalDispenserCapacity = levelData.Dispensers?.Sum(d => d.CellCount) ?? 0;

        if (totalLetters > totalActiveCells + totalDispenserCapacity)
            throw new InvalidOperationException("Букв слишком много для доступных ячеек и диспенсеров!");

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

        List<(char letter, WordWithRange word)> lettersQueue = new List<(char, WordWithRange)>();
        foreach (var w in levelData.Words)
            lettersQueue.AddRange(w.word.Select(ch => (ch, w)));

        int letterIndex = 0;

        bool TryPlaceInDispensers(int queueIndex)
        {
            if (levelData.Dispensers == null || levelData.Dispensers.Count == 0) return false;
            var entry = lettersQueue[queueIndex];
            if (entry.letter == '\0') return true;
            foreach (var dispenser in levelData.Dispensers)
            {
                var data = new BoardEntityData(entry.letter.ToString(), new Vector2Int(-1, -1), true, entry.word.color, false);
                if (dispenser.TryAdd(data))
                {
                    lettersQueue[queueIndex] = ('\0', null);
                    return true;
                }
            }
            return false;
        }

        for (int r = 0; r <= maxFilledRowIndex; r++)
        {
            int lettersInThisRow = (r < maxFilledRowIndex) ? activeCellsPerRow[r] : partialRowLetters;

            List<int> indexesForRow = new List<int>();
            int i = letterIndex;
            while (indexesForRow.Count < lettersInThisRow && i < lettersQueue.Count)
            {
                var (ch, w) = lettersQueue[i];
                if (ch == '\0') { i++; continue; }
                if (w != null && r + 1 >= w.rowMin && r + 1 <= w.rowMax)
                {
                    indexesForRow.Add(i);
                }
                else
                {
                    if (TryPlaceInDispensers(i))
                    {
                        // letter moved to dispenser, advance
                    }
                }
                i++;
            }

            for (int j = 0; j < lettersQueue.Count && indexesForRow.Count < lettersInThisRow; j++)
            {
                if (indexesForRow.Contains(j)) continue;
                var (ch, w) = lettersQueue[j];
                if (ch == '\0') continue;
                if (w != null && r + 1 >= w.rowMin && r + 1 <= w.rowMax)
                {
                    indexesForRow.Add(j);
                }
                else
                {
                    TryPlaceInDispensers(j);
                }
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
                    if (ch == '\0')
                    {
                        board[r, col] = null;
                        occupied[r, col] = true;
                    }
                    else
                    {
                        board[r, col] = new BoardEntityData(ch.ToString(), new Vector2Int(r, col), true, w.color, false);
                        occupied[r, col] = true;
                        lettersQueue[letterPos] = ('\0', null);
                        while (letterIndex < lettersQueue.Count && lettersQueue[letterIndex].letter == '\0')
                            letterIndex++;
                    }
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
                board[r, c] = null;
                occupied[r, c] = true;
            }
        }

        if (levelData.Dispensers != null && levelData.Dispensers.Count > 0)
        {
            for (int idx = 0; idx < lettersQueue.Count; idx++)
            {
                var entry = lettersQueue[idx];
                if (entry.letter == '\0') continue;
                TryPlaceInDispensers(idx);
            }
        }

        bool allUsed = lettersQueue.All(l => l.letter == '\0');
        if (!allUsed)
        {
            var leftovers = lettersQueue.Where(l => l.letter != '\0').Select(l => l.letter).ToList();
            if (leftovers.Count > 0)
                throw new InvalidOperationException($"Не удалось разместить все буквы. Остались: {string.Join(", ", leftovers)}");
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