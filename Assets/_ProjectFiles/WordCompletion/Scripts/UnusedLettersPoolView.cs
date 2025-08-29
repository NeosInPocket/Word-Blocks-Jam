using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnusedLettersPoolView : BoardSpawner<LetterCell>
{
    public List<LetterCell> Cells => _cells;

    public bool TryAdd(LetterUnitBase unit)
    {
        var freeCell = _cells.FirstOrDefault(x => x.Occupant == null);

        if (freeCell != null)
        {
            freeCell.SetOccupant(unit);
            return true;
        }
        
        return false;
    }
}
