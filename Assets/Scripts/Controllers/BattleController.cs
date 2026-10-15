using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _focusMaterial;
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private CameraController _cameraController;
    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;

    private PlaceholderFactory<SelectCheckerCommand> _commandFactory;

    [Inject]
    public void Construct(PlayerController playerController, Battlefield battlefield, PlaceholderFactory<SelectCheckerCommand> commandFactory, CameraController cameraController)
    {
        this._playerController = playerController;
        this._battlefield = battlefield;
        this._commandFactory = commandFactory;
        this._cameraController = cameraController;
    }

    private void OnEnable()
    {
        Cell.OnClick += HandleObjectClick;
        Cell.OnEnter += HandleObjectEnter;
        Cell.OnExit += HandleObjectExit;
    }
    private void Start()
    {        
        ResetToSelectMode();
        UpdateTurnText();
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
    
    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"Ходит {CurrentTurn}");
        UpdateTurnText();
        _playerController.SetBlock(true);
        if(_cameraController != null)
        {
        _cameraController.RotateToTeam(CurrentTurn, () => 
        {
            ResetToSelectMode();
            _playerController.SetBlock(false);
        //currentCommand = new SelectCheckerCommand(_battlefield, this, _playerController);
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
        _battlefield.ClearAllHighlights();
        currentCommand = _commandFactory.Create();
    }
}
