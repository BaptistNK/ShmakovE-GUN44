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
    private Cell pendingTargetCell = null;
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
            if (pendingTargetCell != null && pendingTargetCell != targetCell)
            {
                _battlefield.HighlightCell(pendingTargetCell, true, Color.green);
            }

            pendingTargetCell = targetCell;
            _battlefield.HighlightCell(targetCell, true, Color.yellow);
            Debug.Log($"[MoveCommand] Выбрана клетка {targetCell.Coordinates}. Нажмите SPACE для подтверждения или ESC для отмены.");

        }
    }
    public void ExecuteConfirmedMove()
    {
        if (pendingTargetCell == null)
        {
            Debug.LogWarning("[MoveCommand] Нельзя подтвердить ход: клетка не выбрана кликом мыши!");
            return;
        }

        Checker checkerToMove = selectedCell.CurrentChecker;
        _battlefield.ClearAllHighlights();

        Checker checkerToRemove = null;
        bool didCapture = false;

        // Лучевой поиск срубленной фигуры (ваш рабочий алгоритм для дамок и шашек)
        Vector2Int stepDir = new Vector2Int(
            System.Math.Sign(pendingTargetCell.Coordinates.x - selectedCell.Coordinates.x),
            System.Math.Sign(pendingTargetCell.Coordinates.y - selectedCell.Coordinates.y)
        );

        Cell checkCell = _battlefield.GetCellInDirection(selectedCell, stepDir);

        while (checkCell != null && checkCell != pendingTargetCell)
        {
            if (checkCell.IsOccupied)
            {
                checkerToRemove = checkCell.CurrentChecker;
                checkCell.SetChecker(null);
                didCapture = true;
                break;
            }
            checkCell = _battlefield.GetCellInDirection(checkCell, stepDir);
        }

        // Запускаем физическое движение
        _playerController.MoveChecker(checkerToMove, pendingTargetCell, _battleController, _battlefield, checkerToRemove, didCapture);
    }
}
