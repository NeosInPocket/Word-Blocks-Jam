using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [SerializeField] private List<Selectable> selectables;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    public void EnableMenuSelection(bool value)
    {
        selectables.ForEach(x => x.interactable = value);
    }
}