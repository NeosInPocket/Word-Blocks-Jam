using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private List<string> tutorialReplicas;
    [SerializeField] private BoardView boardView;
    [SerializeField] private PlayerSelectionTracker playerSelectionTracker;
    [SerializeField] private float handMoveAmplitude;
    [SerializeField] private float handMoveFreq;
    [SerializeField] private Ease handMoveEase;

    private WordsQueue _wordsQueue;
    private List<char> _savedCharacters;
    private int _characterIndex;
    private LetterUnit _currentUnit;
    private Tween moveTween;

    public void StartTutorial(WordsQueue wordsQueue)
    {
        gameObject.SetActive(true);
        _characterIndex = 0;
        _wordsQueue = wordsQueue;
        playerSelectionTracker.TutorialCellTapped += TutorialCellTapped;
        MoveNext();
    }

    private void MoveNext()
    {
        _currentUnit = SelectNextUnit();
        if (_currentUnit == null)
        {
            playerSelectionTracker.TutorialCellTapped = null;
            
            SaveManager.Load();
            SaveManager.Data.tutorialCompleted = true;
            SaveManager.Save();
            
            gameObject.SetActive(false);
            return;
        }
        
        MoveUIToWorldTarget(_currentUnit.transform);
    }
    
    private void TutorialCellTapped(BoardCellView obj)
    {
        if (obj.Occupant == null || obj.Occupant != _currentUnit) return;
        
        StopAllCoroutines();
        MoveNext();
        playerSelectionTracker.ProcessCellTapped(obj);
    }

    private LetterUnit SelectNextUnit()
    {
        if (_characterIndex >= _wordsQueue.CurrentWord.Length)
            return null;
        
        var currentWord = _wordsQueue.CurrentWord;
        var character = currentWord[_characterIndex].ToString();

        var unit = boardView.Cells.First(x => x.Occupant != null && x.Occupant.Character == character).Occupant.GetComponent<LetterUnit>();
        _characterIndex++;
        return unit;
    }

    private void HandAnimation()
    {
        Vector2 delta = Vector2.one * handMoveAmplitude;
        delta.x *= -1;
        
        var finalAnchorPosition = uiElement.anchoredPosition - delta;

        if (moveTween != null)
        {
            moveTween.Kill();
        }
        
        moveTween = uiElement.DOAnchorPos3D(finalAnchorPosition, handMoveFreq).SetEase(handMoveEase).SetLoops(-1, LoopType.Yoyo);
    }
     
    void MoveUIToWorldTarget(Transform worldTarget)
    {
        if (mainCamera == null || uiElement == null || worldTarget == null || canvas == null)
            return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldTarget.position);
        screenPos.z = 0;
        
        uiElement.anchoredPosition = screenPos;
        HandAnimation();
    }
}
