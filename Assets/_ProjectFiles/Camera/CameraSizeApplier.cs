using UnityEngine;

public class CameraWidthApplier
{
    private const float ReferenceOrthoSize = 6.63f;
    private const int ReferenceWidth = 1080;
    private const int ReferenceHeight = 2160;
    
    private Camera _cam;
    private float _targetWorldWidth;
    
    public CameraWidthApplier(Camera cam)
    {
        _cam = cam;
        
        float referenceAspect = (float)ReferenceWidth / ReferenceHeight;
        _targetWorldWidth = 2f * ReferenceOrthoSize * referenceAspect;

        ApplyScale();
    }

    public void ApplyScale()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        _cam.orthographicSize = _targetWorldWidth / (2f * currentAspect);
    }
}