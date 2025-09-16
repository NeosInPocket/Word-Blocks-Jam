using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDecorationsSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private float ySpawnPosition = 0f;
    [SerializeField] private int maxSpawnAttempts = 500;

    [Header("Distance Constraints")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 5f;

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

            GameObject temp = Instantiate(prefab, position, Quaternion.identity, transform);

            // Проверка расстояний между объектами
            if (IsOverlapping(position))
            {
                Destroy(temp);
                continue;
            }

            _decorations.Add(temp);
        }
    }

    private bool IsOverlapping(Vector3 testPosition)
    {
        foreach (var decoration in _decorations)
        {
            if (decoration == null) continue;

            float distance = Vector3.Distance(testPosition, decoration.transform.position);

            if (distance < minDistance || distance > maxDistance)
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
