using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardEnvironmentObjectsSpawner : MonoBehaviour
{
    [SerializeField] public GameObject corneredPrefab;
    [SerializeField] public GameObject corneredOneSmoothPathPrefab;
    [SerializeField] public GameObject corneredTwoSmoothPathPrefab;
    [SerializeField] public GameObject oneSmoothPrefab;
    [SerializeField] public GameObject twoSmoothPrefab;
    [SerializeField] public GameObject allSmoothPrefab;
    [SerializeField] public int additionalHorizontalBlocksCount;
    [SerializeField] public int additionalVerticalBlocksCount;
    [SerializeField] private BoardView boardView;
    [SerializeField] private float ySpawnDelta;
    [SerializeField] private float blockSizePerCellSize = 2.475f;
    [SerializeField] private EnvironmentDecorationsSpawner decorations;
    [SerializeField] private int decorationsAdditiveCellsCount;
    
    private Dictionary<BlockType, GameObject> prefabs;
    private float _cellSize;
    private List<GameObject> blocks;
    
    private void SetPrefabs() {
        prefabs = new Dictionary<BlockType, GameObject>() {
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
        blocks = new List<GameObject>();
        
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
    }

    private void Clear()
    {
        if (blocks == null || blocks.Count == 0) return;
        
        foreach (var block in blocks)
        {
            Destroy(block);
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
