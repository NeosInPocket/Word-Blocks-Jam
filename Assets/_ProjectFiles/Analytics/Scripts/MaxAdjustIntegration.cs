using UnityEngine;

public class MaxAdjustIntegration : MonoBehaviour
{
    void Start()
    {
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
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