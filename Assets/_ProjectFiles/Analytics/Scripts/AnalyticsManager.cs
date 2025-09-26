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
    
    public void SendNextLevelEvent(int level)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", level.ToString());

        AppsFlyer.sendEvent("af_next_level", eventValues);

        Debug.Log($"Event: load next level: {level}");
    }

    public void SendLevelStartedEvent(int level)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", level.ToString());

        AppsFlyer.sendEvent("af_level_started", eventValues);

        Debug.Log($"Event: Level started: {level}");
    }

    public void SendLevelCompletedEvent(int level)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level_number", level.ToString());
        AppsFlyer.sendEvent("af_level_completed", eventValues);
        Debug.Log($"Event: Level completed: {level}");
    }

}
