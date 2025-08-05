using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorBoardCellView : LetterCell
{
    [SerializeField] private LetterUnitBase letterUnitPrefab;
    private LetterUnitFactory _factory;
    public Transform SpawnPositionTransform => spawnTransform;

    private void Start()
    {
        _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        if (!LevelEditingToggleState.State) return;
        Selection.activeGameObject = null;
    }
    
    private void Update()
    {
        if (!LevelEditingToggleState.State) return;
        
        if (Selection.activeGameObject == gameObject)
        {
            Selection.activeGameObject = null;
        }
    }
    
    public void SetData(BoardEntityData data)
    {
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
    
        _occupant = _factory.Create<EditorLetterUnit>(data);
        _occupant.transform.position = SpawnPositionTransform.position;
    }
}