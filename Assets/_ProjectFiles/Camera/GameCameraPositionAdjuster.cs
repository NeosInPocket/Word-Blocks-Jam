using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCameraPositionAdjuster : MonoBehaviour
{
    [SerializeField] public Camera targetCamera;
    [SerializeField] public RectTransform menuMask;
    [SerializeField] public RectTransform gameMask;
    [SerializeField] public Transform bottomObject;
    private Vector3 _preferredPosition;
    private float _preferredSize;

    public void ApplyPreferredCameraParameters()
    {
        targetCamera.transform.position = _preferredPosition;
        targetCamera.orthographicSize = _preferredSize;
    }

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

        Vector3 maskCenter = (worldVectors[0] + worldVectors[2]) * 0.5f;
        Vector3 toMaskTarget = maskCenter - targetCamera.transform.position;
        
        Vector3 objectsCenter = GetCenterOfObjects(objectsToFit);
        
        Vector3 toTarget = objectsCenter - targetCamera.transform.position;
        Vector3 forward = targetCamera.transform.forward;
        
        Vector3 projection = Vector3.ProjectOnPlane(toTarget, forward);
        Vector3 maskCenterProjection = Vector3.ProjectOnPlane(toMaskTarget, forward);

        _preferredPosition = targetCamera.transform.position + projection - maskCenterProjection;

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

        float ratio = 0;
        
        if (height > width)
        {
            
        }
        else
        {
            
        }
        
        ratio = maskHeight / height;
        
        //ratio = maskWidth / width;
        
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
        
        // Vector3 center = Vector3.zero;
        // foreach (var pos in objects)
        // {
        //     center += pos;
        // }
        // center /= objects.Count;
        return center;
    }
}