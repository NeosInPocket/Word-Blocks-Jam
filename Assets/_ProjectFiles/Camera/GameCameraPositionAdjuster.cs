using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCameraPositionAdjuster
{
    private List<GameObject> objects;
    private Image mask;
    private Camera camera;

    public GameCameraPositionAdjuster(List<GameObject> objects, Image mask, Camera camera)
    {
        this.objects = objects;
        this.mask = mask;
        this.camera = camera;
    }

    public void GetOptimalCamera(out Vector3 optimalPosition, out float optimalSize)
    {
        if (objects == null || objects.Count == 0)
        {
            optimalPosition = camera.transform.position;
            optimalSize = camera.orthographicSize;
            return;
        }

        Bounds bounds = new Bounds(objects[0].transform.position, Vector3.zero);
        foreach (var obj in objects)
        {
            bounds.Encapsulate(obj.transform.position);
        }

        Vector3 objectsCenter = bounds.center;

        RectTransform maskRect = mask.rectTransform;
        Vector2 maskSize = maskRect.rect.size;

        Vector3 maskWorldScale = maskRect.lossyScale;
        float maskWorldWidth = maskSize.x * maskWorldScale.x;
        float maskWorldHeight = maskSize.y * maskWorldScale.y;

        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        Vector3 cameraUp = camera.transform.up;

        float maxX = 0f;
        float maxY = 0f;
        foreach (var obj in objects)
        {
            Vector3 localOffset = obj.transform.position - objectsCenter;
            float projX = Mathf.Abs(Vector3.Dot(localOffset, cameraRight));
            float projY = Mathf.Abs(Vector3.Dot(localOffset, cameraUp));

            maxX = Mathf.Max(maxX, projX);
            maxY = Mathf.Max(maxY, projY);
        }

        float sizeX = maxX / (maskWorldWidth / 2f);
        float sizeY = maxY / (maskWorldHeight / 2f);

        float requiredScale = Mathf.Max(sizeX, sizeY);
        optimalSize = camera.orthographicSize * requiredScale;

        Vector3 maskWorldPos = maskRect.position;
        Vector3 offset = maskWorldPos - objectsCenter;
        optimalPosition = objectsCenter + offset;
        optimalPosition.z = camera.transform.position.z;
    }
}
