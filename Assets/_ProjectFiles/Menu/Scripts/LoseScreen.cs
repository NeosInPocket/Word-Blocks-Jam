using System;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private LevelBootstrap bootstrap;
    [SerializeField] private Animator animator;
    [SerializeField] private MenuView menuView;
    [SerializeField] private PlayerSelectionTracker tracker;
    [SerializeField] private Button menuButton;

    private void OnEnable()
    {
        menuButton.interactable = false;
    }

    public void OnReplay()
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
        menuButton.interactable = true;
        gameObject.SetActive(false);
    }
}