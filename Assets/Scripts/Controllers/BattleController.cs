using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;

    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _focusMaterial;
    [SerializeField] private Material _selectMaterial;
    //UI
    [SerializeField] private TextMeshProUGUI turnText;
    //Camera
    [SerializeField] private Camera mainCamera;
    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;

    private InputManager inputActions;
    private Cell lastHoveredCell = null;
    private void Awake()
    {
        inputActions = new InputManager();
    }

    private void OnEnable()
    {
        if (inputActions == null) inputActions = new InputManager();
        inputActions.Enable();
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Select.performed += OnSelectPerformed;
        inputActions.Gameplay.Cancel.performed += OnCancelPerformed;
        inputActions.Gameplay.Confirm.performed += OnConfirmPerformed;
        /*Cell.OnClick += HandleObjectClick;
        Cell.OnEnter += HandleObjectEnter;
        Cell.OnExit += HandleObjectExit;*/
    }
    private void Start()
    {
        //currentCommand = new SelectCheckerCommand(_battlefield, this, _playerController);
        if (mainCamera == null) mainCamera = Camera.main;
        ResetToSelectMode();
        UpdateTurnText();

    }
    private void OnDisable()
    {
        
        inputActions.Gameplay.Select.performed -= OnSelectPerformed;
        inputActions.Gameplay.Cancel.performed -= OnCancelPerformed;
        inputActions.Gameplay.Confirm.performed -= OnConfirmPerformed;
        inputActions.Gameplay.Disable();
        /*Cell.OnClick -= HandleObjectClick;
        Cell.OnEnter -= HandleObjectEnter;
        Cell.OnExit -= HandleObjectExit;*/
    }
    private void HandleObjectClick(Cell cell)
    {
        if (_playerController != null && _playerController.IsInputBlocked) 
        {
            Debug.Log("[BattleController] Click blocked");
            return;
        }
        if (currentCommand != null)
        {
            currentCommand.Interact(cell);
        }
    }       

    private void HandleObjectEnter(Cell cell)
    {
        if (_playerController != null && _playerController.IsInputBlocked)
        {
            Debug.Log("[BattleController] Click blocked");
            return;
        }
    }
    private void HandleObjectExit(Cell cell)
    {
        if (_playerController != null && _playerController.IsInputBlocked)
        {

        }
    }
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (_playerController != null && _playerController.IsInputBlocked) 
        {
            Debug.Log("31231");
            return;
        }
        Vector2 mousePosition = inputActions.Gameplay.Point.ReadValue<Vector2>();
        Cell clickedCell = GetCellUnderCursor(mousePosition);

        if (clickedCell != null && currentCommand != null)
        {
            currentCommand.Interact(clickedCell);
        }
    }
    private void OnConfirmPerformed(InputAction.CallbackContext context)
    {
        if (_playerController.IsInputBlocked) return;
        if(currentCommand is MoveCommand moveCmd)
        {
            moveCmd.ExecuteConfirmedMove();
        }
    }
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (_playerController.IsInputBlocked) return;

        Debug.Log("[BattleController] Нажат ESC. Отмена выбора.");
        ResetToSelectMode();
    }

    public void ResetToSelectMode()
    {
        _battlefield.ClearAllHighlights();
        currentCommand = new SelectCheckerCommand(_battlefield, this, _playerController);
    }

    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"Ходит {CurrentTurn}");
        UpdateTurnText();
        currentCommand = new SelectCheckerCommand(_battlefield, this, _playerController);
    }

    private void UpdateTurnText()
    {
        if (turnText == null) return;
        
        if(CurrentTurn==Team.White)
        {
            turnText.text = "Ход: <color=#FFFFFF>Белые</color>";
        }
        if (CurrentTurn == Team.Black)
        {
            turnText.text = "Ход: <color=#000000>Черные</color>";
        }
    }

    public void SetCommand(IGameplayCommand newCommand)
    {
        currentCommand = newCommand;
    }
    private void HandleCellClick(Cell clickedCell)
    {
        if (_playerController.IsInputBlocked) return;
        if(currentCommand!=null)
        {
            currentCommand.Interact(clickedCell);
        }
    }
    private void HandleHoverLogic()
    {
        if (_playerController != null && _playerController.IsInputBlocked) return;

        // Считываем Vector2 позицию курсора на экране (из Action Point)
        Vector2 mousePosition = inputActions.Gameplay.Point.ReadValue<Vector2>();
        Cell currentCell = GetCellUnderCursor(mousePosition);

        // Если мы перевели мышку на другую клетку доски
        if (currentCell != lastHoveredCell)
        {
            if (lastHoveredCell != null)
            {
                _battlefield.HoverCell(lastHoveredCell, false);
            }

            if (currentCell != null)
            {
                _battlefield.HoverCell(currentCell, true);
            }

            lastHoveredCell = currentCell;
        }
    }
    private Cell GetCellUnderCursor(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell == null) cell = hit.collider.GetComponentInParent<Cell>();
            return cell;
        }
        return null;
    }
}
