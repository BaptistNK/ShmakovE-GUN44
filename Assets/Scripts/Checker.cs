using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Checker : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Team team;
    public Team checkerTeam => team;

    public Cell CurrentCell {  get; private set; }
    public bool IsKing { get; private set; }
    public void Init(Team team, Cell startCell)
    {
        this.team = team;
        MoveToCell(startCell);
    }

    public void MoveToCell(Cell newCell)
    {
        CurrentCell = newCell;
    }

    public void PromoteToKing()
    {
        if (IsKing) return;
        IsKing = true;
        name = $"King {team} Checker";
        GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        crown.name = "King_Crown";
        crown.transform.SetParent(transform);
        crown.transform.localScale = new Vector3(.5f,.2f,.5f);
        crown.transform.localPosition = new Vector3(0f, 1f, 0f);
        crown.transform.localRotation = Quaternion.identity;
        Collider crownCollider = crown.GetComponent<Collider>();
        if(crownCollider!= null )
        {
            Destroy(crownCollider);
        }
        Renderer crownRender = crown.GetComponent<Renderer>();
        Renderer myRender = crown.GetComponent<Renderer>();

        if (crownRender != null && myRender != null) 
        {
            crownRender.material.color = new Color(1f, .85f, 0f);
            if (crownRender.material.HasProperty("_BaseColor"))
                crownRender.material.SetColor("_BaseColor", new Color(1f, 0.85f, 0f));
        }
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
