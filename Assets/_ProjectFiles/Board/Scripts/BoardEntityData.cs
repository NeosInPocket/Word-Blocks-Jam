using System;
using UnityEngine;

[Serializable]
public class BoardEntityData
{
    public string character;
    public Vector2Int position;
    public bool isVisible = true;
    public Color color;
    public bool isOccupiedByDispenser;
    
    public BoardEntityData(string character, Vector2Int position, bool isVisible, Color color, bool isOccupiedByDispenser)
    {
        this.character = character;
        this.position = position;
        this.isVisible = isVisible;
        this.color = color;
        this.isOccupiedByDispenser = isOccupiedByDispenser;
    }
}
