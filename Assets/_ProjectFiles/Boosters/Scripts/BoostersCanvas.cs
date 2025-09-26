using UnityEngine;

public class BoostersCanvas : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void HideBoosters()
    {
        animator.SetTrigger("Hide");
    }

    public void OnBoostersHidden()
    {
        gameObject.SetActive(false);
    }
}
