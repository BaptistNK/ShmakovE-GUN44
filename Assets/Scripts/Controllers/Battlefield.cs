using UnityEngine;
using System.Collections.Generic;
using System;

public class Battlefield : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject whiteUnitPrefab;
    [SerializeField] private GameObject blackUnitPrefab;

    [Header("Settings")]
    [SerializeField] private float _cellSize = 1.0f;
    [SerializeField] private float unitHeightOffset = 1.2f;

    [Header("Board Colors")]
    [SerializeField] private Material _darkMaterial;
    [SerializeField] private Material _lightMaterial;

    private Dictionary<Vector2Int, Cell> _grid = new Dictionary<Vector2Int, Cell>();

    void Start()
    {
        GenerateGrid();
        SetupInitialPieces();
    }

    void Update()
    {

    }
    // создание шахматной доски
    private void GenerateGrid()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                Vector3 worldPosition = new Vector3(x * _cellSize, 0f, y * _cellSize);

                GameObject cellObj = Instantiate(_cellPrefab, worldPosition, Quaternion.identity, transform);
                Cell cell = cellObj.GetComponent<Cell>();
                if(cell!= null)
                {
                    cell.Coordinates = coords;
                    if ((x + y) % 2 == 0) 
                    {
                        cell.InitBaseColor(_darkMaterial);
                    }
                    else
                    {
                        cell.InitBaseColor(_lightMaterial);
                    }
                    _grid[coords] = cell;
                }
            }
        }
        Debug.Log($"[Battlefield] Сетка сгенерирована. Всего клеток в словаре: {_grid.Count}");
    }
    //метод расставления unit на клетку
    private void SpawnUnitAt(Cell cell, GameObject prefab, Team team)
    {
        

        if (prefab == null)
        {
            Debug.LogWarning($"[Battlefield] Не назначен префаб для команды");
            return;
        }

        Vector3 spawnPosition = cell.transform.position + new Vector3(0f, unitHeightOffset, 0f);
        GameObject unitObj = Instantiate(prefab, spawnPosition, Quaternion.identity, cell.transform);
        Unit unit = unitObj.GetComponent<Unit>();

        if (unit != null)
        {
            cell.Unit = unit;
            unit.CurrentCell = cell;

            unit.Team = team;
            unit.Type = UnitType.Pawn;
        }
        else
        {
            Debug.LogWarning($"[Battlefield] На префабе {prefab.name} отсутствует компонент Unit!");
        }
        Debug.Log("Чпуньк");
    }
    //авто расстановка шашек
    public void SetupInitialPieces()
    {
        int spawnedWhite = 0;
        int spawnedBlack = 0;

        for (int x = 0; x < 8; x++) 
        {
            for (int y = 0; y < 8; y++) 
            {
                if ((x + y) % 2 == 0)
                {
                    Cell cell = GetCell(new Vector2Int(x, y));
                    if (y < 3)
                    {
                        SpawnUnitAt(cell, whiteUnitPrefab, Team.White);
                        spawnedWhite++;
                    }
                    else if (y > 4)
                    {
                        SpawnUnitAt(cell, blackUnitPrefab, Team.Black);
                        spawnedBlack++;
                    }
                }
            }
        }
        Debug.Log($"[Battlefield] Расстановка завершена. Создано Белых: {spawnedWhite}, Чёрных: {spawnedBlack}");
    }

    public Cell GetCell(Vector2Int coords)
    {
        return _grid.TryGetValue(coords, out Cell cell) ? cell : null;
    }
}


