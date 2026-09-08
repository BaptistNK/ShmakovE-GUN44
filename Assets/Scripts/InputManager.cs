using Zenject;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class InputManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Image _restartSlider;
    [SerializeField] private float _holdDuration = 2f;

    private Controls.GameActions _gameInput;
    private SceneController _sceneController;
    private float _currentHoldTime = 0f;
    private bool _isHolding = false;

    [Inject]
    public void Construct(Controls.GameActions gameInput, SceneController sceneController)
    {
        _gameInput = gameInput;
        _sceneController = sceneController;
    }

    private void OnEnable()
    {
        _gameInput.Restart.started += ctx => StartHolding();
        _gameInput.Restart.canceled += ctx => StopHolding();
        ResetSlider();
    }

    private void OnDisable()
    {
        _gameInput.Restart.started -= ctx => StartHolding();
        _gameInput.Restart.canceled -= ctx => StopHolding();
    }
    private void StartHolding()
    {
        _isHolding = true;
        if (_restartSlider != null)
        {
            _restartSlider.gameObject.SetActive(true);
        }
    }
    private void StopHolding()
    {
        _isHolding = false;
        ResetSlider();
    }
    private void ResetSlider()
    {
        _currentHoldTime = 0;
        if(_restartSlider!=null)
        {
            _restartSlider.fillAmount = 0f;
            _restartSlider.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (_isHolding)
        {
            _currentHoldTime += Time.deltaTime;
            if(_restartSlider!=null)
            {
                _restartSlider.fillAmount = _currentHoldTime / _holdDuration;
            }

            if(_currentHoldTime>=_holdDuration)
            {
                _isHolding = false;
                ResetSlider();
                Debug.Log("Шкала заполнена! перезапуск сцены...");
                _sceneController.OpenGameScene();
            }
        }
    }
}
