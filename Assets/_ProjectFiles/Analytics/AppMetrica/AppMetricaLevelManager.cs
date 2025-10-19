using UnityEngine;
using Io.AppMetrica;

public class AppMetricaLevelManager : MonoBehaviour
{
    public static AppMetricaLevelManager Instance { get; private set; }

    private float levelStartTime;
    private int currentLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (!PlayerPrefs.HasKey("registration_date"))
        {
            PlayerPrefs.SetString("registration_date", System.DateTime.UtcNow.ToString("o"));
            Debug.Log($"[AppMetricaLevelManager] Registration date set: {System.DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        }
        else
        {
            Debug.Log($"[AppMetricaLevelManager] Registration date found: {PlayerPrefs.GetString("registration_date")}");
        }
    }

    public void ReportLevelStart(int levelNumber)
    {
        currentLevel = levelNumber;
        levelStartTime = Time.time;

        int daysSinceReg = GetDaysSinceRegistration();
        var now = System.DateTime.UtcNow;

        string eventParams = $"{{\"level\":\"{currentLevel}\", \"days_since_reg\":\"{daysSinceReg}\"}}";
        AppMetrica.ReportEvent("level_start", eventParams);
        AppMetrica.SendEventsBuffer();

        Debug.Log($"[AppMetricaLevelManager] Level {currentLevel} start @ {now:yyyy-MM-dd HH:mm:ss} | Days since registration: {daysSinceReg}");
        Debug.Log($"[AppMetricaLevelManager] Sent: level_start, params: {eventParams}");
    }

    public void ReportLevelComplete()
    {
        float timeSpent = Time.time - levelStartTime;
        int daysSinceReg = GetDaysSinceRegistration();
        var now = System.DateTime.UtcNow;

        string eventParams = $"{{\"level\":\"{currentLevel}\", \"time_spent\":\"{(int)timeSpent}\", \"days_since_reg\":\"{daysSinceReg}\"}}";
        AppMetrica.ReportEvent("level_complete", eventParams);
        AppMetrica.SendEventsBuffer();

        Debug.Log($"[AppMetricaLevelManager] Level {currentLevel} complete @ {now:yyyy-MM-dd HH:mm:ss} | Time spent: {(int)timeSpent}s | Days since registration: {daysSinceReg}");
        Debug.Log($"[AppMetricaLevelManager] Sent: level_complete, params: {eventParams}");
        Debug.Log($"[AppMetricaLevelManager] Forced sending AppMetrica buffer @ {now:yyyy-MM-dd HH:mm:ss}");
    }

    public void ReportLevelFailed()
    {
        float timeSpent = Time.time - levelStartTime;
        int daysSinceReg = GetDaysSinceRegistration();
        var now = System.DateTime.UtcNow;

        string eventParams = $"{{\"level\":\"{currentLevel}\", \"time_spent\":\"{(int)timeSpent}\", \"days_since_reg\":\"{daysSinceReg}\"}}";
        AppMetrica.ReportEvent("level_failed", eventParams);
        AppMetrica.SendEventsBuffer();

        Debug.Log($"[AppMetricaLevelManager] Level {currentLevel} failed @ {now:yyyy-MM-dd HH:mm:ss} | Time spent: {(int)timeSpent}s | Days since registration: {daysSinceReg}");
        Debug.Log($"[AppMetricaLevelManager] Sent: level_failed, params: {eventParams}");
        Debug.Log($"[AppMetricaLevelManager] Forced sending AppMetrica buffer @ {now:yyyy-MM-dd HH:mm:ss}");
    }


    private int GetDaysSinceRegistration()
    {
        string regDateString = PlayerPrefs.GetString("registration_date");
        System.DateTime regDate = System.DateTime.Parse(regDateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        return (System.DateTime.UtcNow - regDate).Days;
    }
}
