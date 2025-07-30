using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level_", menuName = "Game/Level Asset")]
public class LevelAsset : ScriptableObject
{
    public int levelId;
    public int rows;
    public int columns;
    public int poolCount;
    public List<WordWithRange> words;
    public List<BoardEntityData> board;

    public BoardEntityData[,] boardArray => To2DArray(board, rows, columns);
    
    public BoardEntityData[,] To2DArray(List<BoardEntityData> list, int rows, int columns)
    {
        BoardEntityData[,] result = new BoardEntityData[rows, columns];

        foreach (var data in list)
        {
            Vector2Int pos = data.position;
            if (pos.x >= 0 && pos.x < rows && pos.y >= 0 && pos.y < columns)
            {
                result[pos.x, pos.y] = data;
            }
            else
            {
                Debug.LogWarning($"BoardEntityData with invalid position: {pos}");
            }
        }

        return result;
    }
}