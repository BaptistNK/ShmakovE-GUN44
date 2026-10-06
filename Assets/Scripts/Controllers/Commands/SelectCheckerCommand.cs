using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCheckerCommand : IGameplayCommand
{
    private Battlefield battlefield;
    private BattleController battleController;
    private PlayerController playerController;
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
        //проверка на выделенную клетку        
        if (!targetCell.IsOccupied)
        {
            return;
        }
        //проверка: принадлежит шашка текущему игроку?
        if (targetCell.CurrentChecker.checkerTeam != battleController.CurrentTurn) 
        {
            Debug.Log($"Сейчас ход {battleController.CurrentTurn}");
            return;
        }
        battlefield.HighlightCell(targetCell, true, Color.red);
        int validMovesCount = 0;

        foreach (Cell neighbor in targetCell.Neighbors)
        {
            if (!neighbor.IsOccupied)
            {
                battlefield.HighlightCell(neighbor, true, Color.green);
                validMovesCount++;
            }
        }
        bool hasValidMoves = validMovesCount > 0;
        if (hasValidMoves)
        {
            battleController.SetCommand(new MoveCommand(battlefield, battleController, playerController, targetCell));
            Debug.Log($"Шашка выбрана. Доступно ходов: {validMovesCount}");
        }
        else
        {
            Debug.Log("Шашка заблокирована. нет доступных ходов");
        }
    }

    
}
