using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }
    
    public void SendNextLevelEvent()
    {
        SaveManager.Load();
        var currentLevel = SaveManager.Data.levelsCompleted + 1;
        
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", currentLevel.ToString());

        AppsFlyer.sendEvent("af_next_level", eventValues);

        Debug.Log($"Event: load next level: {currentLevel}");
    }

    public void SendLevelStartedEvent()
    {
        SaveManager.Load();
        var currentLevel = SaveManager.Data.levelsCompleted + 1;
        
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", currentLevel.ToString());

        AppsFlyer.sendEvent("af_level_started", eventValues);

        Debug.Log($"Event: Level started: {currentLevel}");
    }

    public void SendLevelCompletedEvent()
    {
        SaveManager.Load();
        var currentLevel = SaveManager.Data.levelsCompleted + 1;
        
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", currentLevel.ToString());
        AppsFlyer.sendEvent("af_level_completed", eventValues);
        Debug.Log($"Event: Level completed: {currentLevel}");
    }

}
