using System.Collections.Generic;

public readonly struct WordLevelWrapper : ILevelData
{
    private readonly WordLevel _wordLevel;
    public WordLevelWrapper(WordLevel wordLevel) => _wordLevel = wordLevel;
    public int LevelId => _wordLevel.levelId;
    public List<WordWithRange> Words => _wordLevel.words;
}