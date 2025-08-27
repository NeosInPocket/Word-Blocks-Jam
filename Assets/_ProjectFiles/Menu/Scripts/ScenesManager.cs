using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        //levelText.text = "Level " + CurrentLevelInfo.Instance.LevelId;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        CurrentLevelInfo.Instance.LevelId++;
        SceneManager.LoadScene(1);
    }
}
