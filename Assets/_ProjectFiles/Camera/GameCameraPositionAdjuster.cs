using System.Collections.Generic;
using UnityEngine;

public class GameCameraPositionAdjuster : MonoBehaviour
{
    public Camera targetCamera;
    public RectTransform maskRect;
    
    public void FitCamera(List<Transform> objectsToFit)
    {
        Vector3[] v = new Vector3[4];
        maskRect.GetWorldCorners(v);
        List<Vector3> worldVectors = new();
        List<Vector3> normalizedVectors = new();
        
        for (var i = 0; i < 4; i++)
        {
            var normalized = new Vector3(v[i].x / Screen.width, v[i].y / Screen.height, 0);
            var world = targetCamera.ViewportToWorldPoint(normalized);
            worldVectors.Add(world);
            normalizedVectors.Add(normalized);
        }

        var normalizedMaskCenter = normalizedVectors[0] + 
        
        Vector3 maskCenter = (worldVectors[0] + worldVectors[2]) * 0.5f;
        
        Vector3 objectsCenter = GetCenterOfObjects(objectsToFit);
        
        Vector3 toTarget = objectsCenter - targetCamera.transform.position;
        Vector3 forward = targetCamera.transform.forward.normalized;
        
        Vector3 projection = Vector3.ProjectOnPlane(toTarget, forward);

        targetCamera.transform.position += projection;
    }
    
    private Vector3 GetCenterOfObjects(List<Transform> objects)
    {
        Vector3 center = Vector3.zero;
        foreach (Transform t in objects)
        {
            center += t.position;
        }
        center /= objects.Count;
        return center;
    }
}