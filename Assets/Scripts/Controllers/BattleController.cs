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
    }
    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        ResetToSelectMode();
        UpdateTurnText();

    }
    private void Update()
    {
        HandleHoverLogic();
    }
    private void OnDisable()
    {
        
        inputActions.Gameplay.Select.performed -= OnSelectPerformed;
        inputActions.Gameplay.Cancel.performed -= OnCancelPerformed;
        inputActions.Gameplay.Confirm.performed -= OnConfirmPerformed;
        inputActions.Gameplay.Disable();
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
    
    private void HandleHoverLogic()
    {
        if (_playerController != null && _playerController.IsInputBlocked) return;

        Vector2 mousePosition = inputActions.Gameplay.Point.ReadValue<Vector2>();
        Cell currentCell = GetCellUnderCursor(mousePosition);

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
            Debug.Log($"[Raycast] Луч попал в объект: {hit.collider.gameObject.name} на слое {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell == null) cell = hit.collider.GetComponentInParent<Cell>();
            if (cell == null)
            {
                Checker checker = hit.collider.GetComponent<Checker>();
                if (checker == null) checker = hit.collider.GetComponentInParent<Checker>();

                // Если попали в шашку, берем клетку, на которой она физически стоит в данных игры!
                if (checker != null) cell = checker.CurrentCell;
            }
            return cell;
        }
        return null;
    }
}
