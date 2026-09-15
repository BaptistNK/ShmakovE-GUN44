using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public event Action<GameObject> OnPointerClickEvent;
    [SerializeField] private MeshRenderer _colorCell;
    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;
    private BattleController _battleController;
    
    public Unit Unit { get; set; }
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

    public void SetSelect(CellSelectType selectType)
    {
       
    }

    public void ReserSelect()
    {
        if(_select!=null)
        {
            _select.enabled = false;
        }
    }

    public void InitBaseColor(Material material)
    {
        if (_colorCell != null && material != null) 
        {
            _colorCell.sharedMaterial = material;
        }
    }
}
