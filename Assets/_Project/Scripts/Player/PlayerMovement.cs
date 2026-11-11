using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private IInputService _inputService;
    private PlayerSettings _settings;
    private Camera _mainCamera;
    private Animator _animator;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isAiming;
    private bool _isRunning;
    private bool _isSquatting;
    private bool _isTurningBody;
    private float _originalHeight;
    private Vector3 _originalCenter;

    // Безопасные от деления на ноль векторы камеры для текущего кадра
    private Vector3 _safeCameraForward;
    private Vector3 _safeCameraRight;
    [Inject]
    public void Construct(IInputService inputService, PlayerSettings settings)
    {
        _inputService = inputService;
        _settings = settings;
        Debug.Log("[PlayerMovement] Сервис ввода успешно внедрен через Zenject!");
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main;
        _originalHeight = _characterController.height;
        _originalCenter= _characterController.center;

        if (_animator != null)
        {
            Debug.Log($"[PlayerMovement] Найден Animator на объекте: {_animator.gameObject.name}, Controller: {(_animator.runtimeAnimatorController != null ? _animator.runtimeAnimatorController.name : "ПУСТО!")}");
        }
        else
        {
            Debug.LogError("[PlayerMovement] Animator НЕ НАЙДЕН!");
        }

    }

    private void OnEnable()
    {
        _inputService.OnAimPressed += HandleAimPressed;
        _inputService.OnAimReleased += HandleAinReleased;
        _inputService.OnDashPressed += HandleDashPressed;
        _inputService.OnDashReleased += HandleDashReleased;
        _inputService.OnJumpPressed += HandleJumpPressed;
        _inputService.OnSquatPressed += HandleSquatPressed;
        _inputService.OnSquatReleased += HandleSquatReleased;
    }

    private void OnDisable()
    {
        _inputService.OnAimPressed -= HandleAimPressed;
        _inputService.OnAimReleased -= HandleAinReleased;
        _inputService.OnDashPressed -= HandleDashPressed;
        _inputService.OnDashReleased -= HandleDashReleased;
        _inputService.OnJumpPressed -= HandleJumpPressed;
        _inputService.OnSquatPressed -= HandleSquatPressed;
        _inputService.OnSquatReleased -= HandleSquatReleased;
        _isRunning = false; //отключаем фантомный бег
    }
    void Start()
    {
        
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
    }
    void LateUpdate()
    {
        UpdateSafeCameraVectors();
        HandleRotation();
    }
    private void UpdateSafeCameraVectors()
    {
        if (_mainCamera == null) return;

        // Фильтруем Forward камеры
        Vector3 camForward = _mainCamera.transform.forward;
        camForward.y = 0f;
        if (camForward.sqrMagnitude > 0.001f)
        {
            camForward.Normalize();
            _safeCameraForward = camForward;
        }
        else
        {
            _safeCameraForward = transform.forward;
        }

        // Фильтруем Right камеры
        Vector3 camRight = _mainCamera.transform.right;
        camRight.y = 0f;
        if (camRight.sqrMagnitude > 0.001f)
        {
            camRight.Normalize();
            _safeCameraRight = camRight;
        }
        else
        {
            _safeCameraRight = transform.right;
        }
    }
    private void HandleMovement()
    {
        if (_inputService == null || _settings == null || Time.deltaTime <= 0f) return;

        Vector2 input = _inputService.MoveAxis;
        if (float.IsNaN(input.x) || float.IsInfinity(input.x)) input.x = 0f;
        if (float.IsNaN(input.y) || float.IsInfinity(input.y)) input.y = 0f;

        Vector3 moveDirection = _safeCameraForward * input.y + _safeCameraRight * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = _settings.WalkSpeed;
        if (_isSquatting) currentSpeed = _settings.WalkSpeed * 0.5f;
        else if (_isRunning) currentSpeed = _settings.RunSpeed;

        if (input.y < 0f)
        {
            currentSpeed *= _settings.BackwardSpeedMultiplier;
        }

        _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));

        if (_animator != null && _animator.isActiveAndEnabled)
        {
            float targetMultiplier = _isRunning ? 2f : 1f;
            if (_isSquatting) targetMultiplier = 0.5f;

            float targetHorizontal = input.x * targetMultiplier;
            float targetForward = input.y * targetMultiplier;

            float currentAnimX = _animator.GetFloat("MoveHorizontal");
            float currentAnimY = _animator.GetFloat("MoveForward");

            if (float.IsNaN(currentAnimX) || float.IsInfinity(currentAnimX)) currentAnimX = 0f;
            if (float.IsNaN(currentAnimY) || float.IsInfinity(currentAnimY)) currentAnimY = 0f;

            float currentHorizontal = Mathf.MoveTowards(currentAnimX, targetHorizontal, 5f * Time.deltaTime);
            float currentForward = Mathf.MoveTowards(currentAnimY, targetForward, 5f * Time.deltaTime);

            if (!float.IsNaN(currentHorizontal) && !float.IsInfinity(currentHorizontal))
            {
                _animator.SetFloat("MoveHorizontal", currentHorizontal);
            }
            if (!float.IsNaN(currentForward) && !float.IsInfinity(currentForward))
            {
                _animator.SetFloat("MoveForward", currentForward);
            }
        }
    }

    private void HandleRotation()
    {
        if (_inputService == null || _settings == null || Time.deltaTime <= 0f) return;

        Vector2 input = _inputService.MoveAxis;
        float rotSpeed = _settings.RotationSpeed;
        float targetTurnAnim = 0f;

        if (input.sqrMagnitude > 0.001f)
        {
            _isTurningBody = false;
            Vector3 targetDirection;

            if (input.y >= 0f)
            {
                targetDirection = _safeCameraForward * input.y + _safeCameraRight * input.x;
            }
            else
            {
                targetDirection = -(_safeCameraForward * Mathf.Abs(input.y) + _safeCameraRight * -input.x);
            }

            if (targetDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            }
        }
        else if (_safeCameraForward.sqrMagnitude > 0.001f)
        {
            float signedAngle = Vector3.SignedAngle(transform.forward, _safeCameraForward, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            if (absAngle > 80f)
            {
                _isTurningBody = true;
            }

            if (_isTurningBody)
            {
                targetTurnAnim = Mathf.Sign(signedAngle);

                Quaternion targetRotation = Quaternion.LookRotation(_safeCameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4.5f * Time.deltaTime);

                if (absAngle < 5f)
                {
                    _isTurningBody = false;
                    targetTurnAnim = 0f;
                }
            }
        }

        if (_animator != null && _animator.isActiveAndEnabled)
        {
            float currentTurn = _animator.GetFloat("Turn");
            if (float.IsNaN(currentTurn) || float.IsInfinity(currentTurn)) currentTurn = 0f;

            float smoothTurn = Mathf.MoveTowards(currentTurn, targetTurnAnim, 5f * Time.deltaTime);

            if (!float.IsNaN(smoothTurn) && !float.IsInfinity(smoothTurn))
            {
                _animator.SetFloat("Turn", smoothTurn);
            }
        }
    }


    //================Discreet action logic==================
    private void HandleGravity()
    {
        _isGrounded = true;// _characterController.isGrounded;
        if (_isGrounded)
        {
            /*if (_velocity.y <= 0f)
            {
                _velocity.y = -2f;
            }
            else
            {*/
                if (_velocity.y < 0f)
                {
                    _velocity.y += _settings.Gravity * 2f * Time.deltaTime;
                }
                else
                {
                    _velocity.y += _settings.Gravity * Time.deltaTime;
                }
            //}
        }
        else
        {
            _velocity.y += _settings.Gravity * Time.deltaTime;
        }

        if (_velocity.y < _settings.MaxFallSpeed) 
        {
            _velocity.y = _settings.MaxFallSpeed;
        }
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void HandleJumpPressed()
    {
        if(_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_settings.JumpHeight * -10f * _settings.Gravity);
            _animator.SetTrigger("Jump");
            Debug.Log("Jump");
        }
    }

    private void HandleSquatPressed()
    {
        _isSquatting = true;
        _isRunning = false;
        _characterController.height = _originalHeight * .5f;
        _characterController.center = _originalCenter * .5f;
        Debug.Log("Squad+");
    }
    private void HandleSquatReleased()
    {
        _isSquatting = false;
        _characterController.height = _originalHeight;
        _characterController.center = _originalCenter;
        Debug.Log("Squad-");
    }
    private void HandleAimPressed() => _isAiming = true;
    private void HandleAinReleased() => _isAiming = false;
    private void HandleDashPressed() => _isRunning = true;
    private void HandleDashReleased() => _isRunning = false;
}
