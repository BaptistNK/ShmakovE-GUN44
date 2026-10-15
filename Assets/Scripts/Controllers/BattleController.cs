using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _focusMaterial;
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private TextMeshProUGUI turnText;
    private InputManager inputAction;
    public List<Cell> forcedCells { get; private set; } = new List<Cell>();
    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;
    public DiContainer Container { get; private set; }
    private PlaceholderFactory<SelectCheckerCommand> _commandFactory;

    [Inject]
    public void Construct(DiContainer container, PlayerController playerController, Battlefield battlefield, PlaceholderFactory<SelectCheckerCommand> commandFactory, InputManager inputManager)
    {
        this._playerController = playerController;
        this._battlefield = battlefield;
        this._commandFactory = commandFactory;
        this.Container = container;
        this.inputAction = inputManager;
    }

    private void OnEnable()
    {
        Cell.OnClick += HandleObjectClick;
        Cell.OnEnter += HandleObjectEnter;
        //Cell.OnExit += HandleObjectExit;
    }
    private void Start()
    {   
        AnalizeForcedMoves();
        ResetToSelectMode();
        UpdateTurnText();
    }
    private void OnDisable()
    {
        Cell.OnClick -= HandleObjectClick;
        Cell.OnEnter -= HandleObjectEnter;
        //Cell.OnExit -= HandleObjectExit;
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
    
    private void AnalizeForcedMoves()
    {
        if (forcedCells == null) forcedCells = new List<Cell>();

        forcedCells.Clear();

        if (_battlefield == null) return;

        // Получаем данные из Battlefield
        var result = _battlefield.GetAllCellsWithValidJumps(CurrentTurn);

        // ИСПРАВЛЕНО: Жесткая проверка. Если Battlefield почему-то вернул null, 
        // мы оставляем наш forcedCells просто пустым списком, а не ломаем игру
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
