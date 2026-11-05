using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Unity.VisualScripting;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _focusMaterial;
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private TextMeshProUGUI turnText;

    public List<Cell> forcedCells { get; private set; } = new List<Cell>();


    [SerializeField] private CameraController _cameraController;

    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;
    public DiContainer Container { get; private set; }
    private PlaceholderFactory<SelectCheckerCommand> _commandFactory;
    private InputManager _inputManager;
   

    private Cell _lastHoveredCell;
    private Color _hoverColor = new Color(0.4f, 0.85f, 0.8f, 0.35f);

    [Inject]

    public void Construct(PlayerController playerController, Battlefield battlefield, PlaceholderFactory<SelectCheckerCommand> commandFactory, CameraController cameraController)

    {
        this._playerController = playerController;
        this._battlefield = battlefield;
        this._commandFactory = commandFactory;

        this._cameraController = cameraController;
        _inputManager = new InputManager();

    }

    private void OnEnable()
    {
        if (_inputManager != null)
        {
            _inputManager.Gameplay.Enable();
            _inputManager.Gameplay.Select.performed += OnSelectClicked;
            _inputManager.Gameplay.Cancel.performed += OnCancelPressed;
        }
    }
    private void Start()
    {   
        AnalizeForcedMoves();
        ResetToSelectMode();
        UpdateTurnText();
    }

    private void Update()
    {
        HandleCellHover();
    }

    private void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.Gameplay.Select.performed -= OnSelectClicked;
            _inputManager.Gameplay.Cancel.performed -= OnCancelPressed;
            _inputManager.Gameplay.Disable();
        }
    }
    private void ResetLastHovered()
    {
        if (_lastHoveredCell != null)
        {
            _lastHoveredCell.SetHoverHighlight(false, Color.clear);
            _lastHoveredCell = null;
        }
    }
    private void HandleCellHover()
    {
        if (_playerController != null && _playerController.IsInputBlocked)
        {
            ResetLastHovered();
            return;
        }

        Vector2 mousePosition = _inputManager.Gameplay.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Cell cell = hit.collider.GetComponent<Cell>();

            if (cell == null)
            {
                Checker checker = hit.collider.GetComponent<Checker>();
                if (checker != null)
                {
                    cell = checker.CurrentCell;
                }
            }

            if (cell != _lastHoveredCell)
            {
                ResetLastHovered();

                if (cell != null)
                {
                    _lastHoveredCell = cell;

                    _lastHoveredCell.SetHoverHighlight(true, _hoverColor);
                }
            }
        }
        else
        {
            ResetLastHovered();
        }
    }
    private void OnSelectClicked(InputAction.CallbackContext context)
    {
        if (_playerController != null && _playerController.IsInputBlocked)
        {
            Debug.Log("[BattleController] Input blocked");
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Cell cell = hit.collider.GetComponent<Cell>();

            if (cell == null)
            {
                Checker checker = hit.collider.GetComponent<Checker>();
                if (checker != null)
                {
                    cell = checker.CurrentCell;
                }
            }

            if (cell != null && currentCommand != null)
            {
                currentCommand.Interact(cell);
            }
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (_playerController != null && _playerController.IsInputBlocked) return;

        Debug.Log("[BattleController] Нажата ESC: Сброс и отмена действия.");
        ResetToSelectMode();
    }
        
    private void AnalizeForcedMoves()
    {
        if (forcedCells == null) forcedCells = new List<Cell>();

        forcedCells.Clear();

        if (_battlefield == null) return;

        var result = _battlefield.GetAllCellsWithValidJumps(CurrentTurn);

        if (result != null)
        {
            forcedCells.AddRange(result);
        }

        if (forcedCells.Count > 0)
        {
            Debug.Log($"[BattleController] Обнаружена обязательная рубка! Фишек для атаки: {forcedCells.Count}");
        }
    }

    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"Ходит {CurrentTurn}");
        UpdateTurnText();

        AnalizeForcedMoves();
        ResetToSelectMode();

        _playerController.SetBlock(true);
        if(_cameraController != null)
        {
        _cameraController.RotateToTeam(CurrentTurn, () => 
        {
            ResetToSelectMode();
            _playerController.SetBlock(false);
        });
        }
        else
        {
            ResetToSelectMode();
            _playerController.SetBlock(false);
        }
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

    public void ResetToSelectMode()
    {
        if (_battlefield != null)
        {
            _battlefield.ClearAllHighlights();
        }

        if (_commandFactory != null)
        {
            currentCommand = _commandFactory.Create();
        }
        else
        {
            Debug.LogError("[BattleController] Ошибка: Фабрика _commandFactory не заинжектилась!");
        }
    }
}
