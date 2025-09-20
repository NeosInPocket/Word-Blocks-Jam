using UnityEngine;
using UnityEngine.AI;

public class LetterUnit : LetterUnitBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LetterUnitRenderer renderer;
    [SerializeField] private LetterUnitBlinkRenderer blinkRenderer;

    public void DisableNavMesh()
    {
        navigation.Enabled = false;
    }
    
    public override bool Available
    {
        get => _available;
        set
        {
            _available = value;
            renderer.SetCharacterAvailability(value);
        }
    }

    protected override void SetCharacterData(BoardEntityData data)
    {
        renderer.SetCharacter(data.character, data.color);
        blinkRenderer.Initialize(data.color);
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        renderer.SetCharacterVisibility(value);
    }

    public void Blink(bool darken)
    {
        blinkRenderer.Blink(darken);
    }

    public void EnableVFXFeedback()
    {
        renderer.EnableFeedback();
    }
}
