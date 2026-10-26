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
    private bool isCombo;

    [Inject]
    public MoveCommand(
        Battlefield battlefield,
        BattleController battleController,
        PlayerController playerController,
        PlaceholderFactory<SelectCheckerCommand> selectCommandFactory,
        Cell selectedCell,
        List<Cell> allowedCells,
        [InjectOptional] bool isCombo = false)
    {
        this._battlefield = battlefield;
        this._battleController = battleController;
        this._playerController = playerController;
        this._selectCommandFactory = selectCommandFactory;
        this.selectedCell = selectedCell;
        this.allowedCells = allowedCells;
        this.isCombo= isCombo;
    }

    public void Interact(Cell targetCell)
    {
        if (targetCell.IsOccupied && targetCell.CurrentChecker.checkerTeam == _battleController.CurrentTurn) 
        {
            if(isCombo)
            {
                Debug.LogWarning("Завершите серию сруба");
                return;
            }
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
            if(isCombo)
            {
                Debug.Log("Выберите правильную шашку");
                return;
            }
            _battlefield.ClearAllHighlights();
            _battleController.SetCommand(_selectCommandFactory.Create());
        }        
    }
}
