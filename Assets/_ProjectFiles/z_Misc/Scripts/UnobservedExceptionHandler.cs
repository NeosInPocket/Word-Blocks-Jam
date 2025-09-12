using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class UnobservedExceptionHandler : MonoBehaviour
{
    private void Awake()
    {
        UniTaskScheduler.UnobservedTaskException += OnUnobservedException;
    }

    private void OnDestroy()
    {
        UniTaskScheduler.UnobservedTaskException -= OnUnobservedException;
    }

    private void OnUnobservedException(Exception exception)
    {
        Debug.LogError($"[UniTask Global Exception] {exception.Message}");
    }
}