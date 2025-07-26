using System.Collections.Generic;

public class LevelData
{
    private List<string> _words;
    private int _rows;
    private int _columns;
    public List<string> Words => _words; 
    public int Rows => _rows;
    public int Columns => _columns;
    
    public LevelData(List<string> words, int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
        _words = words;
    }
}
