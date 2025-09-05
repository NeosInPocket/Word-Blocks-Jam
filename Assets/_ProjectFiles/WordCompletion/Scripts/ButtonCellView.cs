using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class ButtonCellView : MonoBehaviour
{
    [SerializeField] private float pressOffset = 0.2f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Vector3 originalPosition;

    public void OnButtonPressed()
    {
        originalPosition = transform.position;
        Vector3 targetPos = originalPosition - new Vector3(0, pressOffset, 0);
        transform.DOMove(targetPos, duration).SetEase(easeType);
    }

    public void StopMove()
    {
        DOTween.Kill(transform);
    }
}