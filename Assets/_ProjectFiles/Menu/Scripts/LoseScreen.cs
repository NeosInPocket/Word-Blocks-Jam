using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private LevelBootstrap bootstrap;
    [SerializeField] private Animator animator;
    [SerializeField] private MenuView menuView;
    [SerializeField] private PlayerSelectionTracker tracker;

    public void OnReplay()
    {
        bootstrap.UpdateLevel(false);
        animator.SetTrigger("Close");
        tracker.Enable(true);
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
        gameObject.SetActive(false);
    }
}