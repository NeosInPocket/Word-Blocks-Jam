using System.Collections.Generic;

public readonly struct LevelAssetWrapper : ILevelData
{
    private readonly LevelAsset _levelAsset;
    public LevelAssetWrapper(LevelAsset levelAsset) => _levelAsset = levelAsset;
    public int Rows => _levelAsset.Rows;
    public int Columns => _levelAsset.Columns;
    public int PoolCount => _levelAsset.PoolCount;
    public int LevelId => _levelAsset.LevelId;
    public List<WordWithRange> Words => _levelAsset.Words;
}