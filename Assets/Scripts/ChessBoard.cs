using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    private List<ChessBoard> allCells = new List<ChessBoard>();
    private void Awake()
    {
        Cell[] cells = GetComponentsInChildren<Cell>();
        //allCells.AddRange(cells);
    }
}
