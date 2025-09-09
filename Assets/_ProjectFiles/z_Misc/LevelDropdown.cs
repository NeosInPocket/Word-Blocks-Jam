using UnityEngine;

public class LevelDropdown : MonoBehaviour
{
    [SerializeField] private LevelBootstrap bootstrap;
    
    public void RefreshLevel(string input)
    {
        if (int.TryParse(input, out int level))
        {
            bootstrap.SelectLevel(level);   
        }
    }
}
