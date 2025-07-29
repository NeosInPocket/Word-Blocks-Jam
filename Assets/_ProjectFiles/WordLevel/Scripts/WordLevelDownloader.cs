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
            WordLevelRawResponse raw = JsonUtility.FromJson<WordLevelRawResponse>(json);

            return ParseRawLevels(raw.levels);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading word levels: {e.Message}");
            return null;
        }
    }

    private List<WordLevel> ParseRawLevels(List<RawLevel> rawLevels)
    {
        List<WordLevel> parsedLevels = new();

        foreach (var raw in rawLevels)
        {
            var level = new WordLevel
            {
                levelId = raw.levelId,
                words = new List<WordWithRange>()
            };

            foreach (string entry in raw.words)
            {
                if (string.IsNullOrWhiteSpace(entry)) continue;

                string word = entry;
                int rowMin = 0;
                int rowMax = int.MaxValue;

                // Пример: world (1-2)
                int openIndex = entry.IndexOf('(');
                int closeIndex = entry.IndexOf(')');

                if (openIndex > 0 && closeIndex > openIndex)
                {
                    word = entry.Substring(0, openIndex).Trim();
                    string rangePart = entry.Substring(openIndex + 1, closeIndex - openIndex - 1);

                    var parts = rangePart.Split('-');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int min) &&
                        int.TryParse(parts[1], out int max))
                    {
                        rowMin = min;
                        rowMax = max;
                    }
                }

                level.words.Add(new WordWithRange(word, rowMin, rowMax));
            }

            parsedLevels.Add(level);
        }

        return parsedLevels;
    }
}
