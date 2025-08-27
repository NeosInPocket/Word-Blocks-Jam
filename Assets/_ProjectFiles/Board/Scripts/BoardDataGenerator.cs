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

        var board = new BoardEntityData[rows, columns];
        var occupied = new bool[rows, columns];

        int totalLetters = levelData.Words.Sum(w => w.word.Length);
        var (activeCellsPerRow, totalActiveCells) = CalculateActiveCellsPerRow(activeCells, rows, columns);
        int totalDispenserCapacity = levelData.Dispensers?.Sum(d => d.CellCount) ?? 0;

        ValidateCapacity(totalLetters, totalActiveCells, totalDispenserCapacity);

        var (maxFilledRowIndex, partialRowLetters) = CalculateFilledRows(
            totalLetters, activeCellsPerRow, rows);

        var lettersQueue = BuildLettersQueue(levelData.Words);

        int letterIndex = 0;

        for (int r = 0; r <= maxFilledRowIndex; r++)
        {
            int lettersInThisRow = (r < maxFilledRowIndex) ? activeCellsPerRow[r] : partialRowLetters;
            var indexesForRow = SelectIndexesForRow(lettersQueue, r, lettersInThisRow, levelData, TryPlaceInDispensers, ref letterIndex);
            MapLettersToRow(board, occupied, activeCells, r, columns, indexesForRow, lettersQueue, ref letterIndex, lettersInThisRow);
        }

        //FillRemainingRowsWithNull(board, occupied, activeCells, maxFilledRowIndex + 1, rows, columns);
        PlaceRemainingInDispensers(lettersQueue, levelData, TryPlaceInDispensers);
        ValidateAllLettersUsed(lettersQueue);
        ShuffleLettersBetweenRows(board, levelData, maxFilledRowIndex, columns);

        return board;

        bool TryPlaceInDispensers(int queueIndex)
            => TryPlaceLetterInDispensers(queueIndex, levelData, lettersQueue);

    }

    private (int[] activeCellsPerRow, int totalActiveCells) CalculateActiveCellsPerRow(bool[,] activeCells, int rows, int columns)
    {
        int[] activeCellsPerRow = new int[rows];
        int totalActiveCells = 0;
        for (int r = 0; r < rows; r++)
        {
            int count = 0;
            for (int c = 0; c < columns; c++)
                if (activeCells[r, c]) count++;
            activeCellsPerRow[r] = count;
            totalActiveCells += count;
        }
        return (activeCellsPerRow, totalActiveCells);
    }

    private void ValidateCapacity(int totalLetters, int totalActiveCells, int totalDispenserCapacity)
    {
        if (totalLetters > totalActiveCells + totalDispenserCapacity)
            throw new InvalidOperationException("Букв слишком много для доступных ячеек и диспенсеров!");
    }

    private (int maxFilledRowIndex, int partialRowLetters) CalculateFilledRows(
        int totalLetters, int[] activeCellsPerRow, int rows)
    {
        int maxFilledRowIndex = -1;
        int fullRows = 0;
        int partialRowLetters = 0;
        int sum = 0;
        int defaultCellsCountInRow = activeCellsPerRow.Max();
        
        for (int r = 0; r < rows; r++)
        {
            int cellsInRow = activeCellsPerRow[r];

            if (cellsInRow < defaultCellsCountInRow)
                partialRowLetters = cellsInRow;
            
            if (sum + activeCellsPerRow[r] < totalLetters)
            {
                sum += activeCellsPerRow[r];
                fullRows++;
            }
            else
            {
                maxFilledRowIndex = r;
                fullRows++;
                break;
            }
        }
        if (maxFilledRowIndex == -1) maxFilledRowIndex = fullRows - 1;
        return (maxFilledRowIndex, partialRowLetters);
    }

    private List<(char letter, WordWithRange word)> BuildLettersQueue(List<WordWithRange> words)
    {
        var lettersQueue = new List<(char, WordWithRange)>();
        foreach (var w in words)
            lettersQueue.AddRange(w.word.Select(ch => (ch, w)));
        return lettersQueue;
    }

    private bool TryPlaceLetterInDispensers(
        int queueIndex,
        LevelData levelData,
        List<(char letter, WordWithRange word)> lettersQueue)
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

    private List<int> SelectIndexesForRow(
        List<(char letter, WordWithRange word)> lettersQueue, int row,
        int lettersNeeded, LevelData levelData, Func<int, bool> tryPlaceInDispensers, ref int letterIndex)
    {
        List<int> indexesForRow = new List<int>();
        int i = letterIndex;
        while (indexesForRow.Count < lettersNeeded && i < lettersQueue.Count)
        {
            var (ch, w) = lettersQueue[i];
            if (ch == '\0') { i++; continue; }
            if (w != null && row + 1 >= w.rowMin && row + 1 <= w.rowMax)
            {
                indexesForRow.Add(i);
            }
            else
            {
                tryPlaceInDispensers(i);
            }
            i++;
        }

        for (int j = 0; j < lettersQueue.Count && indexesForRow.Count < lettersNeeded; j++)
        {
            if (indexesForRow.Contains(j)) continue;
            var (ch, w) = lettersQueue[j];
            if (ch == '\0') continue;
            if (w != null && row + 1 >= w.rowMin && row + 1 <= w.rowMax)
            {
                indexesForRow.Add(j);
            }
            else
            {
                tryPlaceInDispensers(j);
            }
        }

        if (indexesForRow.Count < lettersNeeded)
        {
            int toFill = lettersNeeded - indexesForRow.Count;
            for (int fill = 0; fill < toFill; fill++)
                indexesForRow.Add(-1);
        }
        return indexesForRow;
    }

    private void MapLettersToRow(
        BoardEntityData[,] board, bool[,] occupied, bool[,] activeCells,
        int r, int columns, List<int> indexesForRow,
        List<(char letter, WordWithRange word)> lettersQueue,
        ref int letterIndex, int lettersInThisRow)
    {
        var activePositions = GetActiveCellPositions(activeCells, occupied, r, columns);
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

    private List<int> GetActiveCellPositions(bool[,] activeCells, bool[,] occupied, int row, int columns)
    {
        var activePositions = new List<int>();
        for (int c = 0; c < columns; c++)
            if (activeCells[row, c] && !occupied[row, c])
                activePositions.Add(c);
        return activePositions;
    }

    private void FillRemainingRowsWithNull(BoardEntityData[,] board, bool[,] occupied, bool[,] activeCells, int startRow, int rows, int columns)
    {
        for (int r = startRow; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                board[r, c] = null;
                occupied[r, c] = true;
            }
        }
    }

    private void PlaceRemainingInDispensers(
        List<(char letter, WordWithRange word)> lettersQueue,
        LevelData levelData,
        Func<int, bool> tryPlaceInDispensers)
    {
        if (levelData.Dispensers != null && levelData.Dispensers.Count > 0)
        {
            for (int idx = 0; idx < lettersQueue.Count; idx++)
            {
                var entry = lettersQueue[idx];
                if (entry.letter == '\0') continue;
                tryPlaceInDispensers(idx);
            }
        }
    }

    private void ValidateAllLettersUsed(List<(char letter, WordWithRange word)> lettersQueue)
    {
        bool allUsed = lettersQueue.All(l => l.letter == '\0');
        if (!allUsed)
        {
            var leftovers = lettersQueue.Where(l => l.letter != '\0').Select(l => l.letter).ToList();
            if (leftovers.Count > 0)
                throw new InvalidOperationException($"Не удалось разместить все буквы. Остались: {string.Join(", ", leftovers)}");
        }
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