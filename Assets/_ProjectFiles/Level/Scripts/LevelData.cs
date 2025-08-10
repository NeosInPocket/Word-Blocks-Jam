using System;
using System.Collections.Generic;
using System.Linq;
using _ProjectFiles._Features.LetterDispenser.Scripts;

public class LevelData
{
    private List<WordWithRange> _words;
    private int _rows;
    private int _columns;
    private int _poolCapacity;
    private bool[,] _activeCells;
    private List<LetterDispenserData> _dispensers;
    public List<WordWithRange> Words => _words; 
    public int Rows => _rows;
    public int Columns => _columns;
    public int PoolCapacity => _poolCapacity;
    public bool[,] ActiveCells => _activeCells;
    public List<LetterDispenserData> Dispensers => _dispensers;
    
    public LevelData(List<WordWithRange> words, int rows, int columns, int poolCapacity, bool[,] activeCells, List<LetterDispenserData> dispensers)
    {
        if (activeCells.GetLength(0) != rows || activeCells.GetLength(1) != columns)
            throw new ArgumentException("Размеры activeCells должны соответствовать rows и columns");

        _dispensers = dispensers;
        _rows = rows;
        _columns = columns;
        _words = words;
        _poolCapacity = poolCapacity;
        _activeCells = activeCells;
    }
    
    public static void CalculateOptimalRowRanges(List<WordWithRange> words, int rows, int cols)
{
    if (words == null || words.Count == 0 || rows <= 0 || cols <= 0)
        return;

    int[] occupiedCellsInRow = new int[rows];

    List<WordWithRange> wordOrder = new List<WordWithRange>(words);

    foreach (var w in wordOrder)
    {
        int wordLength = w.word.Length;
        int minRowSpan = Math.Max(2, Math.Min(wordLength, rows));

        int assignedRowMin = 1;
        int assignedRowMax = assignedRowMin + minRowSpan - 1;
        if (assignedRowMax > rows) assignedRowMax = rows;

        bool foundPlace = false;
        while (assignedRowMax <= rows)
        {
            int freeCellsCount = 0;
            for (int r = assignedRowMin - 1; r <= assignedRowMax - 1; r++)
            {
                freeCellsCount += (cols - occupiedCellsInRow[r]);
            }

            if (freeCellsCount >= wordLength)
            {
                foundPlace = true;
                break;
            }

            assignedRowMin++;
            assignedRowMax++;
            if (assignedRowMin > rows) break;
        }

        if (!foundPlace)
        {
            assignedRowMin = 1;
            assignedRowMax = assignedRowMin + minRowSpan - 1;
            if (assignedRowMax > rows) assignedRowMax = rows;

            while (assignedRowMax < rows)
            {
                assignedRowMax++;
                int freeCellsCount = 0;
                for (int r = assignedRowMin - 1; r <= assignedRowMax - 1; r++)
                    freeCellsCount += (cols - occupiedCellsInRow[r]);
                if (freeCellsCount >= wordLength)
                {
                    foundPlace = true;
                    break;
                }
            }

            if (!foundPlace)
            {
                assignedRowMin = 1;
                assignedRowMax = rows;
            }
        }

        w.rowMin = assignedRowMin;
        w.rowMax = assignedRowMax;

        int rowsSpan = assignedRowMax - assignedRowMin + 1;
        int baseCountPerRow = wordLength / rowsSpan;
        int extra = wordLength % rowsSpan;
        for (int r = assignedRowMin - 1; r <= assignedRowMax - 1; r++)
        {
            occupiedCellsInRow[r] += baseCountPerRow;
            if (extra > 0)
            {
                occupiedCellsInRow[r]++;
                extra--;
            }
            
            if (occupiedCellsInRow[r] > cols)
                occupiedCellsInRow[r] = cols;
        }
    }
}
}
