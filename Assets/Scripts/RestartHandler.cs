using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RestartHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _restartPanel; // Панель, содержащая шкалу
    [SerializeField] private Image _progressBar;       // UI Image со свойством Image Type = Filled

    [Header("Settings")]
    [SerializeField] private float _holdDuration = 2.0f; // Время удержания клавиши в секундах

    private float _currentHoldTime = 0f;
    private bool _isTabPressed = false;

    private void Start()
    {
        // Скрываем панель шкалы при старте
        if (_restartPanel != null) _restartPanel.SetActive(false);
        if (_progressBar != null) _progressBar.fillAmount = 0f;
    }

    private void Update()
    {
        // Проверяем состояние клавиши Tab через новую Input System напрямую
        if (Keyboard.current != null)
        {
            _isTabPressed = Keyboard.current.tabKey.isPressed;
        }

        if (_isTabPressed)
        {
            // Активируем панель при начале удержания
            if (_restartPanel != null && !_restartPanel.activeSelf)
            {
                _restartPanel.SetActive(true);
            }

            // Накапливаем время
            _currentHoldTime += Time.deltaTime;

            // Обновляем заполнение UI-картинки (значение от 0.0 до 1.0)
            if (_progressBar != null)
            {
                _progressBar.fillAmount = Mathf.Clamp01(_currentHoldTime / _holdDuration);
            }

            // Если шкала заполнилась полностью — перезапускаем текущую сцену
            if (_currentHoldTime >= _holdDuration)
            {
                RestartLevel();
            }
        }
        else
        {
            // Если игрок отпустил Tab — сбрасываем прогресс и прячем UI
            if (_currentHoldTime > 0f)
            {
                _currentHoldTime = 0f;
                if (_progressBar != null) _progressBar.fillAmount = 0f;
                if (_restartPanel != null) _restartPanel.SetActive(false);
            }
        }
    }

    private void RestartLevel()
    {
        // Получаем индекс или имя активной сцены и перезагружаем её
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
