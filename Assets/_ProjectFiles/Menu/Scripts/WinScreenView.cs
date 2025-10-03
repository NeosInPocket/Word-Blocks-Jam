using System;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenView : MonoBehaviour
{
    [SerializeField] private LevelBootstrap bootstrap;
    [SerializeField] private Animator animator;
    [SerializeField] private MenuView menuView;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private Button settingsButton;

    private void OnEnable()
    {
        settingsButton.interactable = false;
    }

    public void OnNextLevel()
    {
        bootstrap.UpdateLevel(false);
        animator.SetTrigger("Close");
        tracker.Enable(true);
        bootstrap.ApplyGameCamera();
    }

    public void OnMenu()
    {
        bootstrap.UpdateLevel(false);
        animator.SetTrigger("Close");
        menuView.Animator.SetTrigger("Open");
        menuView.EnableMenuSelection(true);
    }

    public void OnClose()
    {
        settingsButton.interactable = true;
        gameObject.SetActive(false);
    }
}
