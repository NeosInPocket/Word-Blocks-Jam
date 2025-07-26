using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerInputTracker : MonoBehaviour
{
    public bool HasActiveTouch => activeFinger != null;
    public Vector2 TouchPosition => activeFinger?.screenPosition ?? Vector2.zero;

    private Finger activeFinger;
    public static PlayerInputTracker Instance { get; private set; }
    private Vector2 lastTouchPosition;
    public Vector2 LastTouchPosition => lastTouchPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else return;
        
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    private void Update()
    {
        if (HasActiveTouch)
        {
            lastTouchPosition = TouchPosition;
        }
    }

    private void OnEnable()
    {
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;

        EnhancedTouchSupport.Disable();
    }

    private void OnFingerDown(Finger finger)
    {
        if (activeFinger == null)
            activeFinger = finger;
    }

    private void OnFingerUp(Finger finger)
    {
        if (finger == activeFinger)
            activeFinger = null;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}