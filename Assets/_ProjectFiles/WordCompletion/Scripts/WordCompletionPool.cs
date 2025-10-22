using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class WordCompletionPool : UnusedLettersPoolView
{
    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject loseObject;
    [SerializeField] private UnusedLettersPoolView unusedLettersPool;
    [SerializeField] private LevelBootstrap levelBootstrap;
    [SerializeField] private float poolAnimationDuration;
    [SerializeField] private float maxModelSize;
    [SerializeField] private float minModelSize;
    [SerializeField] private float threeWordsScale;
    [SerializeField] private float modelSizePerUnitSize;
    [SerializeField] private float modelDisappearOffset;
    [SerializeField] private float letterUnitExtraY;
    [SerializeField] private int buildDelay;
    [SerializeField] private int modelDestroyDelay;
    private WordsQueue _queue;
    private bool _isBuildingPool;
    public bool IsBuildingPool => _isBuildingPool;

    public void BuildPool(WordsQueue wordsQueue)
    {
        _queue = wordsQueue;
        BuildPool(wordsQueue.CurrentWord.Length);
        _model.AddButtons();
        _model.CanvasLetters.SetWord(wordsQueue.CurrentWord);

        var size = modelSizePerUnitSize * boardView.PreferredCellSize;
        
        if (size > maxModelSize)
        {
            size = maxModelSize;
        }

        if (size < minModelSize)
        {
            size = minModelSize;
        }
        
        _model.transform.localScale = size * Vector3.one;
    }

    public bool HasNonFilledCharacter(string character)
    {
        var containsCharacter = _queue.CurrentWord.Contains(character);
        if (!containsCharacter) return false;
        
        var characterCount = _queue.CurrentWord.Count(x => x.ToString() == character);
        var cells = Cells.Where(x => x.Occupant != null && x.Occupant.Character == character).Select(x => x.Occupant.Character).ToList();

        var result =  cells.Count != characterCount;
        return result;
    }

    public async UniTask SetCharacter(LetterUnitBase character, bool findPath = true)
    {
        character.GetComponent<LetterUnit>().EnableNavMesh();
        List<int> characterPositions = new List<int>();

        var word = _queue.CurrentWord;
        
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i].ToString() == character.Character)
            {
                characterPositions.Add(i);
            }
        }

        int freePosition = 0;
        
        foreach (var pos in characterPositions)
        {
            if (Cells[pos].Occupant != null && Cells[pos].Occupant.Character == character.Character) continue;

            freePosition = pos;
            break;
        }
        
        var cell = Cells[freePosition];
        cell.SetOccupant(character.GetComponent<LetterUnit>());

        if (findPath)
        {
            var gridPath = GetLetterUnitPathGrid(character.PositionData.position);
            var position = _model.SpawnPositions[freePosition].position;
            position.y += letterUnitExtraY;
            
            await character.Navigation.SetTarget(position, true,
                _model.Buttons[freePosition], gridPath);
        }
        else
        {
            var position = _model.SpawnPositions[freePosition].position;
            position.y += letterUnitExtraY;
            await character.Navigation.SetTarget(position, true,
                _model.Buttons[freePosition], null);
        }
        
        _model.Feedbacks[freePosition].SetActive(true);
        
        var cellWithOccupants = Cells.Where(x => x.Occupant != null).ToList();
        var someAreNotInPlace = cellWithOccupants.Any(x => !x.Occupant.Navigation.InFinalPlace);
        var missingLetters = cellWithOccupants.Count != _queue.CurrentWord.Length;
        
        if (missingLetters || someAreNotInPlace)
        {
            CheckLose();
            return;
        }

        if (TryCompleteWord())
        {
            if (_queue.MoveNext())
            {
                AudioManager.Instance.PlayWordCompleted();
                
                
                await BuildNextPool();

                var lettersInPool = unusedLettersPool.Cells.Where(x => x.Occupant != null)
                    .Select(x => x.Occupant.Character).ToList();
                var matchingCharacters = _queue.CurrentWord.Where(x => lettersInPool.Contains(x.ToString())).ToList();

                if (matchingCharacters.Any())
                {
                    List<UniTask> tasks = new();
                    
                    foreach (var ch in matchingCharacters)
                    {
                        var poolCell = unusedLettersPool.Cells.FirstOrDefault(x =>
                            x.Occupant != null && x.Occupant.Character == ch.ToString());
                        if (poolCell == null) continue;

                        var task = SetCharacter(poolCell.Occupant, false);
                        tasks.Add(task);
                        
                        poolCell.RemoveOccupant();
                    }

                    await UniTask.WhenAll(tasks);
                }
            }
            else
            {
                AudioManager.Instance.PlayWin();
                
                levelBootstrap.OnLevelComplete();
                winObject.SetActive(true);
                tracker.Enable(false);
                SaveManager.Load();
                SaveManager.Data.levelsCompleted++;
                SaveManager.Save();
            }
        }
        
        CheckLose();
    }

    public async UniTask CheckLose()
    {
        Debug.Log("lose check");
        
        if (unusedLettersPool.IsFull)
        {
            var unusedPoolUnits = unusedLettersPool.Cells.Where(x => x.Occupant != null);
            var mainPoolUnits = Cells.Where(x => x.Occupant != null);
            
            var allUnits = unusedPoolUnits.Concat(mainPoolUnits).Select(x => x.Occupant).ToList();
            
            await WaitForAllMovingUnits(allUnits);
            while (_isBuildingPool)
            {
                await UniTask.Yield();
            }
            
            if (boardView.Cells.All(x => x.Occupant == null))
            {
                return;
            }
            
            var filledLettersCount = Cells
                .Where(x => x.Occupant != null)
                .GroupBy(x => x.Occupant.Character)
                .ToDictionary(g => g.Key, g => g.Count());

            var wordLettersCount = _queue.CurrentWord
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            var missingLetters = new List<string>();

            foreach (var kvp in wordLettersCount)
            {
                char letter = kvp.Key;
                int requiredCount = kvp.Value;

                int filledCount = filledLettersCount.ContainsKey(letter.ToString()) ? filledLettersCount[letter.ToString()] : 0;

                int missingCount = requiredCount - filledCount;
                if (missingCount > 0)
                {
                    missingLetters.AddRange(Enumerable.Repeat(letter.ToString(), missingCount));
                }
            }
            
            if (missingLetters.Any())
            {
                var availableBoardLetters = boardView.Cells.Where(x => x.Occupant != null && x.Occupant.Available).ToList();
                var matchingCharacters = availableBoardLetters.Where(x => missingLetters.Contains(x.Occupant.Character)).ToList();
                var isLose = !matchingCharacters.Any();

                if (isLose)
                {
                    loseObject.SetActive(true);
                    tracker.Enable(false);
                    
                    LevelTimer.Instance.FinishLevel();
                    AdjustEventManager.Instance.TrackLevelFinish("lmf6ne", LevelSelection.Instance.AnalyticsLevelNumber, $"Level {LevelSelection.Instance.AnalyticsLevelNumber}", "Completed", LevelTimer.Instance.GetLevelTime());
                }
            }
        }
    }

    private async UniTask WaitForAllMovingUnits(List<LetterUnit> units)
    {
        while (units.Any(x => !x.Navigation.InFinalPlace))
        {
            await UniTask.Yield();
        }
    }
    
    private async UniTask BuildNextPool()
    {
        _isBuildingPool = true;
            //tracker.Enable(false);
        
        await UniTask.Delay(buildDelay);
        
        var occupants = Cells.Select(x => x.Occupant).ToList();
            
        foreach (var unit in occupants)
        {
            unit.DisableNavMesh();
            unit.transform.SetParent(_model.transform);
            unit.EnableVFXFeedback();
        }

        foreach (var button in _model.Buttons)
        {
            button.StopMove();
        }

        var oldModel = _model;
        
        AudioManager.Instance.PlayNewWord();
        
        await _model.transform
            .DOMoveX(_model.transform.position.x - modelDisappearOffset, poolAnimationDuration)
            .SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        
        DestroyPoolDelayed(oldModel.gameObject);
        BuildPool(_queue);
        
        var modelOffsetPosition = _model.transform.position;
        var modelStartPosition = modelOffsetPosition;
        modelOffsetPosition.x += Camera.main.orthographicSize;
        
        _model.transform.position = modelOffsetPosition;
        
        AudioManager.Instance.PlayNewWord();
        
        await _model.transform.DOMove(modelStartPosition, poolAnimationDuration).SetEase(Ease.InOutQuad)
            .AsyncWaitForCompletion();
        
        tracker.Enable(true);
        
        _isBuildingPool = false;
    }

    private async UniTask DestroyPoolDelayed(GameObject pool)
    {
        await Task.Delay(modelDestroyDelay);
        Destroy(pool);
    }
    
    private bool TryCompleteWord()
    {
        if (_queue.CurrentWord == null) return false;
        
        string word = String.Empty;
        
        foreach (var cell in Cells)
        {
            if (cell.Occupant == null) continue;
            word += cell.Occupant.Character;
        }
        
        return _queue.CurrentWord.Equals(word);
    }
}
