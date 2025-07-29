using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BoardEntityData
{
    public string character;
    public Vector2Int position;
    
    public BoardEntityData(string character, Vector2Int position)
    {
        this.character = character;
        this.position = position;
    }
    
    public static BoardEntityData[,] To2DArray(List<BoardEntityData> flatList)
    {
        if (flatList == null || flatList.Count == 0)
            throw new ArgumentException("Input list is null or empty.");

        int maxRow = flatList.Max(data => data.position.x);
        int maxCol = flatList.Max(data => data.position.y);

        BoardEntityData[,] result = new BoardEntityData[maxRow + 1, maxCol + 1];

        foreach (var data in flatList)
        {
            Vector2Int pos = data.position;
            result[pos.x, pos.y] = data;
        }

        return result;
    }
}
