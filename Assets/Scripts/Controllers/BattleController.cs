using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BattleController : MonoBehaviour
{
    private IGameplayCommand _gameplayCommand;
    private PlayerController _playerController;
    public Team CurrentTurn {  get; private set; }

    [Inject]
    public void Construct(IGameplayCommand gameplayCommand, PlayerController playerController)
    {
        _gameplayCommand = gameplayCommand;
        _playerController = playerController;
    }
    void Start()
    {
        Debug.Log($"[BattleController] Игра началась! Ходит команда: ");
    }

    //обработка кликов мышью
    public void HandleCellSelection(Cell cell)
    {
        if (_playerController != null && _playerController.IsBusy)
            return;
        if(_gameplayCommand!=null)
        {
            _gameplayCommand.Interact(cell);
        }
    }
    //обработка отмены
    public void OnCancelInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (_playerController != null & _playerController.IsBusy)
                return;
            Debug.Log("[BattleController] Нажат Cancel (ESC). Сброс выбора");
            if(_gameplayCommand != null)
            {
                _gameplayCommand.Cancel();
            }
        }
    }
    
    //обработка клавиатуры
    public void OnConfirmInput(InputAction.CallbackContext context)
    {
        if( context.performed)
        {
            if (_playerController != null && _playerController.IsBusy)
                return;
            Debug.Log("[BattleController] Нажат Space");
        }
    }
    //Обработка смены хода

    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"[BattleController] Ход передан. Сейчас ходят: {CurrentTurn}");

        if (_gameplayCommand != null)
        {
            _gameplayCommand.Cancel();
        }
    }
}
