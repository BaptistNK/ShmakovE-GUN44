using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : IGameplayCommand
{
    private Battlefield _battlefield;
    private BattleController _battleController;
    private PlayerController _playerController;
    private Cell selectedCell;
    public MoveCommand(Battlefield battlefield, BattleController battleController, PlayerController playerController, Cell selectedCell)
    {
        this._battlefield = battlefield;
        this._battleController = battleController;
        this.selectedCell = selectedCell;
        this._playerController = playerController;
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

        if(selectedCell.Neighbors.Contains(targetCell)&&!targetCell.IsOccupied)
        {
            Checker checkerToMove = selectedCell.CurrentChecker;
            _battlefield.ClearAllHighlights();
            _playerController.MoveChecker(checkerToMove, targetCell, _battleController, _battlefield);
        }
        else
        {
            Debug.Log("Ход сброшен");
            _battlefield.ClearAllHighlights();
            _battleController.SetCommand(new SelectCheckerCommand(_battlefield, _battleController, _playerController));

        }
    }
}
