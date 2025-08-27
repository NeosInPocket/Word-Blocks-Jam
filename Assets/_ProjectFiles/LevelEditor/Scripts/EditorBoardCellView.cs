using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorBoardCellView : LetterCell
{
    [SerializeField] private LetterUnitBase letterUnitPrefab;
    [SerializeField] private float unitSizePerCellSize = 0.7f;
    private LetterUnitFactory _factory;
    public Transform SpawnPositionTransform => spawnTransform;

    private void Start()
    {
        _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        
        #if UNITY_EDITOR
        if (!LevelEditingToggleState.State) return;
        Selection.activeGameObject = null;
        #endif
    }
    
    private void Update()
    {
        #if UNITY_EDITOR
        if (!LevelEditingToggleState.State) return;
        
        if (Selection.activeGameObject == gameObject)
        {
            Selection.activeGameObject = null;
        }
        #endif
    }
    
    public void SetData(BoardEntityData data)
    {
        _data = data;
        
        if (_factory == null)
        {
            _factory = new LetterUnitFactory(letterUnitPrefab, transform);
        }
    
        _occupant = _factory.Create<EditorLetterUnit>(data, unitSizePerCellSize * SideSize);
        _occupant.transform.position = SpawnPositionTransform.position;
        
        var editorLetter = _occupant.GetComponent<EditorLetterUnit>();

        if (data.color.a == 0)
        {
            data.color.a = 1f;
        }
        
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