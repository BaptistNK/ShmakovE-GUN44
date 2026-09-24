using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class CheckersMoveCommand : IGameplayCommand
{
    private readonly Battlefield _battlefield;
    private readonly PlayerController _playerController;
    private readonly BattleController _battleController;

    private Cell _selectedCell;
    private List<Cell> _highlightedMoveCells = new List<Cell>();
    private List<Cell> _highlightedAttackCells = new List<Cell>();
    private Dictionary<Cell, Cell> _attackTargetsMap = new Dictionary<Cell, Cell>();

    [Inject]

    public CheckersMoveCommand(Battlefield battlefield, PlayerController playerController, BattleController battleController)
    {
        _battleController = battleController;
        _playerController = playerController;
        _battlefield = battlefield;
    }

    public void Cancel()
    {
        ClearHighlights();
    }

    public void Interact(Cell clickedCell)
    {
        if(clickedCell.CurrentUnit != null && clickedCell.CurrentUnit.Team == _battleController.CurrentTurn)
        {
            SelectUnit(clickedCell);
            return;
        }
        if(_selectedCell!=null)
        {
            if(_highlightedMoveCells.Contains(clickedCell))
            {
                Cell source = _selectedCell;
                ClearHighlights();
                _playerController.ExecuteMove(source, clickedCell, null);
            }
            else if (_highlightedAttackCells.Contains(clickedCell))
            {
                Cell source = _selectedCell;
                Cell killedCell = _attackTargetsMap[clickedCell];
                ClearHighlights();
                _playerController.ExecuteMove(source, clickedCell, killedCell);
            }
        }
    }

   private void SelectUnit(Cell cell)
    {
        ClearHighlights();
        _selectedCell = cell;
        _selectedCell.SetHighlight(CellHighlightState.Selected);
        CalculateAvailableMoves(cell);
    }

    private void ClearHighlights()
    {
        if (_selectedCell != null) _selectedCell.SetHighlight(CellHighlightState.None);
        foreach (Cell cell in _highlightedMoveCells) cell.SetHighlight(CellHighlightState.None);
        foreach (Cell cell in _highlightedAttackCells) cell.SetHighlight(CellHighlightState.None);

        _selectedCell = null;
        _highlightedMoveCells.Clear();
        _highlightedAttackCells.Clear();
        _attackTargetsMap.Clear();
    }

    public void CalculateAvailableMoves(Cell sourceCell)
    {
        Unit unit = sourceCell.CurrentUnit;
        Vector2Int currentCoords = sourceCell.Coordinates;

        Vector2Int[] diagonals = new Vector2Int[]
        {
            new Vector2Int(1,1),
            new Vector2Int(-1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,-1)
        };
        foreach (Vector2Int dir in diagonals)
        {
            if(unit.Type==UnitType.Pawn)
            {
                bool isForward = (unit.Team == Team.White && dir.y > 0) || (unit.Team == Team.White && dir.y < 0);
                Vector2Int targetCoords = currentCoords + dir;
                Cell targetCell = _battlefield.GetCell(targetCoords);
                if(targetCell!=null)
                {
                    if(targetCell.CurrentUnit == null)
                    {
                        if(isForward)
                        {
                            _highlightedMoveCells.Add(targetCell);
                            targetCell.SetHighlight(CellHighlightState.CanMove);
                        }
                    }
                    else if (targetCell.CurrentUnit.Team !=unit.Team)
                    {
                        Vector2Int jumpCoords = targetCoords + dir;
                        Cell jumpCell = _battlefield.GetCell(jumpCoords);
                        if(jumpCell!=null && jumpCell.CurrentUnit==null)
                        {
                            _highlightedAttackCells.Add(jumpCell);
                            _attackTargetsMap[jumpCell] = targetCell;
                            jumpCell.SetHighlight(CellHighlightState.CanAttack);
                        }
                    }
                }
            }
            else if (unit.Type == UnitType.King)
            {
                Vector2Int scanCoords = currentCoords + dir;
                Cell enemyOnLine = null;
                while(true)
                {
                    Cell scanCell = _battlefield.GetCell(scanCoords);
                    if (scanCell == null) break;
                    if (enemyOnLine == null)
                    {
                        if(scanCell.CurrentUnit==null)
                        {
                            _highlightedMoveCells.Add(scanCell);
                            scanCell.SetHighlight(CellHighlightState.CanMove);
                        }
                        else if(scanCell.CurrentUnit.Team==unit.Team)
                        {
                            break;
                        }
                        else
                        {
                            enemyOnLine = scanCell;
                        }
                    }
                    else
                    {
                        if(scanCell.CurrentUnit==null)
                        {
                            _highlightedAttackCells.Add(scanCell);
                            _attackTargetsMap[scanCell] = enemyOnLine;
                            scanCell.SetHighlight(CellHighlightState.CanAttack);
                        }
                        else
                        {
                            break;
                        }
                    }
                    scanCoords += dir;
                }
            }
        }
    }

}
