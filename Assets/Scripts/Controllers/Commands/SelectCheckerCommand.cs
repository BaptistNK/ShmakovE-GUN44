using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        List<Cell> jumpCells = GetValidJumps(targetCell, battlefield, battleController.CurrentTurn);

        if (jumpCells.Count > 0)
        {
            validTargetCells.AddRange(jumpCells);
            foreach (Cell cell in jumpCells)
            {
                battlefield.HighlightCell(cell, true, Color.green);
            }
        }
        else
        {
            Vector2Int[] allDirections = new Vector2Int[]
            {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
            };

            //если выбрана дамка
            if (selectedChecker.IsKing)
            {
                foreach (Vector2Int dir in allDirections)
                {
                    Cell nextCell = battlefield.GetCellInDirection(targetCell, dir);
                    while (nextCell != null && !nextCell.IsOccupied)
                    {
                        battlefield.HighlightCell(nextCell, true, Color.green);
                        validTargetCells.Add(nextCell);
                        nextCell = battlefield.GetCellInDirection(nextCell, dir);
                    }
                }
            }
            else
            {
                foreach (Vector2Int dir in allDirections)
                {
                    Cell neighbor = battlefield.GetCellInDirection(targetCell, dir);
                    if (neighbor == null) continue;
                    bool isMovingForward = (selectedChecker.checkerTeam == Team.White && dir.y > 0) ||
                                           (selectedChecker.checkerTeam == Team.Black && dir.y < 0);
                    if (neighbor.IsOccupied)
                    {
                        if (neighbor.CurrentChecker.checkerTeam != selectedChecker.checkerTeam)
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
                        if (isMovingForward)
                        {
                            battlefield.HighlightCell(neighbor, true, Color.green);
                            validTargetCells.Add(neighbor);
                        }
                    }
                }
            }
        }
        if (validTargetCells.Count > 0)
        {
            battleController.SetCommand(new MoveCommand(battlefield, battleController, playerController, targetCell, validTargetCells));
        }
    }
    public static List<Cell> GetValidJumps(Cell fromCell, Battlefield field, Team currentTeam)
    {
        List<Cell> jumps = new List<Cell>();

        if (field == null || fromCell == null) return jumps;

        Checker checker = fromCell.CurrentChecker;
        Vector2Int[] allDirections = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
        };
        foreach (Vector2Int dir in allDirections)
        {
            //условия для дамки
            if (checker.IsKing)
            {
                Cell nextCell = field.GetCellInDirection(fromCell, dir);
                Checker firstFoundChecker = null;

                while (nextCell != null)
                {
                    if (nextCell.IsOccupied)
                    {
                        if (firstFoundChecker != null) break; // Две фигуры подряд перепрыгнуть нельзя

                        // Защита: проверяем, что шашка физически существует на клетке
                        if (nextCell.CurrentChecker == null)
                        {
                            break; // Сбой данных, прерываем этот луч
                        }

                        firstFoundChecker = nextCell.CurrentChecker;
                    }
                    else
                    {
                        // Клетка пустая. Проверяем, нашли ли мы врага ранее на этой линии
                        // ИСПРАВЛЕНО: Добавлена жесткая проверка, что firstFoundChecker ФИЗИЧЕСКИ существует в памяти
                        if (firstFoundChecker != null)
                        {
                            if (firstFoundChecker.checkerTeam != currentTeam)
                            {
                                jumps.Add(nextCell); // Дамка может прыгнуть на любую из пустых клеток ЗА врагом
                            }
                            else
                            {
                                break; // Свою фигуру рубить нельзя, прерываем луч
                            }
                        }
                    }
                    nextCell = field.GetCellInDirection(nextCell, dir);
                }
            }
            Cell neighbor = field.GetCellInDirection(fromCell, dir);

            if (neighbor == null || !neighbor.IsOccupied || neighbor.CurrentChecker == null)
            {
                continue;
            }

            if (neighbor.CurrentChecker.checkerTeam != currentTeam) 
            {
                Cell jumpCell = field.GetCellInDirection(neighbor, dir);
                if(jumpCell!=null && !jumpCell.IsOccupied)
                {
                    jumps.Add(jumpCell);
                }
            }
        }
        return jumps;
    }    
}
