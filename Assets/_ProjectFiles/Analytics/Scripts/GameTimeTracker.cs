using UnityEngine;

public class GameTimeTracker : MonoBehaviour
{
    private float sessionStartTime;
    private int accumulatedTime;

    public int InGameTime => accumulatedTime + (int)(Time.realtimeSinceStartup - sessionStartTime);

    public int AllGameTime
    {
        get
        {
            SaveManager.Load();
            var time = InGameTime + SaveManager.Data.timePlayed;
            return time;
        }
    }
    
    
    public static GameTimeTracker Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            sessionStartTime = Time.realtimeSinceStartup;
        }
        else
        {
            accumulatedTime += (int)(Time.realtimeSinceStartup - sessionStartTime);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            accumulatedTime += (int)(Time.realtimeSinceStartup - sessionStartTime);
        }
        else
        {
            sessionStartTime = Time.realtimeSinceStartup;
        }
    }

    void OnApplicationQuit()
    {
        accumulatedTime += (int)(Time.realtimeSinceStartup - sessionStartTime);
        
        SaveManager.Load();
        SaveManager.Data.timePlayed += InGameTime;
        SaveManager.Save();
    }
}