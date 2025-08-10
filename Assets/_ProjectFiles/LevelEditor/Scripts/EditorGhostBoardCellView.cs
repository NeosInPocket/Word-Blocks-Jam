using UnityEngine;

public class EditorGhostBoardCellView : MonoBehaviour
{
    public PositionData positionData;
    [SerializeField] protected EditorBoardCellView prefab;
    private Renderer _renderer;
    
    public float SideSize
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();   
            }

            return _renderer.bounds.size.x;
        }

        set => transform.localScale *= value / SideSize;
    }

    public void Initialize(float sideSize, Vector2Int position)
    {
        SideSize = sideSize;
        positionData.position = position;
    }
    
    public void ReplaceWithBoardCell()
    {
        var cell = Instantiate(prefab, transform.position, prefab.transform.rotation, transform.parent);
        cell.SetData(new BoardEntityData(null, positionData.position, true,  Color.white,false));
    }
}