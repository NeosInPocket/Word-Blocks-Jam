using UnityEngine;
using UnityEngine.AI;

public class EnvironmentBlock : MonoBehaviour
{
    public PositionData positionData;
    [SerializeField] private BoxCollider blockCollider;
    private NavMeshObstacle _obstacle;
    public BoxCollider BlockCollider => blockCollider;
    
    public void AddObstacleComponent()
    {
        _obstacle = gameObject.AddComponent<NavMeshObstacle>();
        var size = _obstacle.size;
        size.x *= 1.24f;
        size.z *= 1.24f;
        _obstacle.size = Vector3.one * 0.5f;//0.00002f;
        _obstacle.carving = true;
    }
}