using System;
using System.Collections.Generic;
using System.Linq;

public class LevelData
{
    private List<WordWithRange> _words;
    private int _rows;
    private int _columns;
    private int _poolCapacity;
    private bool[,] _activeCells;
    public List<WordWithRange> Words => _words; 
    public int Rows => _rows;
    public int Columns => _columns;
    public int PoolCapacity => _poolCapacity;
    public bool[,] ActiveCells => _activeCells;
    
    public LevelData(List<WordWithRange> words, int rows, int columns, int poolCapacity, bool[,] activeCells)
    {
        if (activeCells.GetLength(0) != rows || activeCells.GetLength(1) != columns)
            throw new ArgumentException("Размеры activeCells должны соответствовать rows и columns");
        
        _rows = rows;
        _columns = columns;
        _words = words;
        _poolCapacity = poolCapacity;
        _activeCells = activeCells;
    }
    
    public static void CalculateOptimalRowRangesOptimized(List<WordWithRange> words, int rows, int cols)
{
    if (words == null || words.Count == 0 || rows <= 0 || cols <= 0)
        return;

    // Текущий массив подсчёта занятых ячеек по каждому ряду (инициализирован нулями)
    int[] occupiedCellsInRow = new int[rows];

    // Временный список слов для распределения в исходном порядке
    List<WordWithRange> wordOrder = new List<WordWithRange>(words);

    foreach (var w in wordOrder)
    {
        int wordLength = w.word.Length;
        // Минимальная длина диапазона в рядах - 2, если слово длинее 1, иначе 1
        int minRowSpan = Math.Max(2, Math.Min(wordLength, rows)); // не больше строк в поле

        int assignedRowMin = 1;
        int assignedRowMax = assignedRowMin + minRowSpan - 1;
        if (assignedRowMax > rows) assignedRowMax = rows;

        // Пытаемся сдвигать диапазон вниз, пока не найдём место для слова в доступных рядах
        // Максимально можем сдвинуть до rows - minRowSpan + 1
        bool foundPlace = false;
        while (assignedRowMax <= rows)
        {
            // Считаем свободные ячейки во всех рядах диапазона assignedRowMin..assignedRowMax
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
            // Не нашли место с минимальным диапазоном, пытаемся увеличить диапазон, пока не дойдём до rows
            assignedRowMin = 1; // сброс к началу
            assignedRowMax = assignedRowMin + minRowSpan - 1;
            if (assignedRowMax > rows) assignedRowMax = rows;

            while (assignedRowMax < rows)
            {
                assignedRowMax++;
                // Проверяем теперь с расширенным диапазоном
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
                // Если и это не помогло - ставим максимальный диапазон по всем строкам
                assignedRowMin = 1;
                assignedRowMax = rows;
            }
        }

        // Назначаем диапазон слову
        w.rowMin = assignedRowMin;
        w.rowMax = assignedRowMax;

        // Обновляем количество занятых ячеек по строкам диапазона
        // Распределяем длину слова как равномерно по всем рядам диапазона (без детализации на буквы)
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
            // Но костыль - если вдруг заполнено больше, чем cols, уменьшим до cols:
            if (occupiedCellsInRow[r] > cols)
                occupiedCellsInRow[r] = cols;
        }
    }
}
}
