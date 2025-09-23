using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCameraPositionAdjuster : MonoBehaviour
{
    public Camera targetCamera;
    public Image maskRect;
    
    public void FitCamera(List<Transform> objectsToFit)
    {
        Vector3[] v = new Vector3[4];
        maskRect.rectTransform.GetWorldCorners(v);
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

        targetCamera.transform.position += projection - maskCenterProjection;
        
        // List<Vector3> objectProjections = new List<Vector3>();
        // foreach (var obj in objectsToFit)
        // {
        //     Vector3 toObj = obj.position;
        //     Vector3 localProj = Vector3.ProjectOnPlane(toObj, forward);
        //     objectProjections.Add(localProj);
        // }
        //
        // float sizeIncrement = 0.1f;
        // float minSize = 7f;
        // float maxSize = 50f;
        // float currentSize = minSize;
        //
        // while (currentSize < maxSize)
        // {
        //     //targetCamera.orthographicSize = currentSize;
        //     bool areAllInside = true;
        //     
        //     foreach (var obj in objectsToFit)
        //     {
        //         areAllInside &= IsRayHittingImage(targetCamera, obj, maskRect);
        //     }
        //
        //     if (areAllInside)
        //         return;
        //     
        //     currentSize += sizeIncrement;
        // }
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
    
    private bool IsRayHittingImage(Camera cam, Transform shooter, Image targetImage)
    {
        Canvas canvas = targetImage.canvas;
        if (canvas == null) return false;

        Vector3 origin = shooter.position;

        Vector3 toPlane = canvas.transform.position - origin;
        Vector3 direction = Vector3.Dot(toPlane, cam.transform.forward) >= 0 
            ? cam.transform.forward 
            : -cam.transform.forward;

        Plane canvasPlane = new Plane(canvas.transform.forward, canvas.transform.position);
        if (!canvasPlane.Raycast(new Ray(origin, direction), out float enter))
            return false;

        Vector3 hitPoint = origin + direction * enter;

        Debug.DrawRay(origin, direction * enter, Color.red, 100f);

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, hitPoint);

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        if (gr == null) return false;

        gr.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetImage.gameObject)
                return true;
        }

        return false;
    }
}