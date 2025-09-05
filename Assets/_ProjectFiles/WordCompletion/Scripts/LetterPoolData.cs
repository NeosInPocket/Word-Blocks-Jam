public class LetterPoolData
{
    private LetterUnit _unit;
    public LetterUnit Occupant => _unit;
    
    public void SetOccupant(LetterUnit unit)
    {
        _unit = unit;
    }

    public void RemoveOccupant()
    {
        _unit = null;
    }
}