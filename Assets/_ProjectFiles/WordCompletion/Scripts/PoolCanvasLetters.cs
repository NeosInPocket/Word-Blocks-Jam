using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolCanvasLetters : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> letters;
    private Canvas _canvas;
    
    public List<TMP_Text> Letters => letters;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = GameObject.Find("UI Camera").GetComponent<Camera>();
    }

    public void SetWord(string word)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            letters[i].text = word[i].ToString();
        }
    }
}
