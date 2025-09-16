using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraWidthApplier : MonoBehaviour
{
    [Header("Reference Settings")]
    public float referenceOrthoSize = 6.63f;
    public int referenceWidth = 1080;
    public int referenceHeight = 2160;

    private Camera cam;
    private float targetWorldWidth;

    void Start()
    {
        cam = GetComponent<Camera>();

        float referenceAspect = (float)referenceWidth / referenceHeight;
        targetWorldWidth = 2f * referenceOrthoSize * referenceAspect;

        ApplyScale();
    }

    void ApplyScale()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        cam.orthographicSize = targetWorldWidth / (2f * currentAspect);
    }
}