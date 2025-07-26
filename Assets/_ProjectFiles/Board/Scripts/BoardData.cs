public class BoardData
{
    private BoardEntityData[,] _board;
    public BoardEntityData[,] Board => _board;
    
    public void GenerateBoard(LevelData levelData)
    {
        _board = new BoardGenerator().Generate(levelData);
    }

    public void SetBoard()
    {
        
    }
}
