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
    [SerializeField] private BoardView boardView;
    [SerializeField] private float ySpawnDelta;
    [SerializeField] private float blockSizePerCellSize = 0.7f;
    
    private Dictionary<BlockType, GameObject> prefabs;

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

    public void Spawn(List<BoardEntityData> data)
    {
        var positions = data.Where(x => string.IsNullOrEmpty(x.character) && !x.isOccupiedByDispenser && !x.isGhostCell).Select(x => x.position).ToList();
        PlaceBlocks(positions);
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
            var renderer = block.GetComponent<Renderer>();

            block.transform.localScale = 1f * Vector3.one;
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
