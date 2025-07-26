using System.Collections.Generic;

public class BoardValidator
{
    public bool Validate(LevelData level, BoardEntityData[,] board)
    {
        List<char> allLetters = new();

        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var entity = board[r, c];
                if (entity != null)
                    allLetters.Add(entity.character);
            }
        }

        foreach (string word in level.Words)
        {
            var copy = new List<char>(allLetters);
            foreach (char ch in word)
            {
                if (!copy.Remove(ch))
                    return false;
            }
        }

        return true;
    }
}