using UnityEngine;

public class SDKInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public void Initialize()
    {
        MaxSdk.InitializeSdk();
    }
}
