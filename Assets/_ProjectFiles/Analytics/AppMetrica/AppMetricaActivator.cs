using Io.AppMetrica;
using UnityEngine;

public static class AppMetricaActivator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Activate()
    {
        var config = new AppMetricaConfig("c2d55cb8-2efb-4529-960f-ec282829b53c")
        {
            LocationTracking = false,
            FirstActivationAsUpdate = !IsFirstLaunch(),
        };
        AppMetrica.Activate(config);
    }

    private static bool IsFirstLaunch()
    {
        return !PlayerPrefs.HasKey("first_launch_done");
    }
}