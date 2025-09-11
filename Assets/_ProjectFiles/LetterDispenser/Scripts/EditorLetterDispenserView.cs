#if UNITY_EDITOR
using System;
using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEngine;

public class EditorLetterDispenserView : BoardSpawner<EditorBoardCellView>
{
    [SerializeField] private EditorPipeView pipe;
    private LetterDispenserData _data;
    
    public PositionData positionData;
    public EditorPipeView Pipe => pipe;

    public LetterDispenserData Data => CollectData();
    
    public void FillDispenser(LetterDispenserData data)
    {
        if (data.Board.Count == 0) return;
        
        for (int i = 0; i < data.Board.Count; i++)
        {
            _cells[i].SetData(data.Board[i]);
        }
        
        SetPosition(positionData.position, pipe.transform.position);
    }

    private LetterDispenserData CollectData()
    {
        var data = new LetterDispenserData();
        
        foreach (var cell in _cells)
        {
            var boardData = cell.Data;

            if (boardData == null)
                throw new InvalidOperationException("Все ячейки трубы должны быть заполнены!");
            
            boardData.position = cell.positionData.position;
            
            data.Board.Add(boardData);
        }

        data.cellCount = data.Board.Count;
        return data;
    }

    public void SetData(LetterDispenserData data)
    {
        _data = data;
    }
    
    public void SetPosition(Vector2Int position, Vector3 worldPosition)
    {
        pipe.transform.position = worldPosition;
        positionData.position = position;

        if (_cells is { Count: > 0 })
        {
            foreach (var cell in _cells)
            {
                cell.positionData.position = position;
            }
        }
    }
    
    public void SetColor(Color color)
    {
        SetPipeColor(color);
        
        foreach (var cell in _cells)
        {
            cell.SetColor(color);
        }
    }

    public void ClearBoard()
    {
        foreach (Transform cell in parentTransform)
        {
            DestroyImmediate(cell.gameObject);
        }
    }

    private void SetPipeColor(Color color)
    {
        var renderer = pipe.GetComponent<Renderer>();
        var mpb = new MaterialPropertyBlock();
        
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);

        renderer.SetPropertyBlock(mpb);
    }
}
#endif