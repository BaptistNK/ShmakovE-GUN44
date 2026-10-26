using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectCheckerCommand : IGameplayCommand
{
    private List<Cell> validTargetCells = new List<Cell>();
    private List<Cell> forcedInputCells;

    private Battlefield battlefield;
    private BattleController battleController;
    private PlayerController playerController;
    private DiContainer container;
    [Inject]
    public SelectCheckerCommand(Battlefield battlefield, BattleController battleController, PlayerController playerController, DiContainer container)
    {
        this.battlefield = battlefield;
        this.battleController = battleController;
        this.playerController = playerController;
        this.container = container;
    }
    public void Interact(Cell targetCell)
    {
        if (battlefield == null || battleController == null) return;
        
        battlefield.ClearAllHighlights();
        validTargetCells.Clear();
      
        if (!targetCell.IsOccupied || targetCell.CurrentChecker == null || targetCell.CurrentChecker.checkerTeam != battleController.CurrentTurn) 
        {
            Debug.Log("[SelectCheckerCommand] Клик проигнорирован: ячейка пуста или это чужая команда.");
            return;
        }

        if (battleController.forcedCells != null && battleController.forcedCells.Count > 0) 
        {
            if (!battleController.forcedCells.Contains(targetCell))
            {
                Debug.LogWarning($"[Правила] Клик ЗАБЛОКИРОВАН! Вы нажали на {targetCell.Coordinates}, но должны рубить одной из {battleController.forcedCells.Count} фишек.");
                return;
            }
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
            var moveCmd = container.Instantiate<MoveCommand>(new object[] { targetCell, validTargetCells });
            battleController.SetCommand(moveCmd);
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
            if (checker.IsKing)
            {
                Cell nextCell = field.GetCellInDirection(fromCell, dir);
                Checker firstFoundChecker = null;

                while (nextCell != null)
                {
                    if (nextCell.IsOccupied)
                    {
                        if (firstFoundChecker != null) break; 
                        if (nextCell.CurrentChecker == null)
                        {
                            break; 
                        }

                        firstFoundChecker = nextCell.CurrentChecker;
                    }
                    else
                    {
                        if (firstFoundChecker != null)
                        {
                            if (firstFoundChecker.checkerTeam != currentTeam)
                            {
                                jumps.Add(nextCell); 
                            }
                            else
                            {
                                break; 
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
