using System.Collections.Generic;

[System.Serializable]
public class WordLevel
{
    public int levelId;
    public List<WordWithRange> words = new();
}

[System.Serializable]
public class WordWithRange
{
    public string word;
    public int rowMin;
    public int rowMax;

    public WordWithRange(string word, int rowMin, int rowMax)
    {
        this.word = word;
        this.rowMin = rowMin;
        this.rowMax = rowMax;
    }
}
