using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCameraPositionAdjuster : MonoBehaviour
{
    [SerializeField] public Camera targetCamera;
    [SerializeField] public Image maskRect;
    [SerializeField] public float additionalCameraSize;
    private GraphicRaycaster gr;
    private List<Vector3> worldVectors;
    public Vector3 PreferredPosition { get; private set; }
    public float PreferredSize { get; private set; }

    public void ApplyPreferredCameraParameters()
    {
        targetCamera.transform.position = PreferredPosition;
        targetCamera.orthographicSize = PreferredSize;
    }
    
    public void CalculateCameraParameters(List<Transform> objectsToFit)
    {
        gr = maskRect.canvas.GetComponent<GraphicRaycaster>();
        Vector3[] v = new Vector3[4];
        maskRect.rectTransform.GetWorldCorners(v);

        if (worldVectors == null)
        {
            worldVectors = new();
            
            for (var i = 0; i < 4; i++)
            {
                var normalized = new Vector3(v[i].x / Screen.width, v[i].y / Screen.height, 0);
                var world = targetCamera.ViewportToWorldPoint(normalized);
                worldVectors.Add(world);
            }
        }

        worldVectors = v.ToList();
        
        Vector3 maskCenter = (worldVectors[0] + worldVectors[2]) * 0.5f;
        Vector3 toMaskTarget = maskCenter - targetCamera.transform.position;
        
        Vector3 objectsCenter = GetCenterOfObjects(objectsToFit);
        
        Vector3 toTarget = objectsCenter - targetCamera.transform.position;
        Vector3 forward = targetCamera.transform.forward;
        
        Vector3 projection = Vector3.ProjectOnPlane(toTarget, forward);
        Vector3 maskCenterProjection = Vector3.ProjectOnPlane(toMaskTarget, forward);
        
        var startPosition = targetCamera.transform.position;
        targetCamera.transform.position += projection - maskCenterProjection;
        PreferredPosition = targetCamera.transform.position;
        
        float sizeIncrement = 0.1f;
        float minSize = 3f;
        float maxSize = 50f;
        float size = minSize;
        
        float startCameraSize = targetCamera.orthographicSize;
        
        while (size < maxSize)
        {
            targetCamera.orthographicSize = size;
            bool areAllInside = true;
            
            foreach (var obj in objectsToFit)
            {
                areAllInside &= IsRayHittingImage(targetCamera, obj, maskRect);
            }
        
            if (areAllInside)
            {
                size += additionalCameraSize;
                PreferredSize = size;
        
                targetCamera.orthographicSize = startCameraSize;
                targetCamera.transform.position = startPosition;
                return;
            }
            
            size += sizeIncrement;
        }
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

        Vector3 hitPoint = origin + direction * (enter + 1f);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, hitPoint);

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();

        gr.Raycast(pointerData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetImage.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}