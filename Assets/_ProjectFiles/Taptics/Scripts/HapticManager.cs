using UnityEngine;
using CandyCoded.HapticFeedback;

public class HapticManager : MonoBehaviour
{
    private static HapticManager _instance;
    public static HapticManager Instance { get; private set; }

    private bool _isHapticsEnabled;

    private void Start()
    {
        Instance = this;
        SaveManager.Load();
        IsHapticsEnabled = SaveManager.Data.hapticsEnabled;
    }

    public bool IsHapticsEnabled
    {
        get => _isHapticsEnabled;
        set
        {
            _isHapticsEnabled = value;
            SaveManager.Load();
            SaveManager.Data.hapticsEnabled = value;
            SaveManager.Save();
        }
    }

    public void PlayHaptic()
    {
        if (!IsHapticsEnabled) return;

        HapticFeedback.MediumFeedback();
    }

    public void ToggleHaptics()
    {
        IsHapticsEnabled = !IsHapticsEnabled;
    }
}
