using System.Linq;
using UnityEngine;

public class EditorLetterDispenserFactory : MonoBehaviour
{
    [SerializeField] private EditorLetterDispenserView prefab;

    public EditorLetterDispenserView Create()
    {
        var allCells = FindObjectsByType<EditorBoardCellView>(FindObjectsSortMode.None).ToList();
        var leftCellXPosition = allCells.Min(x => x.transform.position.x);
        var leftCellZPosition = allCells[0].transform.parent.position.z;
        
        Vector3 dispenserSpawnPosition = new Vector3(leftCellXPosition - allCells[0].SideSize * 1.5f, prefab.transform.position.y, leftCellZPosition);
        
        var dispenser = Instantiate(prefab, dispenserSpawnPosition, prefab.transform.rotation, transform);
        
        return dispenser;
    }

    public void Clear()
    {
        foreach (Transform dispenserView in transform)
        {
            DestroyImmediate(dispenserView.gameObject);
        }
    }
}