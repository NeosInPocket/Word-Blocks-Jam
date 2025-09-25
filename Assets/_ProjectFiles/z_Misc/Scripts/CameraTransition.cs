using System;
using UnityEngine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Vector3 gamePosition;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.Linear;

    private void Update()
    {
        uiCamera.orthographicSize = targetCamera.orthographicSize;
    }

    public void MoveToMenuState(Vector3 menuPosition, float menuSize)
    {
        targetCamera.transform.DOMove(menuPosition, duration).SetEase(ease);
        targetCamera.DOOrthoSize(menuSize, duration).SetEase(ease);
    }

    public void MoveToGameState(float gameSize)
    {
        targetCamera.transform.DOMove(gamePosition, duration).SetEase(ease);
        targetCamera.DOOrthoSize(gameSize, duration).SetEase(ease);
    }
}