using System;
using UnityEngine;

public class PlayerSelectionTracker : MonoBehaviour
{
    public Action<LetterUnit> OnLetterUnitTapped { get; set; }
    public static PlayerSelectionTracker Instance { get; private set; }
    private Camera MainCamera => Camera.main;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (PlayerInputTracker.Instance == null || !PlayerInputTracker.Instance.HasActiveTouch)
            return;

        Vector2 touchPosition = PlayerInputTracker.Instance.TouchPosition;
        Ray ray = MainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var fillBox = hit.collider.GetComponentInParent<LetterUnit>();
            if (fillBox != null)
            {
                OnLetterUnitTapped?.Invoke(fillBox);
            }
        }
    }
}