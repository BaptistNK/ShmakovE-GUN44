using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private IInputService _inputService;
    private PlayerSettings _settings;
    private Camera _mainCamera;

    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isAiming;
    private bool _isRunning;
    private bool _isSquatting;

    private float _originalHeight;
    private Vector3 _originalCenter;

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
        _mainCamera = Camera.main;
        _originalHeight = _characterController.height;
        _originalCenter= _characterController.center;
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
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector2 input = _inputService.MoveAxis;

        Vector3 cameraForward = _mainCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _mainCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * input.y + cameraRight * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = _settings.WalkSpeed;
        if (_isSquatting) currentSpeed = _settings.WalkSpeed * .5f; //squat speed
        else if (_isRunning) currentSpeed = _settings.RunSpeed; //run speed
        if (input.y < 0f) 
        {
            currentSpeed = _settings.BackwardSpeedMultiplier; //backward speed
        }
        _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));
    }

    private void HandleRotation()
    {
        float rotSpeed = _settings.RotationSpeed;
        
            Vector3 cameraForvard = _mainCamera.transform.forward;
            cameraForvard.y = 0f;
            if (cameraForvard.sqrMagnitude > .001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForvard);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            }
        
        /*else
        {
            Vector2 input = _inputService.MoveAxis;
            if (input.sqrMagnitude > .001f)
            {
                Vector3 cameraForvard = _mainCamera.transform.forward;
                cameraForvard.y = 0f;
                cameraForvard.Normalize();

                Vector3 cameraRight = _mainCamera.transform.right;
                cameraRight.y = 0f;
                cameraRight.Normalize();

                Vector3 moveDirection = cameraForvard * input.y + cameraRight * input.x;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            }
        }*/
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
