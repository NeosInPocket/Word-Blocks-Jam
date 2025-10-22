using System.Collections.Generic;
using AdjustSdk;
using UnityEngine;

public class AdjustRevenueManager : MonoBehaviour
{
    private static AdjustRevenueManager _instance;
    public static AdjustRevenueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("AdjustRevenueManager");
                _instance = go.AddComponent<AdjustRevenueManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void TrackAdRevenue(double revenue, string currency, int? impressions = null, string network = null, string unit = null, string placement = null, Dictionary<string,string> callbackParameters = null, Dictionary<string,string> partnerParameters = null)
    {
        var source = "applovin_max_sdk";
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(source);

        adRevenue.SetRevenue(revenue, currency);

        if (impressions.HasValue)
        {
            adRevenue.AdImpressionsCount = impressions.Value;
        }

        if (!string.IsNullOrEmpty(network))
        {
            adRevenue.AdRevenueNetwork = network;
        }

        if (!string.IsNullOrEmpty(unit))
        {
            adRevenue.AdRevenueUnit = unit;
        }

        if (!string.IsNullOrEmpty(placement))
        {
            adRevenue.AdRevenuePlacement = placement;
        }

        if (callbackParameters != null)
        {
            foreach (var kvp in callbackParameters)
            {
                adRevenue.AddCallbackParameter(kvp.Key, kvp.Value);
            }
        }

        if (partnerParameters != null)
        {
            foreach (var kvp in partnerParameters)
            {
                adRevenue.AddPartnerParameter(kvp.Key, kvp.Value);
            }
        }

        Adjust.TrackAdRevenue(adRevenue);
    }
}