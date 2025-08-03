using System;
using UnityEngine;

[Serializable]
public class BoardEntityData
{
    public string character;
    public Vector2Int position;
    public bool isVisible = true;
    
    public BoardEntityData(string character, Vector2Int position, bool isVisible)
    {
        this.character = character;
        this.position = position;
        this.isVisible = isVisible;
    }
}
