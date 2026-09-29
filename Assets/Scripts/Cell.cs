using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField]private MeshRenderer _focus;
    [SerializeField]private MeshRenderer _select;
    public static event Action<Cell> OnClick;
    public static event Action<Cell> OnEnter;
    public static event Action<Cell> OnExit;
    private Material _baseMaterial;

    public Vector2Int Coordinates {  get; set; }
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
}
