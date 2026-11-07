using System.Collections;
using System;
using UnityEngine;

public interface IInputService
{
    Vector2 MoveAxis { get; } //Movement 
    Vector2 LookAxis { get; } //Look camera mouse
    event Action OnFirePressed; //Left button mouse
    event Action OnAimPressed; //Right button mouse 
    event Action OnAimReleased; //Right button mouse released
    event Action OnReloadPressed; //R button
    event Action OnDashPressed; //Shift button (Dash)
    event Action OnJumpPressed; //Space button (Jump)
    event Action OnSquatPressed; //Control button (Squat)
    event Action OnSquatReleased; //Control button (Squat) released

    void ToggleInputMap(bool isGameplay); //pause
}
