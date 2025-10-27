using AdjustSdk;
using UnityEngine;

public sealed class AdjustEventManager
{
    private static readonly AdjustEventManager _instance = new AdjustEventManager();
    public static AdjustEventManager Instance => _instance;

    private int session_id;
    private int in_game_time => GameTimeTracker.Instance.AllGameTime;

    private int max_level => LevelSelection.Instance.AnalyticsLevelNumber;

    private AdjustEventManager() { }

    public void SetGlobalParams(int session)
    {
        session_id = session;
    }

    private void AddGlobalParams(AdjustEvent adjustEvent)
    {
        adjustEvent.AddCallbackParameter("session_id", session_id.ToString());
        adjustEvent.AddCallbackParameter("in_game_time", in_game_time.ToString());
        adjustEvent.AddCallbackParameter("max_level", max_level.ToString());
    }

    public void TrackPayment(string token, bool isFirstPayment, string productId, string transactionId, double revenue, string currency)
    {
        var adjustEvent = new AdjustEvent(token);
        adjustEvent.SetRevenue(revenue, currency);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("is_first_payment", isFirstPayment ? "1" : "0");
        adjustEvent.AddCallbackParameter("product_id", productId);
        adjustEvent.AddCallbackParameter("transaction_id", transactionId);
        Adjust.TrackEvent(adjustEvent);
    }

    public void TrackLevelStart(string token, int levelId, string levelName, string levelLine = "")
    {
        var adjustEvent = new AdjustEvent(token);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("level_id", levelId.ToString());
        adjustEvent.AddCallbackParameter("level_name", levelName);
        if (!string.IsNullOrEmpty(levelLine))
            adjustEvent.AddCallbackParameter("level_line", levelLine);
        Adjust.TrackEvent(adjustEvent);

        Debug.Log("<color=green>Level started: " + levelId + "</color>");
    }

    public void TrackLevelFinish(string token, int levelId, string levelName, string levelResult, int levelTime, string levelLine = "")
    {
        var adjustEvent = new AdjustEvent(token);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("level_id", levelId.ToString());
        adjustEvent.AddCallbackParameter("level_name", levelName);
        adjustEvent.AddCallbackParameter("level_result", levelResult);
        adjustEvent.AddCallbackParameter("level_time", levelTime.ToString());
        if (!string.IsNullOrEmpty(levelLine))
            adjustEvent.AddCallbackParameter("level_line", levelLine);
        Adjust.TrackEvent(adjustEvent);
        
        Debug.Log("<color=green>Level finished: " + levelId + "</color>");
    }

    public void TrackABTest(string token, string testName, string testGroup)
    {
        var adjustEvent = new AdjustEvent(token);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("test_name", testName);
        adjustEvent.AddCallbackParameter("test_group", testGroup);
        Adjust.TrackEvent(adjustEvent);
    }

    public void TrackUABalance(string token, string balance)
    {
        var adjustEvent = new AdjustEvent(token);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("balance", balance);
        Adjust.TrackEvent(adjustEvent);
    }

    public void TrackMilestone(string token, string eventName)
    {
        var adjustEvent = new AdjustEvent(token);
        AddGlobalParams(adjustEvent);
        adjustEvent.AddCallbackParameter("event_name", eventName);
        Adjust.TrackEvent(adjustEvent);
    }
}
