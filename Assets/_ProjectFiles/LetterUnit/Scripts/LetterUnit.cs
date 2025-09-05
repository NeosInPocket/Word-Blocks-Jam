using UnityEngine;
using UnityEngine.AI;

public class LetterUnit : LetterUnitBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LetterUnitCharacterRenderer characterRenderer;
    [SerializeField] private CharacterEffect characterPopEffect;
    public CharacterEffect PopEffect => characterPopEffect;
    
    public override bool Available
    {
        get => _available;
        set => _available = value;
    }

    protected override void SetCharacterData(BoardEntityData data)
    {
        characterRenderer.SetCharacter(data.character);
        characterRenderer.SetUnitColor(data.color);
    }

    public override void SetVisibility(bool value)
    {
        _visible = value;
        characterRenderer.SetCharacterVisibility(value);
    }
    
    public void SetAvoidancePriority(int priority)
    {
        //agent.avoidancePriority = priority;
    } 
}
