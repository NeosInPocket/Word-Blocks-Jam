using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostersManager : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private UnusedLettersPoolView unusedLettersPool;
    [SerializeField] private TMP_Text unlockBoosterText;
    [SerializeField] private TMP_Text extraCellBoosterText;
    [SerializeField] private Button unlockBoosterButton;
    [SerializeField] private Button extraCellBoosterButton;

    private bool _unlockBoosterUsed;
    private bool _extraCellBoosterUsed;
    
    private void Start()
    {
        unlockBoosterButton.onClick.AddListener(UseUnlockBooster);
        extraCellBoosterButton.onClick.AddListener(UseExtraCellBooster);
    }

    public void Initialize()
    {
        _unlockBoosterUsed = false;
        _extraCellBoosterUsed = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        SaveManager.Load();
        unlockBoosterText.text = SaveManager.Data.unlockBoostersCount.ToString();
        extraCellBoosterText.text = SaveManager.Data.extraCellBoostersCount.ToString();
        
        unlockBoosterButton.interactable = !_unlockBoosterUsed;
        extraCellBoosterButton.interactable = !_extraCellBoosterUsed;
    }
    
    private void UseUnlockBooster()
    {
        var invisibleUnits = boardView.Cells.Where(x => x.Occupant != null && !x.Occupant.GetVisibility()).ToList();
        
        if (!invisibleUnits.Any()) return;
        
        invisibleUnits.ForEach(x => x.Occupant.SetVisibility(true));
            
        SaveManager.Load();
        if (SaveManager.Data.unlockBoostersCount == 0) return;
        
        SaveManager.Data.unlockBoostersCount--;
        SaveManager.Save();
        
        _unlockBoosterUsed = true;
        UpdateUI();
    }
    
    private void UseExtraCellBooster()
    {
        SaveManager.Load();
        if (SaveManager.Data.extraCellBoostersCount == 0) return;
        
        SaveManager.Data.extraCellBoostersCount--;
        SaveManager.Save();
        
        unusedLettersPool.AddExtraCell();
        
        _extraCellBoosterUsed = true;
        UpdateUI();
    }
}
