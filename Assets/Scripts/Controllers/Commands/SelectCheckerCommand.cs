using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCheckerCommand : IGameplayCommand
{
    private Battlefield battlefield;

    public SelectCheckerCommand(Battlefield _battlefield)
    {
        this.battlefield = _battlefield;
    }
    public void Interact(Cell targetCell)
    {
        if (battlefield == null) return;
       
        battlefield.ClearAllHighlights();

        
        if (!targetCell.IsOccupied)
        {
            return;
        }
        
        battlefield.HighlightCell(targetCell, true, Color.red);

        foreach (Cell neighbor in targetCell.Neighbors)
        {
            if (!neighbor.IsOccupied)
            {
                battlefield.HighlightCell(neighbor, true, Color.green);
            }
        }
    }

    
}
