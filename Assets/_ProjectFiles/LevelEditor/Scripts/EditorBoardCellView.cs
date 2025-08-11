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
        _data = data;
        
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
    
        _occupant = _factory.Create<EditorLetterUnit>(data);
        _occupant.transform.position = SpawnPositionTransform.position;
        
        var editorLetter = _occupant.GetComponent<EditorLetterUnit>();
        editorLetter.Color = data.color;
    }

    public void SetColor(Color color)
    {
        var renderer = GetComponent<Renderer>();
        var mpb = new MaterialPropertyBlock();
        
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);

        renderer.SetPropertyBlock(mpb);
    }
}