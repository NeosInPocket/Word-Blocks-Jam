using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDecorationsSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private float ySpawnPosition = 0f;
    [SerializeField] private int maxSpawnAttempts = 500;

    private readonly List<GameObject> _decorations = new();

    public void Spawn(Vector2 minMaxX, Vector2 minMaxZ)
    {
        RemoveDecorations();

        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("Нет префабов для спавна!");
            return;
        }

        int attempts = 0;

        while (attempts < maxSpawnAttempts)
        {
            attempts++;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            Vector3 position = new Vector3(
                Random.Range(minMaxX.x, minMaxX.y),
                ySpawnPosition,
                Random.Range(minMaxZ.x, minMaxZ.y)
            );

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject temp = Instantiate(prefab, position, rotation, transform);
            Collider tempCollider = temp.GetComponentInChildren<Collider>();

            if (tempCollider == null)
            {
                Debug.LogWarning($"Prefab {prefab.name} не содержит Collider!");
                Destroy(temp);
                continue;
            }

            if (IsOverlapping(tempCollider))
            {
                Destroy(temp);
                continue;
            }

            _decorations.Add(temp);
        }
    }
    
    private bool IsOverlapping(Collider testCollider)
    {
        foreach (var decoration in _decorations)
        {
            if (decoration == null) continue;

            Collider otherCollider = decoration.GetComponentInChildren<Collider>();
            if (otherCollider == null) continue;

            if (testCollider.bounds.Intersects(otherCollider.bounds))
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveDecorations()
    {
        if (_decorations == null || _decorations.Count == 0) return;

        foreach (var decoration in _decorations)
        {
            if (decoration != null)
                Destroy(decoration);
        }

        _decorations.Clear();
    }
}
