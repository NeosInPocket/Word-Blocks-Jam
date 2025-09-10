using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BoardEnvironmentObjectsSpawner : MonoBehaviour
{
    [SerializeField] public EnvironmentBlock corneredPrefab;
    [SerializeField] public EnvironmentBlock corneredOneSmoothPathPrefab;
    [SerializeField] public EnvironmentBlock corneredTwoSmoothPathPrefab;
    [SerializeField] public EnvironmentBlock oneSmoothPrefab;
    [SerializeField] public EnvironmentBlock twoSmoothPrefab;
    [SerializeField] public EnvironmentBlock allSmoothPrefab;
    [SerializeField] public int additionalHorizontalBlocksCount;
    [SerializeField] public int additionalVerticalBlocksCount;
    [SerializeField] private BoardView boardView;
    [SerializeField] private float ySpawnDelta;
    [SerializeField] private float blockSizePerCellSize = 2.475f;
    [SerializeField] private EnvironmentDecorationsSpawner decorations;
    [SerializeField] private int decorationsAdditiveCellsCount;
    
    private Dictionary<BlockType, EnvironmentBlock> prefabs;
    private float _cellSize;
    private List<EnvironmentBlock> blocks;
    
    private void SetPrefabs() {
        prefabs = new Dictionary<BlockType, EnvironmentBlock>() {
            { BlockType.Cornered, corneredPrefab },
            { BlockType.CorneredWithOneSmoothPath, corneredOneSmoothPathPrefab },
            { BlockType.CorneredWithTwoSmoothPath, corneredTwoSmoothPathPrefab },
            { BlockType.OneSmoothCorner, oneSmoothPrefab },
            { BlockType.TwoSmoothCorners, twoSmoothPrefab },
            { BlockType.AllSmoothCorners, allSmoothPrefab }
        };
    }

    public void Spawn(List<BoardEntityData> data, float cellSize)
    {
        Clear();
        blocks = new List<EnvironmentBlock>();
        
        _cellSize = cellSize;
        int maxRowIndex = data.Max(x => x.position.x) + additionalVerticalBlocksCount; 
        int minRowIndex = data.Min(x => x.position.x); 
        int maxColumnIndex = data.Max(x => x.position.y) + additionalHorizontalBlocksCount;
        int minColumnIndex = data.Min(x => x.position.y) - additionalHorizontalBlocksCount;

        List<Vector2Int> positions = new List<Vector2Int>();
        
        for (int i = minRowIndex; i <= maxRowIndex; i++)
            for (int j = minColumnIndex; j <= maxColumnIndex; j++)
                positions.Add(new Vector2Int(i, j));

        var dataPositions = data.Where(x => !string.IsNullOrEmpty(x.character) || x.isOccupiedByDispenser).Select(x => x.position).ToList();
        positions.RemoveAll(x => dataPositions.Contains(x));
        
        PlaceBlocks(positions);

        Vector2Int startBlockPositionData = new Vector2Int(data.Max(x => x.position.x) + 1, minColumnIndex);
        Vector2Int lastBlockPositionData = new Vector2Int(data.Max(x => x.position.x) + decorationsAdditiveCellsCount, maxColumnIndex);
        
        Vector3 startBlockPosition = boardView.GetPositionFromPositionData(startBlockPositionData);
        Vector3 lastBlockPosition = boardView.GetPositionFromPositionData(lastBlockPositionData);
        Vector2 minMaxX = new Vector2(startBlockPosition.x, lastBlockPosition.x);
        Vector2 minMaxZ = new Vector2(startBlockPosition.z, lastBlockPosition.z);
        decorations.Spawn(minMaxX, minMaxZ);
        
        SetObstaclesToOuterDecorations();
    }

    private void SetObstaclesToOuterDecorations()
    {
        var outerBlocks = GetOuterObstacles();
        foreach (var block in outerBlocks)
        {
            block.AddObstacleComponent();
        }
    }
    
    private List<EnvironmentBlock> GetOuterObstacles()
    {
        HashSet<Vector2Int> filledPositions = new HashSet<Vector2Int>(
            blocks.Select(d => d.positionData.position)
        );

        List<EnvironmentBlock> outerCells = new List<EnvironmentBlock>();

        foreach (var block in blocks)
        {
            int neighbors = 0;
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = block.positionData.position + dir;
                if (filledPositions.Contains(neighborPos))
                {
                    neighbors++;
                }
            }

            if (neighbors < 4)
            {
                outerCells.Add(block);
            }
        }

        var maxRowIndex = outerCells.Max(x => x.positionData.position.x);
        outerCells.RemoveAll(x => x.positionData.position.x == maxRowIndex);
        
        return outerCells;
    }
    
    private void Clear()
    {
        if (blocks == null || blocks.Count == 0) return;
        
        foreach (var block in blocks)
        {
            Destroy(block.gameObject);
        }
    }
    
    private void PlaceBlocks(List<Vector2Int> positions)
    {
        if (prefabs == null) SetPrefabs();
        
        HashSet<Vector2Int> occupied = positions.ToHashSet();

        foreach (var pos in positions) {
            var type = GetBlockType(pos, occupied, out Quaternion rotation);
            var prefab = prefabs[type];
            var position = boardView.GetPositionFromPositionData(pos);
            position.y += ySpawnDelta;
            
            var block = Instantiate(prefab, position, rotation, transform);

            block.positionData.position = pos;
            block.transform.localScale = blockSizePerCellSize * _cellSize * Vector3.one;
            blocks.Add(block);
        }
    }

    private BlockType GetBlockType(Vector2Int pos, HashSet<Vector2Int> occupied, out Quaternion rotation)
    {
        bool right = occupied.Contains(pos + new Vector2Int(0, 1));
        bool left  = occupied.Contains(pos + new Vector2Int(0, -1));
        bool up    = occupied.Contains(pos + new Vector2Int(-1, 0));
        bool down  = occupied.Contains(pos + new Vector2Int(1, 0));

        int neighborCount = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        rotation = Quaternion.identity;
        var blockType = BlockType.Cornered;
        
        switch (neighborCount)
        {
            case 0:
                blockType = BlockType.AllSmoothCorners;
                break;

            case 1:
                rotation = GetOneSmoothRotation(up, down, left, right);
                blockType = BlockType.TwoSmoothCorners;
                break;
            
            case 2:
                if ((up && down) || (left && right))
                {
                    blockType = BlockType.CorneredWithTwoSmoothPath;
                    rotation = GetTwoSmoothPathRotation(up, down, left, right);
                }
                else
                {
                    rotation = GetTwoSmoothRotation(up, down, left, right);
                    blockType = BlockType.OneSmoothCorner;
                }
                break;

            case 3: 
                blockType = BlockType.CorneredWithOneSmoothPath;
                rotation = GetOneSmoothPathRotation(up, down, left, right);
                break;
        }

        return blockType;
    }

    private Quaternion GetOneSmoothPathRotation(bool up, bool down, bool left, bool right)
    {
        bool upFree = !up;
        bool downFree = !down;
        bool leftFree = !left;
        bool rightFree = !right;
        
        if (upFree) return Quaternion.Euler(0, -90, 0);
        if (downFree) return Quaternion.Euler(0, 90, 0);
        if (leftFree) return Quaternion.Euler(0, 0, 0);
        if (rightFree) return Quaternion.Euler(0, 180, 0);
        
        throw new ArgumentException("No rotation found");
    }
    
    private Quaternion GetTwoSmoothPathRotation(bool up, bool down, bool left, bool right)
    {
        bool upFree = !up;
        bool rightFree = !right;

        if (upFree) return Quaternion.Euler(0, -90, 0);
        if (rightFree) return Quaternion.Euler(0, 0, 0);
        
        throw new ArgumentException("No rotation found");
    }
    
    private Quaternion GetOneSmoothRotation(bool up, bool down, bool left, bool right)
    {
        if (up) return Quaternion.Euler(0, 90, 0);
        if (right) return Quaternion.Euler(0, 0, 0);
        if (down) return Quaternion.Euler(0, -90, 0);
        if (left) return Quaternion.Euler(0, 180, 0);
        
        throw new ArgumentException("No rotation found");
    }

    private Quaternion GetTwoSmoothRotation(bool up, bool down, bool left, bool right)
    {
        bool upFree = !up;
        bool downFree = !down;
        bool leftFree = !right;
        bool rightFree = !left;

        if (upFree && rightFree) return Quaternion.Euler(0, -90, 0);
        if (upFree && leftFree) return Quaternion.Euler(0, 180, 0);
        if (downFree && rightFree) return Quaternion.Euler(0, 0, 0);
        if (downFree && leftFree) return Quaternion.Euler(0, 90, 0);

        throw new ArgumentException("No rotation found");
    }
}
