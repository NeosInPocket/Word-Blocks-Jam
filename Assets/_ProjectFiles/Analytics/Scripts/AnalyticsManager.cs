using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using TMPro;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using System.Threading.Tasks;
#endif

using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text idfatext;
    public static AnalyticsManager Instance { get; private set; }
    
    #if UNITY_IOS
    
    private async void Awake()
    {
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
        
        while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
            {
                SaveManager.Load();
                SaveManager.Data.trackingAuthorized = true;
                SaveManager.Save();
            }
            
            await Task.Yield();
        }

        SaveManager.Load();

        if (SaveManager.Data.trackingAuthorized)
        {
            string id = String.Empty;
        
            Application.RequestAdvertisingIdentifierAsync(
                (string advertisingId, bool trackingEnabled, string error) =>
                { id = advertisingId; });
        
        
            SaveManager.Data.idfa = id;
            SaveManager.Save();
        }

        idfatext.text = SaveManager.Data.idfa;
    }
    
    #endif

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
