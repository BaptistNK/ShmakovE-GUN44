using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Checker checkerPrefab;

    [Header("Settings")]
    [SerializeField] private float _cellSize;
    private Cell _currentlySelected;

    [Header("Board colors")]
    [SerializeField] private Material _darkMaterial;
    [SerializeField] private Material _lightMaterial;

    private Dictionary<Vector2Int, Cell> _grid = new Dictionary<Vector2Int, Cell>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        BuildGraph();
        SpawnCheckers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create board
    private void GenerateGrid()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                Vector3 worldPosition = new Vector3(x * _cellSize, 0f, y * _cellSize);
                GameObject cellObj = Instantiate(_cellPrefab, worldPosition, Quaternion.identity);
                Cell cell = cellObj.GetComponent<Cell>();
                if (cell != null)
                {
                    cell.Coordinates = coords;
                    if((x+y)%2==0)
                    {
                        cell.InitBaseMaterial(_darkMaterial);
                    }
                    else
                    {
                        cell.InitBaseMaterial(_lightMaterial);
                    }
                    _grid[coords] = cell;
                }
            }
        }
        Debug.Log($"[Battlefield] Поле сгенерировано. клеток в словаре: {_grid.Count}");
    }
    //Create neighbors cell
    private void BuildGraph()
    {
        foreach(var pair in _grid)
        {
            Vector2Int currentCoords = pair.Key;
            Cell currentCell = pair.Value;
            List<Cell> neighbors = new List<Cell>();
            Vector2Int[] diagonalDirections = new Vector2Int[]
            {
                new Vector2Int(1,1),
                new Vector2Int(-1,1),
                new Vector2Int(1,-1),
                new Vector2Int(-1,-1)
            };
            foreach(Vector2Int direction in diagonalDirections)
            {
                Vector2Int neighborCoords = currentCoords + direction;
                if(_grid.ContainsKey(neighborCoords))
                {
                    neighbors.Add(_grid[neighborCoords]);
                }
            }
            currentCell.SetNeighbors(neighbors);
        }
    }

    public void HighlightCell(Cell cell, bool active, Color color)
    {
        if (cell == null) return;
        cell.SetHighlight(active, color);
    }
    //Reset all highlight
    public void ClearAllHighlights()
    {
        foreach(var pair in _grid)
        {
            pair.Value.SetHighlight(false, Color.clear);
        }
    }

    private void SpawnCheckers()
    {
        int whiteCount = 0;
        int blackCount = 0;
        foreach (var pair in _grid)
        {
            Vector2Int coords = pair.Key;
            Cell cell = pair.Value;
            bool isDarkCell = (coords.x + coords.y) % 2 == 0;

            if (!isDarkCell)
            {
                continue;
            }
                
            if (coords.y < 3)
            {
                CreateCheckerOnCell(Team.White, cell);
                whiteCount++;
            }
            else if (coords.y > 4)
            {
                CreateCheckerOnCell(Team.Black, cell);
                blackCount++;
            }
        }
        Debug.Log("Checkers spawn");
    }

    public Cell GetCellInDirection(Cell startCell, Vector2Int direction)
    {
        Vector2Int targetCoords = startCell.Coordinates + direction;
        return GetCellAt(targetCoords);
    }
    private void CreateCheckerOnCell(Team team, Cell cell)
    {
        Vector3 spawnPos = cell.transform.position + Vector3.up * .6f;
        Checker newChecker = Instantiate(checkerPrefab, spawnPos, Quaternion.identity);
        newChecker.name = $"{team} Checker [{cell.Coordinates.x}, {cell.Coordinates.y}]";
        Renderer checkerRenderer = newChecker.GetComponentInChildren<Renderer>();
        if(checkerRenderer != null )
        {
            Color targetColor = (team == Team.White) ? Color.white : Color.black;
            checkerRenderer.material.color=targetColor;
            if(checkerRenderer.material.HasProperty("_BaseColor"))
            {
                checkerRenderer.material.SetColor("_BaseColor", targetColor);
            }

            newChecker.Init(team, cell);
            cell.SetChecker(newChecker);
        }
    }
    public Cell GetCellAt(Vector2Int coords)
    {
        if(_grid.TryGetValue(coords, out Cell cell)) return cell;
        return null;
    }
}
