using System;
using UnityEngine;

namespace TPSShooter.Services.Input
{
    public interface IInputService
    {
        Vector2 MoveAxis { get; }
        Vector2 LookAxis { get; }

        event Action OnFirePressed;
        event Action OnFireReleased;
        event Action OnAimPressed;
        event Action OnAimReleased;
        event Action OnReloadPressed;
        event Action OnDashPressed;
        event Action OnJumpPressed;
        event Action OnSquatPressed;
        event Action OnSquatReleased;
        event Action OnInteractPressed;
        event Action OnMeleeAttackPressed;
        event Action OnPausePressed;
        event Action OnResumePressed;
        event Action OnMapToggled;
        event Action<float> OnWeaponScrolled;

        void ToggleInputMap(bool isGameplay);
    }
}
