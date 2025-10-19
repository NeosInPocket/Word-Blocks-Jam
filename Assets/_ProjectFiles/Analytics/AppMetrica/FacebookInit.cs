using Facebook.Unity;
using UnityEngine;

public class FacebookInit : MonoBehaviour
{
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: " + FB.IsInitialized);
        FB.ActivateApp();
    }

    private void OnHideUnity(bool isGameShown)
    {
        Time.timeScale = isGameShown ? 1 : 0;
    }
}
