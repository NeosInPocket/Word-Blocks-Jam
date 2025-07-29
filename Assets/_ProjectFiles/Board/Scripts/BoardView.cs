using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardCell prefab;
    [SerializeField] private BoxColliderVisualizer boardBoundsCollider;
    [Range(0, 1f)]
    [SerializeField] private float spacingPerCellSize;
    [SerializeField] private Transform ySpawnTransform;
    [SerializeField] private Transform parentTransform;
    private BoardCell[,] _cells;
    public BoardCell[,] Cells => _cells;
    public GameObject LettersCellsParent => parentTransform.gameObject;

    public void BuildBoard(BoardData boardData)
    {
        var isBoxColliderActive = boardBoundsCollider.isActiveAndEnabled;
        boardBoundsCollider.gameObject.SetActive(true);
        
        var board = boardData.Board;
        int cellsPerRow = board.GetLength(0);
        var boardBounds = boardBoundsCollider.Collider.bounds;
        
        var minX = boardBounds.min.x;
        var maxX = boardBounds.max.x;
        var minZ = boardBounds.min.z;

        float xBoardSize = maxX - minX;
        float cellSideSize = xBoardSize / (cellsPerRow + spacingPerCellSize * (cellsPerRow - 1));
        float spacing = spacingPerCellSize * cellSideSize;
        float startX = minX + cellSideSize / 2;
        Vector3 spawnPosition = new Vector3(startX, ySpawnTransform.position.y, minZ + cellSideSize / 2);

        _cells = new BoardCell[board.GetLength(0), board.GetLength(1)];
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var cell = Instantiate(prefab, spawnPosition, prefab.transform.rotation, parentTransform);
                cell.positionData.position = new Vector2Int(i, j);
                
                if (board[i, j] != null)
                    cell.SetData(board[i, j]);
                
                cell.SideSize = cellSideSize;
                spawnPosition.x += spacing + cellSideSize;
                _cells[i, j] = cell;
            }
            
            spawnPosition.x = startX;
            spawnPosition.z += spacing + cellSideSize;
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
