using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Team Team {  get; set; }
    public UnitType Type { get; set; }
    public Cell CurrentCell { get; set; }
    public event Action OnMoveEndCallback;
    [SerializeField] private float _speed = 5f;
    private Cell _currentCell;
    private Coroutine _moveCoroutine;
    public Cell Cell { get; set; }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(_currentCell!=null)
        {
            _currentCell.OnPointerClick(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_currentCell!=null)
        {
            _currentCell.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentCell != null)
        {
            _currentCell.OnPointerExit(eventData);
        }
    }

    void Move(Cell cell)
    {
        if (cell == null) return;
        _currentCell=cell;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = StartCoroutine(MoveRoutine(cell.transform.position));
    }
    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPosition, _speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        _moveCoroutine = null;
        OnMoveEndCallback?.Invoke();
    }
}
