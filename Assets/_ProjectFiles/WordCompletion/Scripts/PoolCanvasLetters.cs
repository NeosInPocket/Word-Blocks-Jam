using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolCanvasLetters : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> letters;
    public List<TMP_Text> Letters => letters;

    public void SetWord(string word)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            letters[i].text = word[i].ToString();
        }
    }
}
