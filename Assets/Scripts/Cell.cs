using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public event Action<GameObject> OnPointerClickEvent;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField]private MeshRenderer _focus;
    [SerializeField]private MeshRenderer _select;
    private CellPaletteSettings _paletteSettings;
    private Material _defaultMaterial;

    [Inject]
    public void Construct(CellPaletteSettings paletteSettings)
    {
        _paletteSettings = paletteSettings;
    }
    private void Awake()
    {
        if(_meshRenderer!=null)
        {
            _defaultMaterial = _meshRenderer.sharedMaterial;
        }
    }
    public Unit Unit { get; set; }
    public NeighbourType NeighbourMask { get; private set; }
    public void SetNeighbours(NeighbourType mask)
    {
        NeighbourMask = mask;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
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
        if (_meshRenderer == null || _paletteSettings == null) return;

        if(selectType==CellSelectType.None)
        {
            _meshRenderer.material = _defaultMaterial;
        }
        else
        {
            Material targetMaterial = _paletteSettings.GetMaterial(selectType);
            if(targetMaterial!= null)
            {
                _meshRenderer.material = targetMaterial;
            }
        }
    }

    public void ReserSelect()
    {
        if(_select!=null)
        {
            _select.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;

        float offset = 1.0f;

        if (NeighbourMask.HasFlag(NeighbourType.Left))
            DrawNeighbourLine(currentPos, currentPos + Vector3.left * offset);

        if (NeighbourMask.HasFlag(NeighbourType.Right))
            DrawNeighbourLine(currentPos, currentPos + Vector3.right * offset);

        if (NeighbourMask.HasFlag(NeighbourType.Top))
            DrawNeighbourLine(currentPos, currentPos + Vector3.forward * offset); 

        if (NeighbourMask.HasFlag(NeighbourType.Bottom))
            DrawNeighbourLine(currentPos, currentPos + Vector3.back * offset);
    }

    private void DrawNeighbourLine(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(to, 0.15f);
    }
}
