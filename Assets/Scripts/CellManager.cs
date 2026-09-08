using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public event Action<GameObject> OnCellClicked;
    private Cell[] allCells;
    private Unit[] allUnits;
    [SerializeField] private float cellSize = 1.0f;
    private const float Tolerance = 0.1f;

    private void Start()
    {
        allCells = FindObjectsByType<Cell>(FindObjectsSortMode.None);

        foreach (Cell cell in allCells)
        {
            cell.OnPointerClickEvent += HandlePointerClick;
        }
        InitializeCells();
        InitializeUnits();
        
    }

    private void HandlePointerClick(GameObject clickedCell)
    {
        OnCellClicked?.Invoke(clickedCell);
    }

    private void OnDestroy()
    {
        if (allCells == null) return;
        foreach (Cell cell in allCells)
        {
            if (cell != null)
            {
                cell.OnPointerClickEvent -= HandlePointerClick;
            }
        }
    }
    private void InitializeCells()
    {
        foreach (Cell _currentCell in allCells)
        {
            _currentCell.OnPointerClickEvent += HandlePointerClick;
            NeighbourType currentMask = NeighbourType.None;
            foreach (Cell potentialNeighbour in allCells)
            {
                if (_currentCell == potentialNeighbour) continue;
                Vector3 direction = potentialNeighbour.transform.position - _currentCell.transform.position;
                float distance = direction.magnitude;
                // Проверяем, находится ли клетка вплотную (на расстоянии одного шага сетки)
                if (Mathf.Abs(distance - cellSize) < Tolerance)
                {
                    // Определяем сторону света на основе координат (в плоскости XZ)
                    if (Mathf.Abs(direction.z) < Tolerance) // Клетки на одной линии по Z, значит соседа ищем по X
                    {
                        if (direction.x > 0) currentMask |= NeighbourType.Right;
                        else if (direction.x < 0) currentMask |= NeighbourType.Left;
                    }
                    else if (Mathf.Abs(direction.x) < Tolerance) // Клетки на одной линии по X, значит соседа ищем по Z
                    {
                        if (direction.z > 0) currentMask |= NeighbourType.Top;
                        else if (direction.z < 0) currentMask |= NeighbourType.Bottom;
                    }
                }
            }
            _currentCell.SetNeighbours(currentMask);
        }
    }

    private void InitializeUnits()
    {
        allUnits=FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit unit in allUnits)
        {
            Cell closesCell = null;
            float closestDistance = float.MaxValue;
            foreach(Cell cell in allCells)
            {
                float distance = Vector3.Distance(unit.transform.position, cell.transform.position);
                Vector2 unitPosXZ = new Vector2(unit.transform.position.x, unit.transform.position.z);
                Vector2 cellPosXZ = new Vector2(cell.transform.position.x, cell.transform.position.z);
                float distanceXZ = Vector2.Distance(unitPosXZ, cellPosXZ);
                if (distanceXZ < Tolerance && distance < closestDistance) 
                {
                    closestDistance = distance;
                    closesCell = cell;
                }
            }
            if(closesCell!=null)
            {
                unit.Cell = closesCell;
                closesCell.Unit = unit;
            }
        }

    }
   
}
