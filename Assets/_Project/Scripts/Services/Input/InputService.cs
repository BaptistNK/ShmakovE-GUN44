using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TPSShooter.Services.Input
{
    public class InputService : IInputService, IInitializable, IDisposable
    {
        private PlayerControls _controls;

        public Vector2 MoveAxis => _controls != null ? _controls.Player.Move.ReadValue<Vector2>() : Vector2.zero;
        public Vector2 LookAxis => _controls != null ? _controls.Player.Look.ReadValue<Vector2>() : Vector2.zero;

        public event Action OnFirePressed;
        public event Action OnFireReleased;
        public event Action OnAimPressed;
        public event Action OnAimReleased;
        public event Action OnReloadPressed;
        public event Action OnDashPressed;
        public event Action OnJumpPressed;
        public event Action OnSquatPressed;
        public event Action OnSquatReleased;
        public event Action OnInteractPressed;
        public event Action OnMeleeAttackPressed;
        public event Action OnPausePressed;
        public event Action OnResumePressed;
        public event Action OnMapToggled;
        public event Action<float> OnWeaponScrolled;

        public void Initialize()
        {
            _controls = new PlayerControls();
            SubscribeEvents();
            ToggleInputMap(true);

#if UNITY_EDITOR
            Debug.Log("[InputService] Успешно инициализирован, карты ввода настроены");
#endif
        }

        public void Dispose()
        {
            if (_controls == null)
            {
                return;
            }

            UnsubscribeEvents();
            _controls.Disable();
            _controls.Dispose();
            _controls = null;

#if UNITY_EDITOR
            Debug.Log("[InputService] Ресурсы успешно освобождены (Dispose)");
#endif
        }

        public void ToggleInputMap(bool isGameplay)
        {
            if (_controls == null)
            {
                return;
            }

            if (isGameplay)
            {
                _controls.UI.Disable();
                _controls.Player.Enable();
            }
            else
            {
                _controls.Player.Disable();
                _controls.UI.Enable();
            }
        }

        private void SubscribeEvents()
        {
            _controls.Player.Fire.started += InvokeFirePressed;
            _controls.Player.Fire.canceled += InvokeFireReleased;
            _controls.Player.Aim.started += InvokeAimPressed;
            _controls.Player.Aim.canceled += InvokeAimReleased;
            _controls.Player.Reload.started += InvokeReloadPressed;
            _controls.Player.Dash.started += InvokeDashPressed;
            _controls.Player.Jump.started += InvokeJumpPressed;
            _controls.Player.Squat.started += InvokeSquatPressed;
            _controls.Player.Squat.canceled += InvokeSquatReleased;
            _controls.Player.Interact.started += InvokeInteractPressed;
            _controls.Player.MeleeAttack.started += InvokeMeleeAttackPressed;
            _controls.Player.Pause.started += InvokePausePressed;
            _controls.Player.ToggleMap.started += InvokeMapToggled;
            _controls.Player.ScrollWeapon.performed += InvokeWeaponScrolled;

            _controls.UI.Resume.started += InvokeResumePressed;
            _controls.UI.CloseMap.started += InvokeMapToggled;
        }

        private void UnsubscribeEvents()
        {
            _controls.Player.Fire.started -= InvokeFirePressed;
            _controls.Player.Fire.canceled -= InvokeFireReleased;
            _controls.Player.Aim.started -= InvokeAimPressed;
            _controls.Player.Aim.canceled -= InvokeAimReleased;
            _controls.Player.Reload.started -= InvokeReloadPressed;
            _controls.Player.Dash.started -= InvokeDashPressed;
            _controls.Player.Jump.started -= InvokeJumpPressed;
            _controls.Player.Squat.started -= InvokeSquatPressed;
            _controls.Player.Squat.canceled -= InvokeSquatReleased;
            _controls.Player.Interact.started -= InvokeInteractPressed;
            _controls.Player.MeleeAttack.started -= InvokeMeleeAttackPressed;
            _controls.Player.Pause.started -= InvokePausePressed;
            _controls.Player.ToggleMap.started -= InvokeMapToggled;
            _controls.Player.ScrollWeapon.performed -= InvokeWeaponScrolled;

            _controls.UI.Resume.started -= InvokeResumePressed;
            _controls.UI.CloseMap.started -= InvokeMapToggled;
        }

        private void InvokeFirePressed(InputAction.CallbackContext ctx) => OnFirePressed?.Invoke();
        private void InvokeFireReleased(InputAction.CallbackContext ctx) => OnFireReleased?.Invoke();
        private void InvokeAimPressed(InputAction.CallbackContext ctx) => OnAimPressed?.Invoke();
        private void InvokeAimReleased(InputAction.CallbackContext ctx) => OnAimReleased?.Invoke();
        private void InvokeReloadPressed(InputAction.CallbackContext ctx) => OnReloadPressed?.Invoke();
        private void InvokeDashPressed(InputAction.CallbackContext ctx) => OnDashPressed?.Invoke();
        private void InvokeJumpPressed(InputAction.CallbackContext ctx) => OnJumpPressed?.Invoke();
        private void InvokeSquatPressed(InputAction.CallbackContext ctx) => OnSquatPressed?.Invoke();
        private void InvokeSquatReleased(InputAction.CallbackContext ctx) => OnSquatReleased?.Invoke();
        private void InvokeInteractPressed(InputAction.CallbackContext ctx) => OnInteractPressed?.Invoke();
        private void InvokeMeleeAttackPressed(InputAction.CallbackContext ctx) => OnMeleeAttackPressed?.Invoke();
        private void InvokePausePressed(InputAction.CallbackContext ctx) => OnPausePressed?.Invoke();
        private void InvokeResumePressed(InputAction.CallbackContext ctx) => OnResumePressed?.Invoke();
        private void InvokeMapToggled(InputAction.CallbackContext ctx) => OnMapToggled?.Invoke();

        private void InvokeWeaponScrolled(InputAction.CallbackContext ctx)
        {
            float scrollY = ctx.ReadValue<Vector2>().y;
            if (Mathf.Abs(scrollY) > 0.01f)
            {
                OnWeaponScrolled?.Invoke(Mathf.Sign(scrollY));
            }
        }
    }
}
