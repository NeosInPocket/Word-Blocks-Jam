using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static PlayerData Data { get; private set; } = new PlayerData();

    public static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении: {ex.Message}");
        }
    }

    public static void Load()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                Data = JsonUtility.FromJson<PlayerData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке: {ex.Message}");
                Data = new PlayerData();
                Save();
            }
        }
        else
        {
            Data = new PlayerData();
        }
    }

    public static void ResetProgress()
    {
        Data = new PlayerData();
        Save();
    }
}