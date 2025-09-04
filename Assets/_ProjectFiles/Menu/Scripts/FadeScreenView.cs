using System;
using UnityEngine;

public class FadeScreenView : MonoBehaviour
{
    [SerializeField] private MenuView menuView;
    private Action _onClosed;
    
    public void CloseMenu(Action onClosed)
    {
        menuView.EnableMenuSelection(false);
        _onClosed = onClosed;
        
        menuView.Animator.SetTrigger("Close");
    }

    public void OpenMenu()
    {
        menuView.EnableMenuSelection(true);
        menuView.Animator.SetTrigger("Open");
    }

    public void OnClosedTrigger()
    {
        _onClosed();
    }
}
