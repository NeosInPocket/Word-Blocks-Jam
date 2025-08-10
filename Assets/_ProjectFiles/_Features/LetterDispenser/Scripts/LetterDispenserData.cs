using System.Collections.Generic;

namespace _ProjectFiles._Features.LetterDispenser.Scripts
{
    public class LetterDispenserData
    {
        public int CellCount
        {
            get => _cellCount;
            set
            {
                _datas.Clear();
                _cellCount = value;
            }
        }
        public bool IsFull => _datas.Count == CellCount;
        public List<BoardEntityData> Data => _datas;
        
        private int _cellCount;
        private List<BoardEntityData> _datas = new();
        

        public bool TryAdd(BoardEntityData data)
        {
            if (_datas.Count >= CellCount) return false;
            
            _datas.Add(data);
            return true;
        }
    }
}