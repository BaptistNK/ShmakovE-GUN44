using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private MeshRenderer meshRenderer;    
    public static event Action<Cell> OnClick;
    public static event Action<Cell> OnEnter;
    public static event Action<Cell> OnExit;
    private Material _baseMaterial;
    public List<Cell> Neighbors { get; private set; } = new List<Cell>();
    public Checker CurrentChecker { get; private set; }
    public Vector2Int Coordinates {  get; set; }

    public bool IsOccupied => CurrentChecker != null;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke(this);
    }

    public void InitBaseMaterial(Material material)
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if (meshRenderer != null && material != null)
        {
            meshRenderer.sharedMaterial = material;
            _baseMaterial = material;
        }
    }
    public void SetHighlight(bool active, Color _color)
    {
        if (meshRenderer == null) return;
        if(active)
        {
            meshRenderer.material.color = _color;
            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", _color * 0.5f);
        }
        else
        {
            meshRenderer.material.color = _color;
            meshRenderer.material.DisableKeyword("_EMISSION");
        }
    }
    public void SetNeighbors(List<Cell> neighbors)
    {
        Neighbors = neighbors;
    }
    public void SetChecker(Checker checker)
    {
        CurrentChecker = checker;
    }
}
