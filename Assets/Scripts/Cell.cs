using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public event Action<GameObject> OnPointerClickEvent;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;
    private BattleController _battleController;
    [Header("Highlight materials")]
    [SerializeField] private Material selectMaterial;
    [SerializeField] private Material moveMaterial;
    [SerializeField] private Material attackMaterial;
    private Material _baseMaterial;

    public Unit CurrentUnit { get; set; }
    public Vector2Int Coordinates { get; set; }

    [Inject]
    public void Construct(BattleController battleController)
    {
        _battleController = battleController;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (_battleController != null)
            {
                _battleController.HandleCellSelection(this);
            }
        }
        OnPointerClickEvent?.Invoke(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_focus!=null)
        {
            _focus.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_focus != null)
        {
            _focus.enabled = false;
        }
    }

    /*public void ReserSelect()
    {
        if(_select!=null)
        {
            _select.enabled = false;
        }
    }*/

    public void InitBaseMaterial(Material material)
    {
        if(meshRenderer==null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if (meshRenderer != null && material != null) 
        {
            meshRenderer.sharedMaterial = material;
            _baseMaterial = material;
        }
    }
    public void SetHighlight(CellHighlightState state)
    {
        if (meshRenderer == null) return;
        switch(state)
        {
            case CellHighlightState.None:
                meshRenderer.sharedMaterial = _baseMaterial;
                break;
            case CellHighlightState.Selected:
                if (selectMaterial != null) meshRenderer.sharedMaterial = selectMaterial;
                break;
            case CellHighlightState.CanMove:
                if (moveMaterial != null) meshRenderer.sharedMaterial = moveMaterial;
                break;
            case CellHighlightState.CanAttack:
                if (attackMaterial != null) meshRenderer.sharedMaterial = attackMaterial;
                break;
        }
    }
}
