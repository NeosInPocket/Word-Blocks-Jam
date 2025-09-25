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

        Vector3 maskCenter = (worldVectors[0] + worldVectors[2]) * 0.5f;
        Vector3 toMaskTarget = maskCenter - targetCamera.transform.position;
        
        Vector3 objectsCenter = GetCenterOfObjects(objectsToFit);
        
        Vector3 toTarget = objectsCenter - targetCamera.transform.position;
        Vector3 forward = targetCamera.transform.forward;
        
        Vector3 projection = Vector3.ProjectOnPlane(toTarget, forward);
        Vector3 maskCenterProjection = Vector3.ProjectOnPlane(toMaskTarget, forward);
        
        PreferredPosition = targetCamera.transform.position + projection - maskCenterProjection;

        List<Vector3> screenPositions = objectsToFit.Select(x => targetCamera.WorldToScreenPoint(x.position)).ToList();
        
        float xMin = screenPositions.Min(x => x.x);
        float xMax = screenPositions.Max(x => x.x);
        float yMin = screenPositions.Min(x => x.y);
        float yMax = screenPositions.Max(x => x.y);
        
        float width = xMax - xMin;
        float height = yMax - yMin;
        
        float maskWidth = Vector3.Distance(v[0], v[3]);
        float maskHeight = Vector3.Distance(v[0], v[1]);

        float ratio = 0;
        
        if (height > width)
        {
            ratio = maskHeight / height;
           
        }
        else
        {
            ratio = maskWidth / width;
        }
        
        PreferredSize = targetCamera.orthographicSize / ratio;
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