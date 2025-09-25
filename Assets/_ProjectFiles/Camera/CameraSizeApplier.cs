using UnityEngine;

public class CameraWidthApplier
{
    private const float ReferenceOrthoSize = 7.7f;
    private const int ReferenceWidth = 1080;
    private const int ReferenceHeight = 2160;

    private Camera _cam;
    private float _preferredSize;
    public float PreferredSize => _preferredSize;

    public CameraWidthApplier(Camera cam)
    {
        _cam = cam;
        CalculateScale();
    }

    public void CalculateScale()
    {
        _preferredSize = ReferenceOrthoSize;
    }
}