using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputService : IInputService, IInitializable, IDisposable
{    
    private readonly InputSettings _settings; 
    private PlayerControls _controls; //Script input system

    public InputService(InputSettings settings)
    {
        _settings = settings;
    }


    //return vector movement WASD
    public Vector2 MoveAxis => _controls != null ? _controls.Player.Move.ReadValue<Vector2>() : Vector2.zero;

    //returt mouse movement
    public Vector2 LookAxis
    {
        get
        {
            if (_controls == null) return Vector2.zero;

            Vector2 rawLook = _controls.Player.Look.ReadValue<Vector2>();
            float InvertMultiplier = _settings.invertY ? -1f : 1f;

            return new Vector2(
                rawLook.x * _settings.mouseSensitivity, 
                rawLook.y * _settings.mouseSensitivity * InvertMultiplier);
        }
            
    }

    public event Action OnFirePressed;
    public event Action OnAimPressed;
    public event Action OnAimReleased;
    public event Action OnReloadPressed;
    public event Action OnDashPressed;
    public event Action OnDashReleased;
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnSquatPressed;
    public event Action OnSquatReleased;

    public void Initialize()
    {

        _controls = new PlayerControls();
        //enable input system
        _controls.Enable();
        //Fire
        _controls.Player.Fire.started += InvokeFirePressed;
        //Aim
        _controls.Player.Aim.started += InvokeAimPressed;
        _controls.Player.Aim.canceled += InvokeAimReleased;
        //reload
        _controls.Player.Reload.started += InvokeReloadPressed;

        _controls.Player.Dash.started += InvokeDashPressed;
        _controls.Player.Dash.canceled += InvokeDashReleased;
        _controls.Player.Jump.started += InvokeJumpPressed;
        //squat
        _controls.Player.Squat.started += InvokeSquatPressed;
        _controls.Player.Squat.canceled += InvokeSquatReleased;

        Debug.Log("[InputService] Успешно инициализирован, все кнопки подписаны");

    }

    public void Dispose()
    {
        if (_controls == null) return;

        //Fire
        _controls.Player.Fire.started -= InvokeFirePressed;
        //Aim
        _controls.Player.Aim.started -= InvokeAimPressed;
        _controls.Player.Aim.canceled -= InvokeAimReleased;
        //reload
        _controls.Player.Reload.started -= InvokeReloadPressed;
        _controls.Player.Dash.started -= InvokeDashPressed;
        _controls.Player.Dash.canceled -= InvokeDashReleased;
        _controls.Player.Jump.started -= InvokeJumpPressed;
        //squat
        _controls.Player.Squat.started -= InvokeSquatPressed;
        _controls.Player.Squat.canceled -= InvokeSquatReleased;

        _controls.Disable();
        _controls.Dispose();
        _controls = null;
        Debug.Log("[InputService] Ресурсы успешно освобождены");
    }

    public void ToggleInputMap(bool isGameplay)
    {
        if(_controls==null) return;
        if(isGameplay)
        {
            _controls.Player.Enable();
        }
        else
        {
            _controls.Player.Disable();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InvokeFirePressed(InputAction.CallbackContext ctx) => OnFirePressed?.Invoke();
    private void InvokeAimPressed(InputAction.CallbackContext ctx) => OnAimPressed?.Invoke();
    private void InvokeAimReleased(InputAction.CallbackContext ctx) => OnAimReleased?.Invoke();
    private void InvokeReloadPressed(InputAction.CallbackContext ctx) => OnReloadPressed?.Invoke();
    private void InvokeDashPressed(InputAction.CallbackContext ctx) => OnDashPressed?.Invoke();
    private void InvokeDashReleased(InputAction.CallbackContext ctx) => OnDashReleased?.Invoke();
    private void InvokeJumpPressed(InputAction.CallbackContext ctx) => OnJumpPressed?.Invoke();
    private void InvokeSquatPressed(InputAction.CallbackContext ctx) => OnSquatPressed?.Invoke();
    private void InvokeSquatReleased(InputAction.CallbackContext ctx) => OnSquatReleased?.Invoke();
}
