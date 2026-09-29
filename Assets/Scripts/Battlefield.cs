using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;
    [Header("Settings")]
    [SerializeField] private float _cellSize;

    [Header("Board colors")]
    [SerializeField] private Material _darkMaterial;
    [SerializeField] private Material _lightMaterial;

    private Dictionary<Vector2Int, Cell> _grid = new Dictionary<Vector2Int, Cell>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
