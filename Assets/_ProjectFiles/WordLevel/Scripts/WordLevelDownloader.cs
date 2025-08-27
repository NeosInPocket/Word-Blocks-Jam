using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class WordLevelDownloader
{
    private string apiUrl = "https://script.google.com/macros/s/AKfycbyb9koODQwTrzKzAjRfqyP2rlN631kHPI0GaBLnRwXQ00tdohcjJH6y_u1P6875jp9S/exec";

    public async Task<List<RawLevel>> LoadLevelsAsync()
    {
        using HttpClient client = new();

        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            WordLevelRawResponse parsed = JsonUtility.FromJson<WordLevelRawResponse>(json);
            return parsed?.levels ?? new List<RawLevel>();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading word levels: {e.Message}");
            return null;
        }
    }
}