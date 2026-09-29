using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    public static event Action<GameObject> Obj;

    private void OnEnable()
    {
        Cell.OnClick += HandleObjectClick;
        Cell.OnEnter += HandleObjectEnter;
        Cell.OnExit += HandleObjectExit;
    }

    private void OnDisable()
    {
        Cell.OnClick -= HandleObjectClick;
        Cell.OnEnter -= HandleObjectEnter;
        Cell.OnExit -= HandleObjectExit;
    }
    private void HandleObjectClick(Cell cell)
    {
        if (_playerController != null && _playerController.IsInputBlocked) 
        {
            Debug.Log("[BattleController] Click blocked");
            return;
        }
       Select(cell);
    }

    private void Select(Cell target)
    {
        Debug.Log("[BattleController] Click succesed");
    }

    private void HandleObjectEnter(Cell cell)
    {
        Debug.Log($"[BattleController] Cursor on {cell.name}");
    }
    private void HandleObjectExit(Cell cell)
    {
        Debug.Log($"[BattleController] Cursor leave {cell.name}");
    }
    
}
