public class LetterPoolData
{
    private LetterUnitBase _unit;
    public LetterUnitBase Occupant => _unit;
    
    public void SetOccupant(LetterUnitBase unit)
    {
        _unit = unit;
    }

    public void RemoveOccupant()
    {
        _unit = null;
    }
}