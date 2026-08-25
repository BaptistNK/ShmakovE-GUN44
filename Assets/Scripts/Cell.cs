using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public event Action<GameObject> OnPointerClickEvent;
    [SerializeField]private MeshRenderer _focus;
    [SerializeField]private MeshRenderer _select;
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

    public void SetSelect(Material material)
    {
        if(_select!=null)
        {
            _select.enabled = true;
            _select.material = material;
        }
        else
        {
            Debug.LogWarning($"На объекте {gameObject.name} не назначена ссылка на Select в инспекторе!",this);
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
        // Рисует линии только для ВЫДЕЛЕННОЙ в данный момент клетки
        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;

        // Шаг сетки для отрисовки линий (подстройте под размер ваших кубиков)
        float offset = 1.0f;

        // Если флаг содержит направление, рисуем туда линию с шариком на конце
        if (NeighbourMask.HasFlag(NeighbourType.Left))
            DrawNeighbourLine(currentPos, currentPos + Vector3.left * offset);

        if (NeighbourMask.HasFlag(NeighbourType.Right))
            DrawNeighbourLine(currentPos, currentPos + Vector3.right * offset);

        if (NeighbourMask.HasFlag(NeighbourType.Top))
            DrawNeighbourLine(currentPos, currentPos + Vector3.forward * offset); // Если Top — это вперед по Z

        if (NeighbourMask.HasFlag(NeighbourType.Bottom))
            DrawNeighbourLine(currentPos, currentPos + Vector3.back * offset);
    }

    private void DrawNeighbourLine(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(to, 0.15f);
    }
}
