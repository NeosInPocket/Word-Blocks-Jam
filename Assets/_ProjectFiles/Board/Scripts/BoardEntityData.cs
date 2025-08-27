using System;
using UnityEngine;

[Serializable]
public class BoardEntityData
{
    public string character;
    public Vector2Int position;
    public bool isVisible = true;
    public Color color = Color.black;
    public bool isOccupiedByDispenser;
    public bool isGhostCell;
    
    public BoardEntityData(string character, Vector2Int position, bool isVisible, Color color, bool isOccupiedByDispenser, bool isGhostCell = false)
    {
        this.character = character;
        this.position = position;
        this.isVisible = isVisible;
        this.color = color;
        this.isOccupiedByDispenser = isOccupiedByDispenser;
        this.isGhostCell = isGhostCell;
    }
}
