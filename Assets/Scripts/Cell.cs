using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    public static event Action<Cell> OnClick;
    public static event Action<Cell> OnEnter;
    public static event Action<Cell> OnExit;
    private Material _baseMaterial;
    public List<Cell> Neighbors { get; private set; } = new List<Cell>();
    public Checker CurrentChecker { get; private set; }
    public Vector2Int Coordinates {  get; set; }

    public bool IsOccupied => CurrentChecker != null;
    public string HighlightType { get; set; } = "Default";
    public void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && meshRenderer.material != null) 
        {
            propBlock = new MaterialPropertyBlock();
        }
    }
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
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", _color);
            meshRenderer.SetPropertyBlock(propBlock);
        }
        else
        {
            meshRenderer.SetPropertyBlock(null);
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
