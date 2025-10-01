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
        _obstacle.size = Vector3.one * 0.1f;
        _obstacle.carving = true;
    }
}