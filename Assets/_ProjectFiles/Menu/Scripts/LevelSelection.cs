using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
   [SerializeField] private List<TMP_Text> levelTexts;

   public int BoardLevelNumber { get; private set; }
   public int AnalyticsLevelNumber { get; private set; }
   
   public void Load()
   {
      SaveManager.Load();

      var completed = SaveManager.Data.levelsCompleted;
      var simulatedCompleted = 0;
      
      if (completed < 50)
      {
         simulatedCompleted = completed;
      }
      else
      {
         int cycleLength = 31;
         int offset = (completed - 50) % cycleLength;
         simulatedCompleted = 19 + offset;
      }
      
      AnalyticsLevelNumber = completed + 1;
      BoardLevelNumber = simulatedCompleted + 1;
      
      UpdateLevel();
   }

   public void UpdateLevel()
   {
      SaveManager.Load();
      var data = SaveManager.Data;
      levelTexts.ForEach(x => x.text = "LEVEL " + BoardLevelNumber);
   }

   public void ResetProgress()
   {
      SaveManager.Load();
      SaveManager.Data.levelsCompleted = 0;
      SaveManager.Save();
      UpdateLevel();
   }
}
