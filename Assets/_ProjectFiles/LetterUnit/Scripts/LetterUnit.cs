using UnityEngine;
using UnityEngine.AI;

public class LetterUnit : LetterUnitBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LetterUnitRenderer renderer;
    [SerializeField] private CharacterEffect characterPopEffect;
    public CharacterEffect PopEffect => characterPopEffect;
    
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
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        renderer.SetCharacterVisibility(value);
    }
    
    public void SetAvoidancePriority(int priority)
    {
        //agent.avoidancePriority = priority;
    } 
}
