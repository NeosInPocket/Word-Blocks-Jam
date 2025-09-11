#if UNITY_EDITOR 
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorLetterDispenserFactory : MonoBehaviour
{
    [SerializeField] private EditorLetterDispenserView prefab;

    private void Start()
    {
        Clear();
    }
    
    public EditorLetterDispenserView Create(int poolCount)
    {
        var existingDispensers = FindObjectsByType<EditorLetterDispenserView>(FindObjectsSortMode.None);
        
        var allCells = FindObjectsByType<EditorBoardCellView>(FindObjectsSortMode.None).ToList();
        var leftCellXPosition = allCells.Min(x => x.transform.position.x);
        var leftCellZPosition = allCells[0].transform.parent.position.z;

        if (existingDispensers.Length > 0)
        {
            leftCellXPosition += 2f * allCells[0].SideSize * existingDispensers.Length;
        }
        
        Vector3 dispenserSpawnPosition = new Vector3(leftCellXPosition, prefab.transform.position.y, leftCellZPosition  - allCells[0].SideSize * 1.5f * poolCount);
        
        var dispenser = Instantiate(prefab, dispenserSpawnPosition, prefab.transform.rotation, transform);
        
        return dispenser;
    }

    public void Clear()
    {
        var childs = new List<GameObject>();

        foreach (Transform child in transform)
        {
            childs.Add(child.gameObject);
        }
        
        foreach (var child in childs)
        {
            DestroyImmediate(child);
        }
    }
}
#endif