using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
   [SerializeField] private TMP_Text levelText;

   public void Load()
   {
      SaveManager.Load();
      
      var levelInfo = new CurrentLevelInfo(SaveManager.Data.levelsCompleted + 1);
      SceneManager.LoadScene(1);
   }

   public void UpdateLevel()
   {
      SaveManager.Load();
      var data = SaveManager.Data;
      levelText.text = "LEVEL " + (data.levelsCompleted + 1);
   }

   public void ResetProgress()
   {
      SaveManager.Load();
      SaveManager.Data.levelsCompleted = 0;
      SaveManager.Save();
      UpdateLevel();
   }
}
