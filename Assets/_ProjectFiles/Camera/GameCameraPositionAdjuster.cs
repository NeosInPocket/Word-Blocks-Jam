using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameCameraPositionAdjuster : MonoBehaviour
{
    [SerializeField] public Camera targetCamera;
    [SerializeField] public RectTransform menuMask;
    [SerializeField] public RectTransform gameMask;
    [SerializeField] public Transform bottomObject;
    private Vector3 _preferredPosition;
    private float _preferredSize;

    public (Vector3 position, float size) CalculateMenuCameraParameters(List<Transform> objectsToFit, bool apply = false)
    {
        var positions = objectsToFit.Select(x => x.position).ToList();
        
        return CalculateCameraParameters(positions, menuMask, apply);
    }

    public (Vector3 position, float size) CalculateGameCameraParameters(List<Transform> objectsToFit, bool apply = false)
    {
        var positions = objectsToFit.Select(x => x.position).ToList();
        positions.Add(bottomObject.position);
        
        return CalculateCameraParameters(positions, gameMask, apply);
    }
    
    private (Vector3 position, float size) CalculateCameraParameters(List<Vector3> objectsToFit, RectTransform mask, bool apply)
    {
        Vector3[] v = new Vector3[4];
        mask.GetWorldCorners(v);

        List<Vector3> worldVectors = new();
        
        for (var i = 0; i < 4; i++)
        {
            var normalized = new Vector3(v[i].x / Screen.width, v[i].y / Screen.height, 0);
            var world = targetCamera.ViewportToWorldPoint(normalized);
            worldVectors.Add(world);
        }
        
        Vector3 objectsCenter = GetCenterOfObjects(objectsToFit);
        Vector3 maskScreen = mask.position;

        Vector3 objectScreen = targetCamera.WorldToScreenPoint(objectsCenter);
        Vector2 delta = maskScreen - objectScreen;

        float worldPerPixel = (targetCamera.orthographicSize * 2f) / Screen.height;

        Vector3 move =
            targetCamera.transform.right * (delta.x * worldPerPixel) +
            targetCamera.transform.up    * (delta.y * worldPerPixel);

        _preferredPosition = targetCamera.transform.position - move;
        
        List<Vector3> screenPositions = objectsToFit.Select(x => targetCamera.WorldToScreenPoint(x)).ToList();
        
        float xMin = screenPositions.Min(x => x.x);
        float xMax = screenPositions.Max(x => x.x);
        float yMin = screenPositions.Min(x => x.y);
        float yMax = screenPositions.Max(x => x.y);
        
        float width = xMax - xMin;
        float height = yMax - yMin;
        
        float worldRatio = width / height;
        float maskWidth = Vector3.Distance(v[0], v[3]);
        
        float maskHeight = Vector3.Distance(v[0], v[1]);
        
        float ratio = maskHeight / height;
        
        _preferredSize = targetCamera.orthographicSize / ratio;
        
        if (apply)
        {
            targetCamera.orthographicSize = _preferredSize;
            targetCamera.transform.position = _preferredPosition;
        }
        
        return (_preferredPosition, _preferredSize);
    }

    private Vector3 GetCenterOfObjects(List<Vector3> objects)
    {
        var maxX = objects.Max(x => x.x);
        var minX = objects.Min(x => x.x);
        
        var maxZ = objects.Max(x => x.z);
        var minZ = objects.Min(x => x.z);
        
        Vector3 center = new Vector3((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
        
        return center;
    }
}