using UnityEngine.Animations.Rigging;
using UnityEngine;

public class HeadLookTarget : MonoBehaviour
{
    [Header("Config constraint")]
    public MultiAimConstraint headConstraint;
    public Transform lookTarget;
    [Header("Limits")]
    public float lookDistance = 10f;
    public float maxLookAngle = 75f;
    public float weightBlendSpeed = 6f;
    private Camera _camera;
    void Awake()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        if (_camera == null || lookTarget == null || headConstraint == null) return;
        // 1. Ставим пустышку Target строго по направлению взгляда камеры
        Vector3 cameraForward = _camera.transform.forward;
        lookTarget.position = _camera.transform.position + cameraForward * lookDistance;

        // 2. Вычисляем угол между грудью персонажа (transform.forward) и взглядом камеры
        // Обнуляем Y, чтобы смотреть только горизонтальный разворот шеи
        Vector3 flatCameraDir = cameraForward;
        flatCameraDir.y = 0f;

        float angle = Vector3.Angle(transform.forward, flatCameraDir);

        // 3. Если камера повернута слишком сильно назад — плавно возвращаем голову в исходное положение
        float targetWeight = (angle <= maxLookAngle) ? 1f : 0f;
        headConstraint.weight = Mathf.Lerp(headConstraint.weight, targetWeight, Time.deltaTime * weightBlendSpeed);
    }
}
