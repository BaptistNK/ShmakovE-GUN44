using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Checker : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentCell != null)
        {
            CurrentCell.OnPointerClick(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CurrentCell != null)
        {
            CurrentCell.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(CurrentCell != null)
        {
            CurrentCell.OnPointerExit(eventData);
        }
    }
}
