using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardEditorView : BoardSpawner<EditorBoardCellView>
{
    public List<EditorBoardCellView> Cells => _cells;

    private void Start()
    {
        ClearBoard();
    }

    public void FillBoard(LevelData levelData)
    {
        var boardGenerator = new BoardDataGenerator();
        var data = boardGenerator.Generate(levelData);

        for (int i = 0; i < levelData.Rows; i++)
        {
            for (int j = 0; j < levelData.Columns; j++)
            {
                if (data[i, j] != null)
                {
                    var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                    if (cell != null)
                        cell.SetData(data[i, j]);
                }
            }
        }
    }
    
    public void BuildBoard(LevelAsset levelAsset)
    {
        ClearBoard();
        BuildEmptyBoard(levelAsset.rows,levelAsset.columns);

        for (int i = 0; i < levelAsset.rows; i++)
        {
            for (int j = 0; j < levelAsset.columns; j++)
            {
                var cell = _cells.FirstOrDefault(x => x.positionData.position == new Vector2Int(i, j));
                var entity = levelAsset.board.FirstOrDefault(x => x.position == new Vector2Int(i, j));

                if (entity == null)
                {
                    _cells.Remove(cell);
                    DestroyImmediate(cell.gameObject);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.character))
                    {
                        cell.SetData(entity);
                    }
                }
            }
        }
    }
    
    public int GetCellsCount()
    {
        int result = 0;
        
        foreach (var cell in _cells)
        {
            if (cell != null)
            {
                result++;
            }
        }
        
        return result;
    }
    
    public void EmptyBoard()
    {
        foreach (var cell in _cells)
        {
            if (cell == null) continue;
            var unit = cell.GetComponentInChildren<EditorLetterUnit>();

            if (unit != null)
            {
                DestroyImmediate(unit.gameObject);
            }
        }
    }
    
    public void ClearBoard()
    {
        while (parentTransform.childCount > 0)
        {
            foreach (Transform child in parentTransform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}