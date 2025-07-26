using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class WordLevelDownloader
{
    private string apiUrl = "https://script.google.com/macros/s/AKfycbzwvXTefHuRrf03xiO_4g_EKDnIIxFcdXeXXatjx_eOCrJb9KsWrGGvQ-OzZiRTTlDH/exec";

    public async Task<List<WordLevel>> LoadLevelsAsync()
    {
        using HttpClient client = new HttpClient();

        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            WordLevelResponse parsed = JsonUtility.FromJson<WordLevelResponse>(json);
            return parsed.levels;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading word levels: {e.Message}");
            return null;
        }
    }
}