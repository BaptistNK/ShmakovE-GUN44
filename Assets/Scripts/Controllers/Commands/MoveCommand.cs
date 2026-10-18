using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoveCommand : IGameplayCommand
{
    private Battlefield _battlefield;
    private BattleController _battleController;
    private PlayerController _playerController;
    private PlaceholderFactory<SelectCheckerCommand> _selectCommandFactory;
    private Cell selectedCell;
    private List<Cell> allowedCells;

    [Inject]
    public MoveCommand(
        Battlefield battlefield,
        BattleController battleController,
        PlayerController playerController,
        PlaceholderFactory<SelectCheckerCommand> selectCommandFactory,
        Cell selectedCell,
        List<Cell> allowedCells)
    {
        this._battlefield = battlefield;
        this._battleController = battleController;
        this._playerController = playerController;
        this._selectCommandFactory = selectCommandFactory;
        this.selectedCell = selectedCell;
        this.allowedCells = allowedCells;
    }

    public void Interact(Cell targetCell)
    {
        //Сброс выбранной шашки при выборе другой
        if (targetCell.IsOccupied && targetCell.CurrentChecker.checkerTeam == _battleController.CurrentTurn) 
        {
            //Выбираем новую шашку
            var selectCmd = _selectCommandFactory.Create();
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
            _battleController.SetCommand(_selectCommandFactory.Create());
        }        
    }
}
