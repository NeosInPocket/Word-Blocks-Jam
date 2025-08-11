using UnityEngine;
using System.Collections.Generic;
using _ProjectFiles._Features.LetterDispenser.Scripts;

[CreateAssetMenu(fileName = "Level_", menuName = "Game/Level Asset")]
public class LevelAsset : ScriptableObject
{
    [SerializeField] private int levelId;
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int poolCount;
    [SerializeField] private List<WordWithRange> words;
    [SerializeField] private List<BoardEntityData> board;
    [SerializeField] private List<LetterDispenserData> dispensers;
    
    public int LevelId => levelId;
    public int Rows => rows;
    public int Columns => columns;
    public int PoolCount => poolCount;
    public List<WordWithRange> Words => words;
    public List<BoardEntityData> Board => board;
    public List<LetterDispenserData> Dispensers => dispensers;
    
    public BoardEntityData[,] boardArray => To2DArray(board, rows, columns);

    public void Initialize(int levelId, int rows, int columns, int poolCount, List<WordWithRange> words, List<BoardEntityData> board, List<LetterDispenserData> dispensers)
    {
        this.levelId = levelId;
        this.rows = rows;
        this.columns = columns;
        this.poolCount = poolCount;
        this.words = words;
        this.board = board;
        this.dispensers = dispensers;
    }
    
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