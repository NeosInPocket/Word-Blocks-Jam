using UnityEngine;
using DG.Tweening;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Vector3 menuPosition;
    [SerializeField] private float menuSize;
    [SerializeField] private Vector3 gamePosition;
    [SerializeField] private float gameSize;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.Linear;

    private void Start()
    {
        targetCamera.transform.position = menuPosition;
        targetCamera.orthographicSize = menuSize;
    }

    public void Initialize(Vector3 gamePosition, float gameSize)
    {
        menuPosition = gamePosition;
        menuSize = gameSize;
    }
    
    public void MoveToMenuState()
    {
        targetCamera.transform.DOMove(menuPosition, duration).SetEase(ease);
        targetCamera.DOOrthoSize(menuSize, duration).SetEase(ease);
    }

    public void MoveToGameState()
    {
        targetCamera.transform.DOMove(gamePosition, duration).SetEase(ease);
        targetCamera.DOOrthoSize(gameSize, duration).SetEase(ease);
    }

    public void SetDefaultState()
    {
        targetCamera.transform.position = menuPosition;
        targetCamera.orthographicSize = menuSize;
    }
}