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

    [Header("UI Restart Settings")]
    [SerializeField] private GameObject _restartPanel;
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _holdDuration = 2.0f;

    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;
    public DiContainer Container { get; private set; }
    private PlaceholderFactory<SelectCheckerCommand> _commandFactory;
    private InputManager _inputManager;
    private float _currentHoldTime = 0f;
    private bool _isTabPressed = false;

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
            _inputManager.Gameplay.Confirm.performed += OnConfirmPressed;
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
        HandleTabRestart();
        HandleCellHover();
    }

    private void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.Gameplay.Select.performed -= OnSelectClicked;
            _inputManager.Gameplay.Cancel.performed -= OnCancelPressed;
            _inputManager.Gameplay.Confirm.performed -= OnConfirmPressed;
            _inputManager.Gameplay.Disable();
        }
    }
    private void ResetLastHovered()
    {
        if (_lastHoveredCell != null)
        {
            if (_battlefield != null)
            {
                _battlefield.HighlightCell(_lastHoveredCell, false, Color.clear);
            }
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

                    if (_battlefield != null)
                    {
                        _battlefield.HighlightCell(_lastHoveredCell, true, _hoverColor);
                    }
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
    private void HandleTabRestart()
    {
        if (Keyboard.current == null) return;

        _isTabPressed = Keyboard.current.tabKey.isPressed;

        if (_isTabPressed)
        {
            if (_restartPanel != null && !_restartPanel.activeSelf)
            {
                _restartPanel.SetActive(true);
            }

            _currentHoldTime += Time.deltaTime;

            if (_progressBar != null)
            {
                _progressBar.fillAmount = Mathf.Clamp01(_currentHoldTime / _holdDuration);
            }

            if (_currentHoldTime >= _holdDuration)
            {
                int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(activeSceneIndex);
            }
        }
        else
        {
            if (_currentHoldTime > 0f)
            {
                _currentHoldTime -= Time.deltaTime * 5f;
                if (_currentHoldTime < 0f) _currentHoldTime = 0f;

                if (_progressBar != null)
                {
                    _progressBar.fillAmount = Mathf.Clamp01(_currentHoldTime / _holdDuration);
                }

                if (_currentHoldTime == 0f && _restartPanel != null)
                {
                    _restartPanel.SetActive(false);
                }
            }
        }
    }
    private void OnConfirmPressed(InputAction.CallbackContext context)
    {
        if (_playerController != null && _playerController.IsInputBlocked) return;

        Debug.Log("[BattleController] Нажат Пробел: Подтверждение действия.");
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
