using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectFiles._Features.LetterDispenser.Scripts
{
    [Serializable]
    public class LetterDispenserData
    {
        public int CellCount
        {
            get => cellCount;
            set
            {
                Board.Clear();
                cellCount = value;
            }
        }
        public bool IsFull => Board.Count == CellCount;
        
        public int cellCount;
        public List<BoardEntityData> Board = new();

        public bool TryAdd(BoardEntityData data)
        {
            if (Board.Count >= CellCount) return false;
            
            Board.Add(data);
            return true;
        }

        public List<BoardEntityData> CopyBoard()
        {
            var result = new List<BoardEntityData>();
            
            foreach (var data in Board)
            {
                var newData = new BoardEntityData(data.character, data.position, data.isVisible, data.color,
                    data.isOccupiedByDispenser);
                
                result.Add(newData);
            }

            return result;
        }
    }
}