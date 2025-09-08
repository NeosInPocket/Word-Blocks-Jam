using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
   [SerializeField] private List<TMP_Text> levelTexts;

   public int Load()
   {
      SaveManager.Load();
      UpdateLevel();
      
      return SaveManager.Data.levelsCompleted + 1;
   }

   public void UpdateLevel()
   {
      SaveManager.Load();
      var data = SaveManager.Data;
      levelTexts.ForEach(x => x.text = "LEVEL " + (data.levelsCompleted + 1));
   }

   public void ResetProgress()
   {
      SaveManager.Load();
      SaveManager.Data.levelsCompleted = 0;
      SaveManager.Save();
      UpdateLevel();
   }
}
