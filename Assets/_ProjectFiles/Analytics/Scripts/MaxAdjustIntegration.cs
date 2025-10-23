using UnityEngine;

public class MaxAdjustIntegration : MonoBehaviour
{
    void Start()
    {
        InitializeAppLovin();
        
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    private void InitializeAppLovin()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += OnAppLovinInitialized;
        MaxSdk.InitializeSdk();
        Debug.Log("AppLovin MAX SDK initialization started");
    }

    private void OnAppLovinInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
    {
        Debug.Log("AppLovin MAX SDK initialized!");
    }

    public void ShowDebugger()
    {
        MaxSdk.ShowMediationDebugger();
    }
    
    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
        string network = adInfo.NetworkName;
        string unit = adInfo.AdUnitIdentifier;
        string placement = adInfo.Placement;
        string currency = "USD";

        AdjustRevenueManager.Instance.TrackAdRevenue(
            revenue,
            currency,
            network: network,
            unit: unit,
            placement: placement
        );
    }
}