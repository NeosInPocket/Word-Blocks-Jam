using System.Collections.Generic;

public readonly struct LevelAssetWrapper : ILevelData
{
    private readonly LevelAsset _levelAsset;
    public LevelAssetWrapper(LevelAsset levelAsset) => _levelAsset = levelAsset;
    public int LevelId => _levelAsset.levelId;
    public List<WordWithRange> Words => _levelAsset.words;
}