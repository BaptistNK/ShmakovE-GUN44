using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _focusMaterial;
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private TextMeshProUGUI turnText;
    private IGameplayCommand currentCommand;
    public Team CurrentTurn { get; private set; } = Team.White;

    private void OnEnable()
    {
        Cell.OnClick += HandleObjectClick;
        Cell.OnEnter += HandleObjectEnter;
        Cell.OnExit += HandleObjectExit;
    }
    private void Start()
    {
        currentCommand = new SelectCheckerCommand(_battlefield, this, _playerController);
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
        currentCommand=new SelectCheckerCommand(_battlefield, this, _playerController);
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
}
