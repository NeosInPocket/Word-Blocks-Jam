using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectFiles._Features.LetterDispenser.Scripts
{
    public class LetterDispenserView : MonoBehaviour
    {
        private Queue<BoardEntityData> _queue;
        public Vector2Int Position => _position;
        private Vector2Int _position;
        
        public void SetData(LetterDispenserData data)
        {
            _queue = new Queue<BoardEntityData>(data.Board);
        }

        public void SetPosition(Vector2Int position, Vector3 worldPosition)
        {
            _position = position;
            transform.position = worldPosition;
        }
        
        public bool TryGetNext(out BoardEntityData data)
        {
            try
            {
                data = _queue.Dequeue();
                return true;
            }
            catch (Exception ex)
            {
                data = null;
                return false;
            }  
        }
    }
}