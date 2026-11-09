using UnityEngine;

[CreateAssetMenu(fileName = "InputSettings", menuName ="Configs/InputSettings")]
public class InputSettings : ScriptableObject
{
    [Header("Mouse Configuration")]
    [SerializeField] private float _mouseSensitivity = 1.0f;
    [SerializeField] private bool _invertY = false;
    public float mouseSensitivity => _mouseSensitivity;
    public bool invertY => _invertY;
}
