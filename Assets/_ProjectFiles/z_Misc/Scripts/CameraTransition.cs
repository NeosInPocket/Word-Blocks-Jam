using System;
using UnityEngine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.Linear;

    private void Update()
    {
        uiCamera.orthographicSize = targetCamera.orthographicSize;
    }

    public void MoveTo(Vector3 menuPosition, float menuSize)
    {
        targetCamera.transform.DOMove(menuPosition, duration).SetEase(ease);
        targetCamera.DOOrthoSize(menuSize, duration).SetEase(ease);
    }
}