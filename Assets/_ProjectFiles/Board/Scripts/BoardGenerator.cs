using System;
using System.Collections.Generic;

public class BoardGenerator
{
    public BoardEntityData[,] Generate(LevelData levelData)
    {
        int rows = levelData.Rows;
        int cols = levelData.Columns;

        var board = new BoardEntityData[rows, cols];
        var totalCells = rows * cols;

        List<char> letters = new();
        foreach (var word in levelData.Words)
        {
            foreach (char letter in word)
                letters.Add(letter);
        }

        if (letters.Count > totalCells)
            throw new Exception("There is not enough space on the field for all the letters.");

        letters.Shuffle();

        int letterIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (letterIndex < letters.Count)
                {
                    board[row, col] = new BoardEntityData(letters[letterIndex]);
                    letterIndex++;
                }
                else
                {
                    board[row, col] = null;
                }
            }
        }

        return board;
    }

}
