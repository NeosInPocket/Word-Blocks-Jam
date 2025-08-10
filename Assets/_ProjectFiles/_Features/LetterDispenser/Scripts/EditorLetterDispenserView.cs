using _ProjectFiles._Features.LetterDispenser.Scripts;
using UnityEngine;

public class EditorLetterDispenserView : BoardSpawner<EditorBoardCellView>
{
    [SerializeField] private Transform model;
    
    public PositionData positionData;
    private LetterDispenserData _data;

    public void FillDispenser(LetterDispenserData data)
    {
        if (data.Data.Count == 0) return;
        
        for (int i = 0; i < data.CellCount; i++)
        {
            data.Data[i].position = positionData.position;
            _cells[i].SetData(data.Data[i]);
        }
    }

    public void SetPosition(Vector2Int position, Vector3 worldPosition)
    {
        model.transform.position = worldPosition;
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

    public void Clear()
    {
        foreach (Transform cell in parentTransform)
        {
            DestroyImmediate(cell.gameObject);
        }
    }

    private void SetPipeColor(Color color)
    {
        var renderer = model.GetComponent<Renderer>();
        var mpb = new MaterialPropertyBlock();
        
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);

        renderer.SetPropertyBlock(mpb);
    }
}
