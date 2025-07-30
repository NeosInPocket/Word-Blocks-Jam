using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class WordLevelDownloader
{
    private string apiUrl = "https://script.google.com/macros/s/AKfycbxxg9W1HN3pnEystIKV9_bzn1g_Os1-eFLia0NTbZzKTlEhrK1NQvJVmQeRHaCk-NCD/exec";

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