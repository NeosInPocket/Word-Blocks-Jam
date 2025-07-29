using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level_", menuName = "Game/Level Asset")]
public class LevelAsset : ScriptableObject
{
    public int levelId;
    public int rows;
    public int columns;
    public int poolCount;
    public List<WordWithRange> words;
    public List<BoardEntityData> board;
}