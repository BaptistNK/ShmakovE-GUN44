using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Configs/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Speeds")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _runSpeed = 7f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] [Range(.1f, 5f)] private float _backwardSpeedMultiplier = .6f;

    [Header("Physics & Jump")]
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _maxFallSpeed = -30f;

    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;
    public float RotationSpeed => _rotationSpeed;
    public float Gravity => _gravity;
    public float JumpHeight => _jumpHeight;
    public float BackwardSpeedMultiplier => _backwardSpeedMultiplier;
    public float MaxFallSpeed => _maxFallSpeed;
}
