using UnityEngine;
using System;

public class LevelTimer : MonoBehaviour
{
    private float levelStartTime;
    private bool levelRunning = false;
    private int levelTime;

    public static LevelTimer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel()
    {
        levelStartTime = Time.realtimeSinceStartup;
        levelRunning = true;
    }

    public void FinishLevel()
    {
        if (!levelRunning) return;
        levelTime = (int)(Time.realtimeSinceStartup - levelStartTime);
        levelRunning = false;
    }

    public int GetLevelTime()
    {
        if (levelRunning)
        {
            return (int)(Time.realtimeSinceStartup - levelStartTime);
        }
        return levelTime;
    }
}