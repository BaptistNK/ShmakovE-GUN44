using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : IGameplayCommand
{
    private Battlefield _battlefield;
    private BattleController _battleController;
    private PlayerController _playerController;
    private Cell selectedCell;
    private List<Cell> allowedCells;
    public MoveCommand(Battlefield battlefield, BattleController battleController, PlayerController playerController, Cell selectedCell, List<Cell> allowedCells)
    {
        this._battlefield = battlefield;
        this._battleController = battleController;
        this.selectedCell = selectedCell;
        this._playerController = playerController;
        this.allowedCells = allowedCells;
    }

    public void Interact(Cell targetCell)
    {
        //Сброс выбранной шашки при выборе другой
        if (targetCell.IsOccupied && targetCell.CurrentChecker.checkerTeam == _battleController.CurrentTurn) 
        {
            //Выбираем новую шашку
            var selectCmd = new SelectCheckerCommand(_battlefield, _battleController, _playerController);
            _battleController.SetCommand(selectCmd);
            selectCmd.Interact(targetCell);
            return;
        }

        if(allowedCells.Contains(targetCell))
        {
            Checker checkerToMove = selectedCell.CurrentChecker;
            _battlefield.ClearAllHighlights();

            Checker checkerToRemove = null;
            bool didCapture = false;

            Vector2Int stepDir = new Vector2Int(
                System.Math.Sign(targetCell.Coordinates.x - selectedCell.Coordinates.x),
                System.Math.Sign(targetCell.Coordinates.y - selectedCell.Coordinates.y)
            );
            Cell checkCell = _battlefield.GetCellInDirection(selectedCell, stepDir);
            
            while (checkCell != null && checkCell != targetCell)
            {
                if (checkCell.IsOccupied)
                {
                    checkerToRemove = checkCell.CurrentChecker;

                    checkCell.SetChecker(null);

                    didCapture = true;
                }

                checkCell = _battlefield.GetCellInDirection(checkCell, stepDir);
            }
            
            _playerController.MoveChecker(checkerToMove, targetCell, _battleController, _battlefield, checkerToRemove, didCapture);
        }
        else
        {
            _battlefield.ClearAllHighlights();
            _battleController.SetCommand(new SelectCheckerCommand(_battlefield, _battleController, _playerController));
        }        
    }
}
