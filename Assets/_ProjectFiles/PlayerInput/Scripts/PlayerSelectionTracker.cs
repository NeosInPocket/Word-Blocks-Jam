using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerSelectionTracker : MonoBehaviour
{
    public Action<BoardCellView> OnBoardCellTapped { get; set; }
    public static PlayerSelectionTracker Instance { get; private set; }
    private Camera MainCamera => Camera.main;

    private void Awake()
    {
        Instance = this;
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        
        Touch.onFingerDown += OnTouch;
    }

    private void OnTouch(Finger obj)
    {
        Vector2 touchPosition = obj.screenPosition;
        Ray ray = MainCamera.ScreenPointToRay(touchPosition);

        var raycasts = Physics.RaycastAll(ray).ToList();
        var boardCellHit = raycasts.FirstOrDefault(x => x.collider != null && x.collider.transform.parent.GetComponent<BoardCellView>() != null);

        if (boardCellHit.collider != null)
        {
            BoardCellView cellView = boardCellHit.collider.GetComponent<BoardCellView>();
            OnBoardCellTapped?.Invoke(cellView);
        }
    }
}