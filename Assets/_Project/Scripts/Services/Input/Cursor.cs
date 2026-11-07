using UnityEngine;

public class Cursor : MonoBehaviour
{
    private void Start()
    {
        // Явно указываем UnityEngine.Cursor, чтобы убрать ошибку
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }
}
