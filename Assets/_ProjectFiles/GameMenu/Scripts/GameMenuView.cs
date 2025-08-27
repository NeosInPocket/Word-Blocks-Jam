using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuView : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
