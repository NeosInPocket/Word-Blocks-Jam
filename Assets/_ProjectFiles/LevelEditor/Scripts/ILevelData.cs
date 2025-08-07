using System.Collections.Generic;

public interface ILevelData
{
    int LevelId { get; }
    List<WordWithRange> Words { get; }
}