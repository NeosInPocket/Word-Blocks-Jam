using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardEditorView : BoardSpawner<EditorBoardCellView>
{
    [SerializeField] private EditorGhostBoardCellView ghostPrefab;
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
                    var ghostCell = Instantiate(ghostPrefab, cell.transform.position, prefab.transform.rotation, cell.transform.parent);
                    ghostCell.Initialize(cell.SideSize, cell.positionData.position);
                    
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
        if (_cells == null) return 0;
        
        var count = _cells.Count(x => x != null);
        return count;
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