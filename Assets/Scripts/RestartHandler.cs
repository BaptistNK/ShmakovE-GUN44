using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public class RestartHandler : MonoBehaviour
{
    private Controls.GameActions _gameInput;
    private SceneController _sceneController;

    [Inject]
    public void Construct(Controls.GameActions gameInput, SceneController sceneController)
    {
        _gameInput = gameInput;
        _sceneController = sceneController;
    }

    private void OnEnable()
    {
        _gameInput.Restart.performed += OnRestartPressed;
    }
    private void OnDisable()
    {
            _gameInput.Restart.performed -= OnRestartPressed;
    }
    private void OnRestartPressed(InputAction.CallbackContext context)
    {
        Debug.LogWarning("Клавиша перезапуска нажата");
        _sceneController.OpenGameScene();
    }
}
