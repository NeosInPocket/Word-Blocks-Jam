using System.Collections.Generic;

[System.Serializable]
public class WordLevel
{
    public int levelId;
    public List<WordWithRange> words = new();

    public WordLevel(int levelId, List<WordWithRange> words)
    {
        this.levelId = levelId;
        this.words = words;
    }
}

[System.Serializable]
public class WordWithRange
{
    public string word;
    public int rowMin;
    public int rowMax;
    public int minRowsNeeded;

    public WordWithRange(string word, int rowMin, int rowMax)
    {
        this.word = word;
        this.rowMin = rowMin;
        this.rowMax = rowMax;
    }
}
