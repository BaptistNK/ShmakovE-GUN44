using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private Team team;
    public Team checkerTeam => team;

    public Cell CurrentCell {  get; private set; }

    public void Init(Team team, Cell startCell)
    {
        this.team = team;
        MoveToCell(startCell);
    }

    public void MoveToCell(Cell newCell)
    {
        CurrentCell = newCell;
    }
}
