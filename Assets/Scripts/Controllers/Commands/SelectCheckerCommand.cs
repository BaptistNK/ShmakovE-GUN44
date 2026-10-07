using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCheckerCommand : IGameplayCommand
{
    private Battlefield battlefield;
    private BattleController battleController;
    private PlayerController playerController;
    private List<Cell> validTargetCells = new List<Cell>();
    public SelectCheckerCommand(Battlefield _battlefield, BattleController _battleController, PlayerController _playerController)
    {
        this.battlefield = _battlefield;
        this.battleController = _battleController;
        this.playerController = _playerController;
    }
    public void Interact(Cell targetCell)
    {
        if (battlefield == null) return;
       
        battlefield.ClearAllHighlights();
        validTargetCells.Clear();
      
        if (!targetCell.IsOccupied || targetCell.CurrentChecker.checkerTeam != battleController.CurrentTurn) 
        {
            return;
        }
        Checker selectedChecker = targetCell.CurrentChecker;
        battlefield.HighlightCell(targetCell, true, Color.red);

        Vector2Int[] allDirections = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
        };

       foreach(Vector2Int dir in allDirections)
        {
            Cell neighbor = battlefield.GetCellInDirection(targetCell,dir);
            if(neighbor ==null) continue;
            bool isMovingForward = (selectedChecker.checkerTeam == Team.White && dir.y > 0) ||
                                   (selectedChecker.checkerTeam == Team.Black && dir.y < 0); 
            if (neighbor.IsOccupied)
            {
                if(neighbor.CurrentChecker.checkerTeam!=selectedChecker.checkerTeam)
                {
                    Cell jumpCell = battlefield.GetCellInDirection(neighbor, dir);
                        if (jumpCell != null && !jumpCell.IsOccupied) 
                    {
                        battlefield.HighlightCell(jumpCell, true, Color.green);
                        validTargetCells.Add(jumpCell);
                    }
                }
            }
            else
            {
                if(isMovingForward)
                {
                    battlefield.HighlightCell(neighbor,true,Color.green);
                    validTargetCells.Add(neighbor);
                }
            }
        }
        if (validTargetCells.Count > 0)
        {
            battleController.SetCommand(new MoveCommand(battlefield, battleController, playerController, targetCell, validTargetCells));
        }
    }

    
}
