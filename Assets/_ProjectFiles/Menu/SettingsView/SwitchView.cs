using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SwitchView : MonoBehaviour
{
    [SerializeField] private RectTransform switchTransform;
    [SerializeField] private float switchDuration;
    [SerializeField] private Ease animationEase;
    [SerializeField] private TMP_Text switchText;
    private bool _isOn;
    private Vector2 _startPosition;

    private void Start()
    {
        _startPosition = switchTransform.anchoredPosition;
    }

    public void Switch()
    {
        _isOn = !_isOn;
        Vector2 endPosition = _startPosition;
        endPosition.x = _isOn ? -_startPosition.x : _startPosition.x;

        switchTransform.DOAnchorPos(endPosition, switchDuration).SetEase(animationEase);
        switchText.text = _isOn ? "ON" : "OFF";
    }
}
