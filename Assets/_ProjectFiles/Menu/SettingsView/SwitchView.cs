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
    private float absolutePosition;

    private void Start()
    {
        absolutePosition = Mathf.Abs(switchTransform.anchoredPosition.x);
        
        SaveManager.Load();
        var pos = switchTransform.anchoredPosition;
        pos.x = SaveManager.Data.hapticsEnabled ? absolutePosition : -absolutePosition;
    }

    public void Switch()
    {
        _isOn = !_isOn;
        Vector2 endPosition = switchTransform.anchoredPosition;
        endPosition.x = _isOn ? absolutePosition : -absolutePosition;

        switchTransform.DOAnchorPos(endPosition, switchDuration).SetEase(animationEase);
        switchText.text = _isOn ? "ON" : "OFF";
        
        HapticManager.Instance.IsHapticsEnabled = _isOn;
    }
}
