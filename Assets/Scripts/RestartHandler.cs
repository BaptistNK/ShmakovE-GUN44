using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RestartHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _restartPanel; 
    [SerializeField] private Image _progressBar;       

    [Header("Settings")]
    [SerializeField] private float _holdDuration = 2.0f;

    private float _currentHoldTime = 0f;
    private bool _isTabPressed = false;

    private void Start()
    {
        if (_restartPanel != null) _restartPanel.SetActive(false);
        if (_progressBar != null) _progressBar.fillAmount = 0f;
    }

    private void Update()
    {
        
        if (Keyboard.current != null)
        {
            _isTabPressed = Keyboard.current.tabKey.isPressed;
        }

        if (_isTabPressed)
        {
            
            if (_restartPanel != null && !_restartPanel.activeSelf)
            {
                _restartPanel.SetActive(true);
            }

            _currentHoldTime += Time.deltaTime;

            if (_progressBar != null)
            {
                _progressBar.fillAmount = Mathf.Clamp01(_currentHoldTime / _holdDuration);
            }

            if (_currentHoldTime >= _holdDuration)
            {
                RestartLevel();
            }
        }
        else
        {
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
