using System.Collections.Generic;

public class LevelData
{
    private List<WordWithRange> _words;
    private int _rows;
    private int _columns;
    private int _poolCapacity;
    public List<WordWithRange> Words => _words; 
    public int Rows => _rows;
    public int Columns => _columns;
    public int PoolCapacity => _poolCapacity;
    
    public LevelData(List<WordWithRange> words, int rows, int columns, int poolCapacity)
    {
        _rows = rows;
        _columns = columns;
        _words = words;
        _poolCapacity = poolCapacity;
    }
}
